using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    public float moveSpeed = 10f;
    public float jumpPower = 12f;
    public float knockbackForce = 10f; // �˹� ��
    public float hitRecoverTime = 0.5f; // �ǰ� �� ���� �ð�

    bool isGrounded = false;
    bool isBeingHit = false; // �ǰ� �� ����
    float h = 0f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isBeingHit) // �ǰ� �߿� ���� �Ұ�
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
        // �� ���� ó��
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }

        // ������ �浹 �� �ǰ� ó��
        if (!isBeingHit && collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(HandleHit(collision.transform));
        }
    }

    private IEnumerator HandleHit(Transform enemy)
    {
        isBeingHit = true;
        anim.SetBool("isHit", true);

        // �˹� ���� ���
        Vector2 dir = (transform.position - enemy.position).normalized;
        rigid.velocity = Vector2.zero; // ���� �ӵ� ����
        rigid.AddForce((dir + Vector2.up) * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(hitRecoverTime);

        anim.SetBool("isHit", false);
        isBeingHit = false;
    }
}
