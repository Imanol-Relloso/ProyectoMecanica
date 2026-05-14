using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] 
    private float normalSpeed;
    [SerializeField] 
    private float sprintSpeed;
    [SerializeField] 
    private float damping = 2.0f;

    private Vector2 moveDir;
    private bool sprint;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearDamping = damping;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        sprint = context.ReadValueAsButton();
    }

    private void Update()
    {
        SpeedControl();
    }
    private void FixedUpdate()
    {
        Vector3 move = transform.forward * moveDir.y + transform.right * moveDir.x;
        rb.AddForce(move * (sprint ? sprintSpeed : normalSpeed));
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = sprint ? sprintSpeed : normalSpeed;

        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
}
