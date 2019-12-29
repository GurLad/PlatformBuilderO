using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float JumpForce;
    private Rigidbody2D rigidbody;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        float vY = rigidbody.velocity.y;
        if (Input.GetButton("Jump") && IsGrounded())
        {
            vY = JumpForce;
        }
        rigidbody.velocity = new Vector2(Speed * Input.GetAxis("Horizontal"), vY);
    }
    private bool IsGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position - new Vector3(0, transform.localScale.y / 2), new Vector2(transform.localScale.x - 0.02f, 0.1f), 0);
        foreach (var item in colliders)
        {
            if (item.gameObject.layer != 8)
            {
                Debug.Log(item.gameObject);
                return true;
            }
        }
        return false;
    }
}
