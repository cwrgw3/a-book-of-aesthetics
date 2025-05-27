using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;

    [Header("Move / Jump")]
    public float moveSpeed = 10f;
    public float jumpPower = 12f;

    [Header("Hit / Knockback")]
    public float knockbackForce = 10f;
    public float hitRecoverTime = 0.5f;

    [Header("Combo Attack")]
    public float attack1Duration = 0.2f;   // 앞쪽 3프레임 애니 길이
    public float attack2Duration = 0.2f;   // 뒤쪽 3프레임 애니 길이
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;

    // 내부 상태
    bool isGrounded = false;
    bool isBeingHit = false;
    bool isAttacking = false;
    bool queuedCombo = false;
    float h = 0f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        if (attackPoint == null)
        {
            Debug.LogWarning("[PlayerMove] attackPoint not assigned, default to self.");
            attackPoint = transform;
        }
    }

    void Update()
    {
        if (isBeingHit) return;

        // 공격 중일 때만 콤보 예약
        if (isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.Z) && !queuedCombo)
            {
                queuedCombo = true;
            }
            return; // 이 상태에서는 더 이상의 입력(이동/점프/첫 공격) 무시
        }

        // 첫 공격
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(HandleComboAttack());
            return;
        }

        // ─── 이동 & 점프 ───
        h = Input.GetAxisRaw("Horizontal");
        bool isMoving = (h != 0f);
        anim.SetBool("isWalking", isMoving);
        if (isMoving) sprite.flipX = (h < 0f);

        if ((Input.GetKeyDown(KeyCode.Space) ||
             Input.GetKeyDown(KeyCode.W) ||
             Input.GetKeyDown(KeyCode.UpArrow))
            && isGrounded)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGrounded = false;
            anim.SetBool("isJumping", true);
        }
    }

    void FixedUpdate()
    {
        if (!isBeingHit && !isAttacking)
            rigid.velocity = new Vector2(h * moveSpeed, rigid.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 땅 착지 검사
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }
        // 적과 충돌 시 피격 처리
        else if (!isBeingHit && collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(HandleHit(collision.gameObject.transform));
        }
    }

    IEnumerator HandleHit(Transform enemy)
    {
        isBeingHit = true;
        anim.SetBool("isHit", true);

        Vector2 dir = (transform.position - enemy.position).normalized;
        rigid.velocity = Vector2.zero;
        rigid.AddForce((dir + Vector2.up) * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(hitRecoverTime);

        anim.SetBool("isHit", false);
        isBeingHit = false;
    }

    IEnumerator HandleComboAttack()
    {
        isAttacking = true;
        queuedCombo = false;

        // ─── 1단 공격 ───
        anim.Play("PlayerAttack1", 0, 0f);    // PlayerAttack1 스테이트(클립) 즉시 재생
        DoHit();
        yield return new WaitForSeconds(attack1Duration);

        // ─── 2단 콤보 ───
        if (queuedCombo)
        {
            queuedCombo = false;
            anim.Play("PlayerAttack2", 0, 0f);  // PlayerAttack2 스테이트(클립) 즉시 재생
            DoHit();
            yield return new WaitForSeconds(attack2Duration);
        }

        isAttacking = false;
    }


    void DoHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position, attackRange, enemyLayers);

        foreach (var hit in hits)
            if (hit.CompareTag("Enemy"))
                hit.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
