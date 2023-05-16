
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header ("Serialized Objects")]
    [SerializeField] private Camera camMain;
    [SerializeField] private GameObject gunFireParticles;
    [SerializeField] private Transform gunOutput;

    [Header ("Settings")]
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float fireRate = 10;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction aimAction;
    private InputAction fireAction;
    private InputAction dashAction;

    private Vector3 moveDirection;
    private Vector3 aimDirection;

    private Quaternion camForwardOn2DPlane;

    private float fireElapsedTime = 0;

    private bool isFiring = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];
        fireAction = playerInput.actions["Fire"];
        dashAction = playerInput.actions["Dash"];

        camForwardOn2DPlane = Quaternion.Euler(0, camMain.transform.eulerAngles.y, 0);

        fireAction.performed += OnPerformFire;
        fireAction.canceled += OnCancelFire;
        dashAction.performed += Dash;
    }

    private void OnPerformFire(InputAction.CallbackContext context)
    {
        isFiring = true;
        fireElapsedTime = fireRate;
    }

    private void OnCancelFire(InputAction.CallbackContext context)
    {
        isFiring = false;
    }

    private void Update()
    {
        GetInput();

        if (isFiring)
            Fire();
    }

    private void FixedUpdate()
    {
        Move();
        Aim();
    }

    private void GetInput()
    {
        moveDirection = new Vector3(moveAction.ReadValue<Vector2>().x, 0, moveAction.ReadValue<Vector2>().y);
        aimDirection = new Vector3(aimAction.ReadValue<Vector2>().x, 0, aimAction.ReadValue<Vector2>().y);
    }

    private void Move()
    {
        transform.position += camForwardOn2DPlane * moveDirection * moveSpeed * Time.fixedDeltaTime;
    }
    
    private void Aim()
    {
        if (aimDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection) * camForwardOn2DPlane;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void Fire()
    {
        fireElapsedTime += Time.deltaTime;

        if (fireElapsedTime > fireRate)
        {
            fireElapsedTime = 0;
            Instantiate(gunFireParticles, gunOutput);
        }
    }

    private void Dash(InputAction.CallbackContext context)
    {

    }

    private void OnDestroy()
    {
        fireAction.performed -= OnPerformFire;
        fireAction.canceled -= OnCancelFire;
        dashAction.performed -= Dash;
    }
}
