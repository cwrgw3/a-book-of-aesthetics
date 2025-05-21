using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    public float moveSpeed = 10f;
    public float jumpPower = 12f;

    bool isGrounded = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(h * moveSpeed, rigid.velocity.y);
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        bool isMoving = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        if (anim.GetBool("isWalking") != isMoving)
        {
            anim.SetBool("isWalking", isMoving);
        }

        if (h != 0)
            transform.localScale = new Vector3(h > 0 ? 2 : -2, 2, 2);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGrounded = false;
            anim.SetBool("isJumping", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }
    }
}
