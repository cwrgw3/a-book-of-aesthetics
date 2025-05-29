using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;

    [Header("Combo Attack (Trigger)")]
    public Transform attackPoint;         // 공격 판정용 트리거 오브젝트
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

    private CircleCollider2D attackCol;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        // attackPoint 셋업
        if (attackPoint == null) attackPoint = transform.GetChild(0);
        attackCol = attackPoint.GetComponent<CircleCollider2D>();
        attackCol.enabled = false;
    }

    void Update()
    {
        if (isBeingHit) return;

        // 공격 중 콤보 예약
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
        if (isMoving)
            sprite.flipX = (h < 0f);

        // attackPoint 좌우 위치 보정
        Vector3 localPos = attackPoint.localPosition;
        localPos.x = Mathf.Abs(localPos.x) * (sprite.flipX ? -1 : 1);
        attackPoint.localPosition = localPos;

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

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }
        else if (!isBeingHit && col.collider.CompareTag("Enemy"))
        {
            StartCoroutine(HandleHit(col.transform));
        }
    }

    IEnumerator HandleHit(Transform enemy)
    {
        isBeingHit = true;
        anim.SetBool("isHit", true);

        // 플레이어 넉백 & 체력 감소
        GetComponent<PlayerHealth>()?.TakeDamage(1, enemy.position);

        yield return new WaitForSeconds(hitRecoverTime);

        anim.SetBool("isHit", false);
        isBeingHit = false;
    }

    IEnumerator HandleComboAttack()
    {
        isAttacking = true;
        queuedCombo = false;

        // 공격 트리거 콜라이더 활성화
        attackCol.enabled = true;

        // 1단 공격
        anim.Play("PlayerAttack1", 0, 0f);
        yield return new WaitForSeconds(attack1Duration);

        // 2단 콤보
        if (queuedCombo)
        {
            anim.Play("PlayerAttack2", 0, 0f);
            yield return new WaitForSeconds(attack2Duration);
        }

        // 콜라이더 비활성화
        attackCol.enabled = false;
        isAttacking = false;
    }

    /// <summary>
    /// AttackPoint 트리거와 Collectible 트리거를 모두 처리
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 적 공격 판정
        if (other.CompareTag("Enemy"))
        {
            var slime = other.GetComponent<MiddleSlimeMove>();
            if (slime != null)
                slime.TakeDamage(1, transform.position);
        }
        // 아이템(Collectible) 수집 처리
        else if (other.CompareTag("Collectible"))
        {
            // UIManager를 통해 우측 상단 숫자 업데이트
            UIManager.Instance.AddCollectible(1);
            Destroy(other.gameObject);
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
