using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public float moveSpeed = 10f;
    public float jumpPower = 12f;

    bool isGrounded = false; // 땅에 닿아 있는지 여부

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
        // Space 키를 눌렀고, 땅에 있을 때만 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGrounded = false; // 점프했으므로 false
        }
    }

    // 바닥 충돌 체크
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // "Ground"라는 태그가 붙은 오브젝트와 충돌하면 땅에 있는 것으로 판단
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
