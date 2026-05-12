using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] 
    private Transform cameraTransform;

    [SerializeField] 
    private float mouseSensibility = 100f;

    private Vector2 lookInput;
    private float cameraRot;

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
        float axisX = lookInput.x * Time.deltaTime * mouseSensibility;
        float axisY = lookInput.y * Time.deltaTime * mouseSensibility;

        cameraRot -= axisY;
        cameraRot = Mathf.Clamp(cameraRot, -75f, 75f);

        cameraTransform.localRotation = Quaternion.Euler(cameraRot, 0f, 0f);

        transform.Rotate(Vector3.up * axisX);
    }
}
