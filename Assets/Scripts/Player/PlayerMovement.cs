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

    private Vector2 moveDir;

    [Header("Sprint")]
    [SerializeField] 
    private float sprintSpeed;
    private bool sprint;

    [Header("Crouch / Slide")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float slideForce;
    [SerializeField] private float slideDuration;

    public bool crouch;
    public bool sliding;
    private float slideTimer;

    private CapsuleCollider capsule;

    private float standHeight;
    [SerializeField] private float crouchHeight = 1f;

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

        input.Player.Crouch.performed += OnCrouch;
        input.Player.Crouch.canceled += OnCrouch;
    }

    private void OnDisable()
    {
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;

        input.Player.Sprint.performed -= OnSprint;
        input.Player.Sprint.canceled -= OnSprint;

        input.Player.Crouch.performed -= OnCrouch;
        input.Player.Crouch.canceled -= OnCrouch;

        input.Player.Disable();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        standHeight = capsule.height;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        sprint = context.ReadValueAsButton();
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        crouch = context.ReadValueAsButton();

        if (context.performed)
            TryStartSlide();
    }

    private void Update()
    {
        SpeedControl();
        HandleCrouch();
        HandleSlide();
    }
    private void FixedUpdate()
    {
        if (sliding) return;

        float currentSpeed = normalSpeed;

        if (sliding)
            currentSpeed = sprintSpeed; 
        else if (crouch)
            currentSpeed = crouchSpeed;

        Vector3 move = transform.forward * moveDir.y + transform.right * moveDir.x;
        rb.AddForce(move * currentSpeed);
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
    private void TryStartSlide()
    {
        if (sprint && moveDir.magnitude > 0.1f && !sliding)
        {
            sliding = true;
            slideTimer = slideDuration;

            Vector3 slideDir = transform.forward;

            rb.AddForce(slideDir * slideForce, ForceMode.Impulse);
        }
    }

    private void HandleCrouch()
    {
        if (sliding) return; 

        float targetHeight = crouch ? crouchHeight : standHeight;

        capsule.height = Mathf.Lerp(capsule.height, targetHeight, Time.deltaTime * 10f);

        Vector3 center = capsule.center;
        center.y = capsule.height / 2f - 1f;
        capsule.center = center;
    }
    private void HandleSlide()
    {
        if (!sliding) return;

        slideTimer -= Time.deltaTime;

        Vector3 vel = rb.linearVelocity;
        Vector3 flatVel = new Vector3(vel.x, 0f, vel.z);

        rb.linearVelocity = new Vector3(flatVel.x, vel.y, flatVel.z);

        if (!crouch)
        {
            sliding = false;
            return;
        }

        if (slideTimer <= 0f)
        {
            sliding = false;
        }
    }
}
