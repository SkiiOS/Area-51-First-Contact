using UnityEngine;
using System.Collections;   
using System.Collections.Generic;

public class PlayerControler : MonoBehaviour
{
    public Rigidbody2D rb;
    bool isGrounded = true;

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.linearVelocity = new Vector2(-5, rb.linearVelocity.y);
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.linearVelocity = new Vector2(5, rb.linearVelocity.y);
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
