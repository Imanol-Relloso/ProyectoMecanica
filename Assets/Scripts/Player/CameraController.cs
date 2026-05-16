using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private PlayerInputActions input;

    [SerializeField] 
    private Transform cameraTransform;

    [SerializeField] 
    private float mouseSensibility = 100f;

    private Vector2 lookInput;
    private float cameraRot;

    [SerializeField]
    private float rotationSpeed = 10f;
    private bool isRotating;
    private Coroutine lookCoroutine;

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
    }

    private void Update()
    {
        MouseLook();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void MouseLook()
    {
        if(isRotating) return;

        float axisX = lookInput.x * Time.deltaTime * mouseSensibility;
        float axisY = lookInput.y * Time.deltaTime * mouseSensibility;

        cameraRot -= axisY;
        cameraRot = Mathf.Clamp(cameraRot, -75f, 75f);

        cameraTransform.localRotation = Quaternion.Euler(cameraRot, 0f, 0f);

        transform.Rotate(Vector3.up * axisX);
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
