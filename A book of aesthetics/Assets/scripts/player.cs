using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;

    [Header("Combo Attack (Trigger)")]
    public Transform attackPoint;         // 공격 범위용 트리거 오브젝트
    public float attack1Duration = 0.2f;
    public float attack2Duration = 0.2f;

    [Header("Movement / Jump")]
    public float moveSpeed = 10f;
    public float jumpPower = 12f;

    [Header("Hit / Recover")]
    public float hitRecoverTime = 0.5f;

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
            attackPoint = transform;
    }

    void Update()
    {
        // 피격 중이면 모든 입력 무시
        if (isBeingHit) return;

        // 공격 모션 중엔 콤보 큐만
        if (isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.Z) && !queuedCombo)
                queuedCombo = true;
            return;
        }

        // 첫 공격 시작
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        // 착지 처리
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }
        // 슬라임(Enemy)와 충돌 시 피격 처리
        else if (!isBeingHit && col.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(HandleHit(col.transform));
        }
    }

    IEnumerator HandleHit(Transform enemy)
    {
        isBeingHit = true;
        anim.SetBool("isHit", true);

        // 필요한 경우 플레이어 넉백 & 체력 처리 호출
        // 예: playerHealth.TakeDamage(1, enemy.position);

        yield return new WaitForSeconds(hitRecoverTime);

        anim.SetBool("isHit", false);
        isBeingHit = false;
    }

    IEnumerator HandleComboAttack()
    {
        isAttacking = true;
        queuedCombo = false;

        // 1단 공격
        anim.Play("PlayerAttack1", 0, 0f);
        yield return new WaitForSeconds(attack1Duration);

        // 2단 콤보
        if (queuedCombo)
        {
            anim.Play("PlayerAttack2", 0, 0f);
            yield return new WaitForSeconds(attack2Duration);
        }

        isAttacking = false;
    }

    // 트리거 기반 히트 판정
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        var slime = other.GetComponent<MiddleSlimeMove>();
        if (slime != null)
        {
            slime.TakeDamage(1, transform.position);
        }
    }

    // 디버그용: 공격 범위 시각화
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        var col = attackPoint.GetComponent<CircleCollider2D>();
        if (col == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, col.radius * attackPoint.localScale.x);
    }
}
