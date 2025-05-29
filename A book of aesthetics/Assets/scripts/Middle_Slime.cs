using UnityEngine;

public class MiddleSlimeMove : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;    // 최대 체력
    int currentHealth;          // 현재 체력

    [Header("Damage Knockback")]
    public float damageKnockbackForce = 10f;  // 피격 시 넉백 힘

    [Header("Drop Item")]
    public GameObject collectiblePrefab;      // 죽을 때 생성할 아이템 프리팹

    [Header("Move / Jump")]
    public float jumpPower = 5f;
    public float movePower = 4f;
    public float jumpDelay = 1f;
    public LayerMask groundLayer;

    Rigidbody2D rigid;
    Animator anim;
    bool movingRight = false;
    bool isGrounded = false;
    float jumpTimer = 0f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        // 같은 레이어끼리 충돌 무시
        int slimeLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(slimeLayer, slimeLayer);
    }

    void FixedUpdate()
    {
        if (isGrounded)
        {
            jumpTimer += Time.fixedDeltaTime;
            if (jumpTimer >= jumpDelay)
            {
                if (!IsGroundAhead())
                    movingRight = !movingRight;

                Jump();
                jumpTimer = 0f;
            }
        }
        FlipDirection();
    }

    void Jump()
    {
        float dir = movingRight ? 1f : -1f;
        rigid.velocity = new Vector2(dir * movePower, jumpPower);
        isGrounded = false;
        anim.SetBool("isGrounded", false);
        anim.SetTrigger("Jump");
    }

    bool IsGroundAhead()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.right * (movingRight ? 0.5f : -0.5f);
        Vector2 down = Vector2.down;
        float distance = 1.5f;
        Debug.DrawRay(origin, down * distance, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(origin, down, distance, groundLayer);
        return hit.collider != null;
    }

    void FlipDirection()
    {
        transform.localScale = new Vector3(movingRight ? -1f : 1f, 1f, 1f);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isGrounded", true);
        }
    }

    /// <summary>
    /// 외부(플레이어)에서 호출: 데미지 적용, 넉백, 사망 처리
    /// </summary>
    public void TakeDamage(int damage, Vector2 sourcePos)
    {
        // 1) 체력 감소
        currentHealth -= damage;
        Debug.Log($"Slime HP: {currentHealth}/{maxHealth}");

        // 2) 넉백
        Vector2 knockDir = ((Vector2)transform.position - sourcePos).normalized;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(knockDir * damageKnockbackForce, ForceMode2D.Impulse);

        // 3) 사망 처리
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        // 아이템 드롭
        if (collectiblePrefab != null)
            Instantiate(collectiblePrefab, transform.position, Quaternion.identity);
        // 슬라임 제거
        Destroy(gameObject);
    }
}
