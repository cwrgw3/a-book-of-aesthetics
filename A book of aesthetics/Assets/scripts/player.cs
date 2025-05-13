using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public float moveSpeed = 10f;
    public float jumpPower = 12f;

    bool isGrounded = false; // ���� ��� �ִ��� ����

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.velocity = new Vector2(h * moveSpeed, rigid.velocity.y);
    }

    void Update()
    {
        // Space Ű�� ������, ���� ���� ���� ����
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGrounded = false; // ���������Ƿ� false
        }
    }

    // �ٴ� �浹 üũ
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // "Ground"��� �±װ� ���� ������Ʈ�� �浹�ϸ� ���� �ִ� ������ �Ǵ�
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
