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
    private float airMultiplier = 0.2f;

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

    private PlayerJump playerJump;

    
    [Header("Ceiling Check")]
    [SerializeField] 
    private float ceilingDistance;
    [SerializeField] 
    private float ceilingCheckRadius;
    [SerializeField] 
    private LayerMask groundMask;

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
        playerJump = GetComponent<PlayerJump>();

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

        if (sprint)
            currentSpeed = sprintSpeed; 
        else if (crouch)
            currentSpeed = crouchSpeed;

        if (!playerJump.Grounded)
            currentSpeed *= airMultiplier;

        Vector3 move = transform.forward * moveDir.y + transform.right * moveDir.x;
        rb.AddForce(move * currentSpeed);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = normalSpeed;
        
        if (sprint)
            speed = sprintSpeed;
        else if (crouch)
            speed = crouchSpeed;

        if (!playerJump.Grounded)
            speed *= airMultiplier;

        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
    private void TryStartSlide()
    {
        if (sprint && moveDir.magnitude > 0.1f && !sliding && playerJump.Grounded)
        {
            sliding = true;
            slideTimer = slideDuration;

            Vector3 slideDir = transform.forward;

            rb.AddForce(slideDir * slideForce, ForceMode.Impulse);
        }
    }

    private void HandleCrouch()
    {
        bool shouldBeCrouched = crouch || sliding;

        if (!shouldBeCrouched && !CanStandUp()) shouldBeCrouched = true;

        float targetHeight = shouldBeCrouched ? crouchHeight : standHeight;

        capsule.height = Mathf.Lerp(capsule.height, targetHeight, Time.deltaTime * 10f);

        Vector3 center = capsule.center;
        center.y = capsule.height / 2f - 1f;
        capsule.center = center;
    }
    public bool CanStandUp()
    {
        Vector3 bottom = transform.position + Vector3.up * 0.1f;
        Vector3 top = transform.position + Vector3.up * standHeight;

        return !Physics.CheckCapsule(bottom, top, ceilingCheckRadius, groundMask);
    }
    private void HandleSlide()
    {
        if (!sliding) return;

        slideTimer -= Time.deltaTime;

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
