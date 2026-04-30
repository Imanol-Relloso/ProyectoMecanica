using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] 
    private float speed;

    private Vector2 moveDir;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.AddForce((new Vector3(moveDir.x, 0, moveDir.y)) * speed);
    }
}
