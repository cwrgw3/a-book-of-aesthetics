using UnityEngine;

public class MiddleSlimeMove : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;    // 최대 체력
    int currentHealth;               // 현재 체력

    [Header("Damage Knockback")]
    public float damageKnockbackForce = 10f;  // 피격 시 넉백 힘

    [Header("Move / Jump")]
    public float jumpPower = 5f;
    public float movePower = 4f;
    public float jumpDelay = 1f;
    public LayerMask groundLayer;

    private Rigidbody2D rigid;
    private Animator anim;
    private bool movingRight = false;
    private bool isGrounded = false;
    private float jumpTimer = 0f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 레이어 충돌 무시
        int slimeLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(slimeLayer, slimeLayer);

        // 체력 초기화
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
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

    private void Jump()
    {
        float dir = movingRight ? 1f : -1f;
        rigid.velocity = new Vector2(dir * movePower, jumpPower);
        isGrounded = false;
        anim.SetBool("isGrounded", false);
        anim.SetTrigger("Jump");
    }

    private bool IsGroundAhead()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.right * (movingRight ? 0.5f : -0.5f);
        Vector2 down = Vector2.down;
        float distance = 1.5f;
        Debug.DrawRay(origin, down * distance, Color.red);
        return Physics2D.Raycast(origin, down, distance, groundLayer);
    }

    private void FlipDirection()
    {
        transform.localScale = new Vector3(movingRight ? -1f : 1f, 1f, 1f);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isGrounded", true);
        }
    }

    /// <summary>
    /// 외부에서 SendMessage 또는 GetComponent<...>().TakeDamage 로 호출
    /// </summary>
    /// <param name="damage">데미지값</param>
    /// <param name="sourcePos">공격원 위치 (넉백 방향 계산용)</param>
    public void TakeDamage(int damage, Vector2 sourcePos)
    {
        // 체력 차감
        currentHealth -= damage;
        Debug.Log($"Slime HP: {currentHealth}/{maxHealth}");

        // 넉백
        Vector2 knockDir = ((Vector2)transform.position - sourcePos).normalized;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(knockDir * damageKnockbackForce, ForceMode2D.Impulse);

        // 사망 체크
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        // 이펙트나 사운드를 추가하고 싶으면 여기에!
        Destroy(gameObject);
    }
}
