using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Serialized Objects")]
    [SerializeField] private GameObject lamp;
    [SerializeField] private GameObject oneHandedGunSlot;
    [SerializeField] private GameObject twoHandedGunSlot;
    [SerializeField] private GameObject specialGunSlot;

    [Header("Player")]
    [SerializeField] private float MoveSpeed = 2.0f;
    [SerializeField] private float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [SerializeField, Range(0.0f, 0.3f)] private float rotationWithoutAimSmoothTime = 0.12f;
    [SerializeField] private float rotationAimSpeed = 5;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float speedChangeRate = 10.0f;

    [SerializeField] private AudioClip landingAudioClip;
    [SerializeField] private AudioClip[] footstepAudioClips;
    [SerializeField, Range(0, 1)] private float footstepAudioVolume = 0.5f;

    [Space(10)]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField] private float jumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] private float fallTimeout = 0.15f;

    [Header("Player Grounded")]

    [Tooltip("Useful for rough ground")]
    [SerializeField] private float groundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [SerializeField] private float groundedRadius = 0.28f;

    [SerializeField] private LayerMask groundLayers;

    // player
    private float speed;
    private float animationBlend;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;

    private bool isGrounded = true;
    private bool isAiming = false;
    private bool isReloadMenuOpen = false;
    private bool isJumpTrigered = false;

    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDMotionSpeed;

    private Input input;
    private Animator animator;
    private CharacterController controller;
    private GameObject mainCamera;

    private Gun currentGun;
    private Gun lastBasicGunUsed;
    private Quaternion camForwardOn2DPlane;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        input = GetComponent<Input>();

        input.OnPressLamp += SwitchLampOn;
        input.OnPressChangeWeapon += SwapWeaponBasic;
        input.OnPressLongChangeWeapon += SwapWeaponSpecial;
        input.OnPressReload += Reload;
        input.OnPressLongReload += ToggleReloadMenu;
        input.OnHoldFire += Fire;
        input.OnPressJump += TriggerJump;

        AssignAnimationIDs();

        //Init gun
        currentGun = oneHandedGunSlot.GetComponentInChildren<Gun>();
        oneHandedGunSlot.SetActive(true);
        twoHandedGunSlot.SetActive(false);
        specialGunSlot.SetActive(false);
        lastBasicGunUsed = oneHandedGunSlot.activeSelf ? oneHandedGunSlot.GetComponentInChildren<Gun>() : twoHandedGunSlot.GetComponentInChildren<Gun>();

        // reset our timeouts on start
        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;

        // init cam
        camForwardOn2DPlane = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0);
    }

    private void Update()
    {
        Gravity();
        Jump();
        GroundedCheck();
        Move();
        Aim();
    }

    private void Fire()
    {
        currentGun.Shoot();
    }

    private void SwitchLampOn()
    {
        lamp.SetActive(!lamp.activeSelf);
    }

    private void SwapWeaponBasic()
    {
        GameObject previousGunSlot = currentGun.transform.parent.gameObject;

        previousGunSlot.SetActive(false);
        currentGun = previousGunSlot == specialGunSlot ?
            lastBasicGunUsed : previousGunSlot == oneHandedGunSlot ?
            twoHandedGunSlot.GetComponentInChildren<Gun>() : oneHandedGunSlot.GetComponentInChildren<Gun>();

        currentGun.transform.parent.gameObject.SetActive(true);
        lastBasicGunUsed = currentGun;
    }

    private void SwapWeaponSpecial()
    {
        lastBasicGunUsed.transform.parent.gameObject.SetActive(!lastBasicGunUsed.transform.parent.gameObject.activeSelf);
        specialGunSlot.SetActive(!specialGunSlot.activeSelf);
        currentGun = specialGunSlot.activeSelf ? specialGunSlot.GetComponentInChildren<Gun>() : lastBasicGunUsed;
    }

    private void Reload()
    {
        if (isReloadMenuOpen)
        {
            UIManager.Instance.CloseScreen(UIManager.Screens.RELOAD);
            return;
        }

        currentGun.Reload();
    }

    private void ToggleReloadMenu()
    {
        isReloadMenuOpen = !isReloadMenuOpen;

        if (isReloadMenuOpen)
            UIManager.Instance.OpenScreen(UIManager.Screens.RELOAD);
        else
            UIManager.Instance.CloseScreen(UIManager.Screens.RELOAD);
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);

        animator.SetBool(animIDGrounded, isGrounded);
    }

    private void Move()
    {
        float targetSpeed = input.IsPressingSprint ? SprintSpeed : MoveSpeed;

        if (input.MoveDirection == Vector2.zero) 
            targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = input.AnalogMovement ? input.MoveDirection.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * speedChangeRate);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);

        if (animationBlend < 0.01f) 
            animationBlend = 0f;

        // Add camForwardOn2DPlane if moevement not by cam
        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        if (isAiming)
        {
            Vector3 inputDirection = new Vector3(input.MoveDirection.x, 0, input.MoveDirection.y);
            controller.Move(camForwardOn2DPlane * inputDirection * (speed * Time.deltaTime) +
                         new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
        }
        else
        {
            controller.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                            new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
        }

        animator.SetFloat(animIDSpeed, animationBlend);
        animator.SetFloat(animIDMotionSpeed, inputMagnitude);
    }

    private void Aim()
    {
        Vector3 inputDirection;

        // Rotate with stick or rotate by movement direction
        if (input.AimDirection != Vector2.zero)
        {
            isAiming = true;
            inputDirection = new Vector3(input.AimDirection.x, 0, input.AimDirection.y).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection) * camForwardOn2DPlane;
            this.targetRotation = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationAimSpeed * Time.fixedDeltaTime);
        }
        else
        {
            isAiming = false;
            inputDirection = new Vector3(input.MoveDirection.x, 0.0f, input.MoveDirection.y).normalized;

            if (input.MoveDirection != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                    rotationWithoutAimSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
    }

    private void TriggerJump()
    {
        isJumpTrigered = true;
    }

    private void Jump()
    {
        if (!isGrounded)
            return;

        // Jump
        if (isJumpTrigered && jumpTimeoutDelta <= 0.0f)
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool(animIDJump, true);
        }

        // jump timeout
        if (jumpTimeoutDelta >= 0.0f)
            jumpTimeoutDelta -= Time.deltaTime;
    }

    private void Gravity()
    {
        if (isGrounded)
        {
            fallTimeoutDelta = fallTimeout;

            // update animator if using character
            animator.SetBool(animIDJump, false);
            animator.SetBool(animIDFreeFall, false);

            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f)
                verticalVelocity = -2f;
        }
        else
        {
            // reset the jump timeout timer
            jumpTimeoutDelta = jumpTimeout;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
                fallTimeoutDelta -= Time.deltaTime;
            else
                animator.SetBool(animIDFreeFall, true);

            isJumpTrigered = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (verticalVelocity < terminalVelocity)
            verticalVelocity += gravity * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius);
    }

    // Called in animation frame
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (footstepAudioClips.Length > 0)
            {
                var index = UnityEngine.Random.Range(0, footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(controller.center), footstepAudioVolume);
            }
        }
    }

    // Called in animation frame
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(controller.center), footstepAudioVolume);
        }
    }

    private void OnDestroy()
    {
        input.OnPressLamp -= SwitchLampOn;
        input.OnPressChangeWeapon -= SwapWeaponBasic;
        input.OnPressLongChangeWeapon -= SwapWeaponSpecial;
        input.OnPressReload -= Reload;
        input.OnPressLongReload -= ToggleReloadMenu;
        input.OnHoldFire -= Fire;
    }
}