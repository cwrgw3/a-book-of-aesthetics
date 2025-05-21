using System.Collections;
using System.Collections.Generic;
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

        // �� �� �̵� �� �ִϸ��̼�
        anim.SetFloat("Speed", Mathf.Abs(h));

        // �� �� �¿� ���⿡ ���� ĳ���� ���� (���û���)
        if (h != 0)
            transform.localScale = new Vector3(h > 0 ? 1 : -1, 1, 1);

        // �� �� ���� �Է� ó��
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGrounded = false;
            anim.SetBool("IsJumping", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("IsJumping", false);
        }
    }
}
