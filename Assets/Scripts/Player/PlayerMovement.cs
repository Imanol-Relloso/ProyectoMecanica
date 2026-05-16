using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInputActions input;

    [Header("Movement")]
    [SerializeField] 
    private float normalSpeed;
    [SerializeField]
    private float damping = 2.0f;
    private Vector2 moveDir;

    [Header("Sprint")]
    [SerializeField] 
    private float sprintSpeed;
    private bool sprint;



    private void Awake()
    {
        input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        input.Player.Enable();

        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;

        input.Player.Sprint.performed += OnSprint;
        input.Player.Sprint.canceled += OnSprint;

    }

    private void OnDisable()
    {
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;

        input.Player.Sprint.performed -= OnSprint;
        input.Player.Sprint.canceled -= OnSprint;


        input.Player.Disable();
    }
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
