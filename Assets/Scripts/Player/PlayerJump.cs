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
    private float wallJumpForceX;
    [SerializeField]
    private float wallJumpForceY;
    [SerializeField]
    private float slipForce;

    private bool onWall;
    private bool onCollision;
    private Vector3 interpolateWallNormal;

    private Dictionary<GameObject, Collision> collisions;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        collisions = new Dictionary<GameObject, Collision>();
    }
    private void FixedUpdate()
    {
        if(onWall)
            Slip();
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
            {
                rb.AddForce(jumpForce * transform.up, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce((interpolateWallNormal.normalized * wallJumpForceX) + (transform.up * wallJumpForceY), ForceMode.Impulse);
            }
        }
    }

    private void Update()
    {
        CalculateCollision();
    }

    private void CalculateCollision()
    {
        if (!onCollision) return;

        interpolateWallNormal = Vector3.zero;

        foreach(KeyValuePair<GameObject, Collision> key in collisions)
        {
            Vector3 temp = Vector3.zero;
            foreach (ContactPoint point in key.Value.contacts)
            {
                temp += point.normal;
            }
            Debug.Log(temp.normalized);
            interpolateWallNormal += temp.normalized;
        }

        float colAngle = Vector3.Angle(interpolateWallNormal.normalized, transform.up);
        Debug.Log(colAngle);

        onWall = (colAngle > 60.0f && colAngle < 120f) ? true : false;
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
        collisions.Remove(collision.gameObject);
        onCollision = false;
    }

    private void OnCollision(Collision coll)
    {
        if (collisions.ContainsKey(coll.gameObject))
        {
            onCollision = true;
            return;
        }

        collisions[coll.gameObject] = coll;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawRay(transform.position, interpolateWallNormal);
    }
}
