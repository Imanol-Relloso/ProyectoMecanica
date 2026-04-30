using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float impulseY;

    private bool onCollision;
    private Vector3 interpolateWallNormal;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (onCollision)
        {
            rb.AddForce(jumpForce * (interpolateWallNormal.normalized + transform.up * impulseY), ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        OnCollision(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        onCollision = false;
    }

    private void OnCollision(Collision coll)
    {
        onCollision = true;
        interpolateWallNormal = Vector3.zero;

        foreach (ContactPoint contactPoint in coll.contacts)
        {
            interpolateWallNormal += contactPoint.normal;
        }
    }
}
