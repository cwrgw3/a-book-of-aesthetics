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

        // ▶ ① 이동 시 애니메이션
        anim.SetFloat("Speed", Mathf.Abs(h));

        // ▶ ② 좌우 방향에 따라 캐릭터 반전 (선택사항)
        if (h != 0)
            transform.localScale = new Vector3(h > 0 ? 1 : -1, 1, 1);

        // ▶ ③ 점프 입력 처리
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
