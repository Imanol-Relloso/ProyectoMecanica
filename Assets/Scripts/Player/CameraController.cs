using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private PlayerInputActions input;

    [SerializeField] 
    private Transform cameraTransform;

    [SerializeField] 
    private float mouseSensibility;
    [SerializeField] 
    private float gamepadSensibility;
    private bool usingGamepad;

    private Vector2 lookInput;
    private float cameraRot;

    [SerializeField]
    private float rotationSpeed = 10f;
    private bool isRotating;
    private Coroutine lookCoroutine;

    private float standHeight;
    [SerializeField] 
    private float crouchHeight;
    [SerializeField] 
    private float slideHeight;

    [SerializeField] 
    private float heightSmooth = 10f;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        input = new PlayerInputActions();
    }
    private void OnEnable()
    {
        input.Player.Enable();

        input.Player.Look.performed += OnLook;
        input.Player.Look.canceled += OnLook;
    }
    private void OnDisable()
    {
        input.Player.Look.performed -= OnLook;
        input.Player.Look.canceled -= OnLook;

        input.Player.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerMovement = GetComponent<PlayerMovement>();
        standHeight = cameraTransform.position.y - playerMovement.transform.position.y;
    }

    private void Update()
    {
        MouseLook();
        HandleCameraHeight();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();

        usingGamepad = context.control.device is UnityEngine.InputSystem.Gamepad;
    }

    private void MouseLook()
    {
        if(isRotating) return;

        float sensibility = usingGamepad ? gamepadSensibility : mouseSensibility;

        float axisX = lookInput.x * Time.deltaTime * sensibility;
        float axisY = lookInput.y * Time.deltaTime * sensibility;

        cameraRot -= axisY;
        cameraRot = Mathf.Clamp(cameraRot, -75f, 75f);

        cameraTransform.localRotation = Quaternion.Euler(cameraRot, 0f, 0f);

        transform.Rotate(Vector3.up * axisX);
    }
    private void HandleCameraHeight()
    {
        float targetHeight = standHeight;

        if (playerMovement.sliding)
            targetHeight = slideHeight;
        else if (playerMovement.crouch || !playerMovement.CanStandUp())
            targetHeight = crouchHeight;

        Vector3 pos = cameraTransform.localPosition;

        pos.y = Mathf.Lerp(pos.y, targetHeight, Time.deltaTime * heightSmooth);

        cameraTransform.localPosition = pos;
    }
    public void LookAt(Vector3 direction)
    {
        if (lookCoroutine != null)
            StopCoroutine(lookCoroutine);

        lookCoroutine = StartCoroutine(Look(direction));
    }
    private IEnumerator Look(Vector3 direction)
    {
        isRotating = true;

        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f)
        {
            isRotating = false;
            yield break;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
        lookCoroutine = null;
    }
}
