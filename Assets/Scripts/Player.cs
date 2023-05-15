
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Camera camMain;
    [SerializeField] private float moveSpeed = 5;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction aimAction;
    private InputAction fireAction;
    private InputAction dashAction;

    private Vector3 moveDirection;
    private Vector2 aimDirection;

    private Quaternion camForward;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];
        fireAction = playerInput.actions["Fire"];
        dashAction = playerInput.actions["Dash"];

        camForward = Quaternion.Euler(0, 146, 0);

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
        aimDirection = aimAction.ReadValue<Vector2>();

        Debug.Log(moveDirection);
    }

    private void Move()
    {
        transform.position += Quaternion.Euler(0, camMain.transform.eulerAngles.y, 0) * moveDirection * moveSpeed * Time.deltaTime;
    }
    
    private void Aim()
    {

    }

    private void Fire(InputAction.CallbackContext context)
    {

    }

    private void Dash(InputAction.CallbackContext context)
    {

    }
}
