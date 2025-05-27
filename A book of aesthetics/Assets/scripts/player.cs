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

    [Header("Attack")]
    public float attackDuration = 0.4f;  
    public float attackDelay = 0.2f;    
    public Transform attackPoint;         
    public float attackRange = 1.0f;
    public LayerMask enemyLayers;

    bool isGrounded = false;
    bool isBeingHit = false;
    bool isAttacking = false;
    float h = 0f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        if (attackPoint == null)
        {
            Debug.LogWarning("[PlayerMove] attackPoint");
            attackPoint = transform;
        }
    }

    void Update()
    {
        if (isBeingHit || isAttacking)
            return;

        h = Input.GetAxisRaw("Horizontal");
        bool isMoving = h != 0f;
        anim.SetBool("isWalking", isMoving);

        if (isMoving)
            sprite.flipX = (h < 0f);

        if ((Input.GetKeyDown(KeyCode.Space)
             || Input.GetKeyDown(KeyCode.W)
             || Input.GetKeyDown(KeyCode.UpArrow))
            && isGrounded)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isGrounded = false;
            anim.SetBool("isJumping", true);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(HandleAttack());
        }
    }

    void FixedUpdate()
    {
        if (!isBeingHit && !isAttacking)
        {
            rigid.velocity = new Vector2(h * moveSpeed, rigid.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }

        if (!isBeingHit && collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(HandleHit(collision.transform));
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

    IEnumerator HandleAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);
        DoHit();

        yield return new WaitForSeconds(attackDuration - attackDelay);
        isAttacking = false;
    }

    void DoHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position, attackRange, enemyLayers);

        foreach (var col in hits)
        {
            if (col.CompareTag("Enemy"))
            {
                col.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
