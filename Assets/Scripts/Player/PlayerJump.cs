using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInputActions input;

    [Header("Normal Jump")]
    [SerializeField]
    private float jumpForce;
    [Header("Wall Jump")]
    [SerializeField]
    private float wallJumpForceX;
    [SerializeField]
    private float wallJumpForceY;
    [Header("Wall Slip")]
    [SerializeField]
    private float slipForce;

    private bool onWall;
    private bool onCollision;
    private Vector3 interpolateWallNormal;
    private Dictionary<GameObject, Vector3> collisions;

    private CameraController camera;

    private void Awake()
    {
        input = new PlayerInputActions();
        camera = GetComponent<CameraController>();
    }

    private void OnEnable()
    {
        input.Player.Enable();

        input.Player.Jump.performed += OnJump;
        input.Player.Jump.canceled += OnJump;
    }
    private void OnDisable()
    {
        input.Player.Jump.performed -= OnJump;
        input.Player.Jump.canceled -= OnJump;

        input.Player.Disable();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        collisions = new Dictionary<GameObject, Vector3>();
    }
    private void FixedUpdate()
    {
        if(onWall)
            Slip();
    }
    private void Update()
    {
        CalculateCollision();
    }
    private void Slip()
    {
        rb.AddForce(-transform.up * slipForce);
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (onCollision)
        {
            if (!onWall)
                rb.AddForce(jumpForce * transform.up, ForceMode.Impulse);
            else
            {
                rb.AddForce((interpolateWallNormal.normalized * wallJumpForceX + transform.up * wallJumpForceY) , ForceMode.Impulse);
             
                camera.LookAt(interpolateWallNormal.normalized);
            }
        }
    }
    private void CalculateCollision()
    {
        if (!onCollision) return;

        interpolateWallNormal = Vector3.zero;

        foreach(KeyValuePair<GameObject, Vector3> key in collisions)
        {
            Vector3 temp = key.Value;
            
            interpolateWallNormal += temp;
        }

        float colAngle = Vector3.Angle(interpolateWallNormal.normalized, transform.up);

        onWall = (colAngle > 60.0f && colAngle < 120f) ? true : false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        onCollision = true;
        OnCollision(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        OnCollision(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        collisions.Remove(collision.gameObject);
        if(collisions.Count <= 0)
            onCollision = false;
    }

    private void OnCollision(Collision coll)
    {
        Vector3 temp = Vector3.zero;
        foreach (ContactPoint point in coll.contacts)
        {
            temp += point.normal;
        }
        collisions[coll.gameObject] = temp.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawRay(transform.position, interpolateWallNormal);
    }
}
