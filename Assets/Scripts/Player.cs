
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header ("Serialized Objects")]
    [SerializeField] private Camera camMain;
    [SerializeField] private GameObject lamp;
    [SerializeField] private GameObject oneHandedGunSlot;
    [SerializeField] private GameObject twoHandedGunSlot;
    [SerializeField] private GameObject specialGunSlot;

    [Header ("Settings")]
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float pressChangeWeaponTimeToHold = 0.5f;
    [SerializeField] private float pressReloadTimeToHold = 0.5f;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction aimAction;
    private InputAction fireAction;
    private InputAction throwAction;
    private InputAction knifeAction;
    private InputAction lampAction;
    private InputAction reloadAction;
    private InputAction changeWeaponAction;
    private InputAction changeThrowAction;
    private InputAction weaponAbilityAction;

    private Gun currentGun;
    private Gun lastBasicGunUsed;

    private Vector3 moveDirection;
    private Vector3 aimDirection;

    private Quaternion camForwardOn2DPlane;

    private bool isPressingFire = false;
    private bool isPressingChangeWeapon = false;
    private bool isPressingReload = false;
    private bool isHoldingChangeWeapon = false;
    private bool isHoldingReload = false;

    private float pressChangeWeaponElapsedTime = 0;
    private float pressReloadElapsedTime = 0;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];
        fireAction = playerInput.actions["Fire"];
        throwAction = playerInput.actions["Throw"];
        knifeAction = playerInput.actions["Knife"];
        lampAction = playerInput.actions["Lamp"];
        reloadAction = playerInput.actions["Reload"];
        changeWeaponAction = playerInput.actions["Change Weapon"];
        changeThrowAction = playerInput.actions["Change Throw"];
        weaponAbilityAction = playerInput.actions["Weapon Ability"];

        camForwardOn2DPlane = Quaternion.Euler(0, camMain.transform.eulerAngles.y, 0);

        fireAction.performed += OnPerformFire;
        fireAction.canceled += OnCancelFire;
        throwAction.performed += Throw;
        knifeAction.performed += Knife;
        lampAction.performed += Lamp;
        reloadAction.performed += OnPerformReload;
        reloadAction.canceled += OnCancelReload;
        changeWeaponAction.performed += OnPerformChangeWeapon;
        changeWeaponAction.canceled += OnCancelChangeWeapon;
        changeThrowAction.performed += ChangeThrow;
        weaponAbilityAction.performed += WeaponAbility;
    }

    private void Start()
    {
        currentGun = oneHandedGunSlot.GetComponentInChildren<Gun>();
        oneHandedGunSlot.SetActive(true);
        twoHandedGunSlot.SetActive(false);
        specialGunSlot.SetActive(false);
        lastBasicGunUsed = oneHandedGunSlot.activeSelf ? oneHandedGunSlot.GetComponentInChildren<Gun>() : twoHandedGunSlot.GetComponentInChildren<Gun>();
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        Move();
        Aim();
    }

    #region Get Inputs
    private void GetInput()
    {
        moveDirection = new Vector3(moveAction.ReadValue<Vector2>().x, 0, moveAction.ReadValue<Vector2>().y);
        aimDirection = new Vector3(aimAction.ReadValue<Vector2>().x, 0, aimAction.ReadValue<Vector2>().y);

        if (isPressingFire)
            OnPressFire();

        if (isPressingChangeWeapon)
            OnPressChangeWeapon();

        if (isPressingReload)
            OnPressReload();
    }

    private void OnPerformFire(InputAction.CallbackContext context)
    {
        isPressingFire = true;
    }

    private void OnPressFire()
    {
        currentGun.Shoot();
    }

    private void OnCancelFire(InputAction.CallbackContext context)
    {
        isPressingFire = false;
    }

    private void OnPerformChangeWeapon(InputAction.CallbackContext context)
    {
        isPressingChangeWeapon = true;
        isHoldingChangeWeapon = false;
        pressChangeWeaponElapsedTime = 0;
    }

    private void OnPressChangeWeapon()
    {
        pressChangeWeaponElapsedTime += Time.deltaTime;

        if (pressChangeWeaponElapsedTime >= pressChangeWeaponTimeToHold && !isHoldingChangeWeapon)
        {
            isHoldingChangeWeapon = true;

            lastBasicGunUsed.transform.parent.gameObject.SetActive(!lastBasicGunUsed.transform.parent.gameObject.activeSelf);
            specialGunSlot.SetActive(!specialGunSlot.activeSelf);
            currentGun = specialGunSlot.activeSelf ? specialGunSlot.GetComponentInChildren<Gun>() : lastBasicGunUsed;
        }
    }

    private void OnCancelChangeWeapon(InputAction.CallbackContext context)
    {
        isPressingChangeWeapon = false;

        if (!isHoldingChangeWeapon)
        {
            GameObject previousGunSlot = currentGun.transform.parent.gameObject;

            previousGunSlot.SetActive(false);
            currentGun = previousGunSlot == specialGunSlot ? 
                lastBasicGunUsed : previousGunSlot == oneHandedGunSlot ? 
                twoHandedGunSlot.GetComponentInChildren<Gun>() : oneHandedGunSlot.GetComponentInChildren<Gun>();

            currentGun.transform.parent.gameObject.SetActive(true);
            lastBasicGunUsed = currentGun;
        }
    }

    private void OnPerformReload(InputAction.CallbackContext context)
    {
        isPressingReload = true;
        isHoldingReload = false;
        pressReloadElapsedTime = 0;
    }

    private void OnPressReload()
    {
        pressReloadElapsedTime += Time.deltaTime;

        if (pressReloadElapsedTime >= pressReloadTimeToHold && !isHoldingReload)
        {
            isHoldingReload = true;
            Debug.Log("Open reload radial menu");
        }
    }

    private void OnCancelReload(InputAction.CallbackContext context)
    {
        isPressingReload = false;

        if (!isHoldingReload)
            currentGun.Reload();
    }
    #endregion

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

    private void Throw(InputAction.CallbackContext context)
    {

    }

    private void Knife(InputAction.CallbackContext context)
    {

    }

    private void Lamp(InputAction.CallbackContext context)
    {
        lamp.SetActive(!lamp.activeSelf);
    }

    private void WeaponAbility(InputAction.CallbackContext context)
    {

    }

    private void ChangeThrow(InputAction.CallbackContext context)
    {

    }

    private void OnDestroy()
    {
        fireAction.performed -= OnPerformFire;
        fireAction.canceled -= OnCancelFire;
    }
}
