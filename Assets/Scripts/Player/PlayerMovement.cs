using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] 
    private float speed;
    [SerializeField] 
    private float sprintSpeed;

    private Vector2 moveDir;
    private bool sprint;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        sprint = context.ReadValueAsButton();
    }

    private void FixedUpdate()
    {
        Vector3 move = transform.forward * moveDir.y + transform.right * moveDir.x;
        float currentSpeed = sprint ? sprintSpeed : speed;

        Vector3 velocity = move * currentSpeed;
        velocity.y = rb.linearVelocity.y; 

        rb.linearVelocity = velocity;
    }
}
