using UnityEngine;

public class GolemMove : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public LayerMask wallLayer; // 벽 감지용 레이어 (Ground 포함 가능)
    public Transform wallCheckPoint;
    public float wallCheckDistance = 0.2f;

    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private bool movingRight = true;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        // 벽에 닿았으면 방향 전환
        if (IsTouchingWall())
        {
            movingRight = !movingRight;
        }

        // 이동
        float direction = movingRight ? 1f : -1f;
        rigid.velocity = new Vector2(direction * moveSpeed, rigid.velocity.y);

        // 시각적 방향 반전
        sprite.flipX = !movingRight;
    }

    bool IsTouchingWall()
    {
        if (wallCheckPoint == null)
            return false;

        Vector2 origin = wallCheckPoint.position;
        Vector2 dir = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, wallCheckDistance, wallLayer);

        Debug.DrawRay(origin, dir * wallCheckDistance, Color.red);
        return hit.collider != null;
    }

    public void TakeDamage(int damage, Vector2 sourcePos)
    {
        currentHealth -= damage;
        Debug.Log($"Golem HP: {currentHealth}/{maxHealth}");

        // 넉백 없음
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 사망 처리 (이펙트 등 추가 가능)
        Destroy(gameObject);
    }
}
