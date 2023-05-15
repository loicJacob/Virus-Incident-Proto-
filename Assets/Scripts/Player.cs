
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Camera camMain;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private ParticleSystem gunFireParticles;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction aimAction;
    private InputAction fireAction;
    private InputAction dashAction;

    private Vector3 moveDirection;
    private Vector3 aimDirection;

    private Quaternion camForwardOn2DPlane;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];
        fireAction = playerInput.actions["Fire"];
        dashAction = playerInput.actions["Dash"];

        camForwardOn2DPlane = Quaternion.Euler(0, camMain.transform.eulerAngles.y, 0);

        moveAction.Enable();

        fireAction.performed += Fire;
        dashAction.performed += Dash;
    }

    private void Update()
    {
        GetInput();
        Move();
        Aim();
    }
    
    private void GetInput()
    {
        moveDirection = new Vector3(moveAction.ReadValue<Vector2>().x, 0, moveAction.ReadValue<Vector2>().y);
        aimDirection = new Vector3(aimAction.ReadValue<Vector2>().x, 0, aimAction.ReadValue<Vector2>().y);

        Debug.Log(moveDirection);
    }

    private void Move()
    {
        transform.position += camForwardOn2DPlane * moveDirection * moveSpeed * Time.deltaTime;
    }
    
    private void Aim()
    {
        if (aimDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection) * camForwardOn2DPlane;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Fire(InputAction.CallbackContext context)
    {
        gunFireParticles.Play();
    }

    private void Dash(InputAction.CallbackContext context)
    {

    }
}
