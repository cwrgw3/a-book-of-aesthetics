using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    public float moveSpeed = 10f;
    public float jumpPower = 12f;
    public float knockbackForce = 10f; // 넉백 힘
    public float hitRecoverTime = 0.5f; // 피격 후 복귀 시간

    bool isGrounded = false;
    bool isBeingHit = false; // 피격 중 상태
    float h = 0f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isBeingHit) // 피격 중엔 조작 불가
            rigid.velocity = new Vector2(h * moveSpeed, rigid.velocity.y);
    }

    void Update()
    {
        if (isBeingHit) return;

        h = Input.GetAxisRaw("Horizontal");

        bool isMoving = h != 0;
        anim.SetBool("isWalking", isMoving);
        anim.speed = isMoving ? 1.2f : 1f;

        if (isMoving)
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
        // 땅 착지 처리
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }

        // 슬라임 충돌 시 피격 처리
        if (!isBeingHit && collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(HandleHit(collision.transform));
        }
    }

    private IEnumerator HandleHit(Transform enemy)
    {
        isBeingHit = true;
        anim.SetBool("isHit", true);

        // 넉백 방향 계산
        Vector2 dir = (transform.position - enemy.position).normalized;
        rigid.velocity = Vector2.zero; // 기존 속도 제거
        rigid.AddForce((dir + Vector2.up) * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(hitRecoverTime);

        anim.SetBool("isHit", false);
        isBeingHit = false;
    }
}
