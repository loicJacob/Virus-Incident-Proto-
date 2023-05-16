
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

    private Vector3 moveDirection;
    private Vector3 aimDirection;

    private Quaternion camForwardOn2DPlane;

    private bool isFiring = false;
    private bool isHoldingOneHandedGun = true;

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
        reloadAction.performed += Reload;
        changeWeaponAction.performed += ChangeWeapon;
        changeThrowAction.performed += ChangeThrow;
        weaponAbilityAction.performed += WeaponAbility;
    }

    private void Start()
    {
        currentGun = oneHandedGunSlot.GetComponentInChildren<Gun>();
    }

    private void OnPerformFire(InputAction.CallbackContext context)
    {
        isFiring = true;
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
        currentGun.Shoot();
    }

    private void Throw(InputAction.CallbackContext context)
    {

    }

    private void Reload(InputAction.CallbackContext context)
    {
        currentGun.Reload();
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

    private void ChangeWeapon(InputAction.CallbackContext context)
    {
        isHoldingOneHandedGun = !isHoldingOneHandedGun;

        currentGun.transform.parent.gameObject.SetActive(false);
        currentGun = isHoldingOneHandedGun ? oneHandedGunSlot.GetComponentInChildren<Gun>() : twoHandedGunSlot.GetComponentInChildren<Gun>();
        currentGun.transform.parent.gameObject.SetActive(true);
        
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
