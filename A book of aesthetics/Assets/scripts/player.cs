using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 컴포넌트 캐싱
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;

    [Header("Move / Jump")]
    public float moveSpeed = 10f;
    public float jumpPower = 12f;

    [Header("Hit / Knockback")]
    public float knockbackForce = 10f;
    public float hitRecoverTime = 0.5f;

    [Header("Attack")]
    public float attackDuration = 0.4f;    // 전체 애니 길이
    public float attackDelay = 0.2f;    // 히트 시점 딜레이
    public Transform attackPoint;           // 히트박스 위치
    public float attackRange = 1.0f;
    public LayerMask enemyLayers;

    // 내부 상태
    bool isGrounded = false;
    bool isBeingHit = false;
    bool isAttacking = false;
    float h = 0f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        // attackPoint가 없으면 자기 위치를 기본으로 사용
        if (attackPoint == null)
        {
            Debug.LogWarning("[PlayerMove] attackPoint가 할당되지 않았습니다. 기본값으로 자기 자신의 transform 사용.");
            attackPoint = transform;
        }
    }

    void Update()
    {
        // 피격 또는 공격 중이면 모든 입력 무시
        if (isBeingHit || isAttacking)
            return;

        // 1) 좌우 이동 입력
        h = Input.GetAxisRaw("Horizontal");
        bool isMoving = h != 0f;
        anim.SetBool("isWalking", isMoving);

        // flipX로 좌우 반전 (스케일 고정)
        if (isMoving)
            sprite.flipX = (h < 0f);

        // 2) 점프 입력
        if ((Input.GetKeyDown(KeyCode.Space)
             || Input.GetKeyDown(KeyCode.W)
             || Input.GetKeyDown(KeyCode.UpArrow))
            && isGrounded)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGrounded = false;
            anim.SetBool("isJumping", true);
        }

        // 3) 공격 입력
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(HandleAttack());
        }
    }

    void FixedUpdate()
    {
        // 피격·공격 중이 아닐 때만 이동
        if (!isBeingHit && !isAttacking)
        {
            rigid.velocity = new Vector2(h * moveSpeed, rigid.velocity.y);
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

        // 슬라임(Enemy) 피격 처리
        if (!isBeingHit && collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(HandleHit(collision.transform));
        }
    }

    IEnumerator HandleHit(Transform enemy)
    {
        isBeingHit = true;
        anim.SetBool("isHit", true);

        // 넉백 방향 계산 및 적용
        Vector2 dir = (transform.position - enemy.position).normalized;
        rigid.velocity = Vector2.zero;
        rigid.AddForce((dir + Vector2.up) * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(hitRecoverTime);

        anim.SetBool("isHit", false);
        isBeingHit = false;
    }

    IEnumerator HandleAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");

        // 히트 시점 대기 후 판정
        yield return new WaitForSeconds(attackDelay);
        DoHit();

        // 애니메이션 종료까지 대기
        yield return new WaitForSeconds(attackDuration - attackDelay);
        isAttacking = false;
    }

    void DoHit()
    {
        // 히트박스 범위 내 모든 적 검사
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position, attackRange, enemyLayers);

        foreach (var col in hits)
        {
            if (col.CompareTag("Enemy"))
            {
                // SendMessage로 던지면 컴포넌트가 없어도 무시
                col.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    // 히트박스 시각화 (디버그용)
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
