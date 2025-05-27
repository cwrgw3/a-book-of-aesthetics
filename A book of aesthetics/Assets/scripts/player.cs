using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite;

    [Header("Combo Attack (Trigger)")]
    public Transform attackPoint;         // ���� ������ Ʈ���� ������Ʈ
    public float attack1Duration = 0.2f;
    public float attack2Duration = 0.2f;

    [Header("Movement / Jump")]
    public float moveSpeed = 10f;
    public float jumpPower = 12f;

    [Header("Hit / Recover")]
    public float hitRecoverTime = 0.5f;

    // ���� ����
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
        // �ǰ� ���̸� ��� �Է� ����
        if (isBeingHit) return;

        // ���� ��� �߿� �޺� ť��
        if (isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.Z) && !queuedCombo)
                queuedCombo = true;
            return;
        }

        // ù ���� ����
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(HandleComboAttack());
            return;
        }

        // ������ �̵� & ���� ������
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
        // ���� ó��
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }
        // ������(Enemy)�� �浹 �� �ǰ� ó��
        else if (!isBeingHit && col.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(HandleHit(col.transform));
        }
    }

    IEnumerator HandleHit(Transform enemy)
    {
        isBeingHit = true;
        anim.SetBool("isHit", true);

        // �ʿ��� ��� �÷��̾� �˹� & ü�� ó�� ȣ��
        // ��: playerHealth.TakeDamage(1, enemy.position);

        yield return new WaitForSeconds(hitRecoverTime);

        anim.SetBool("isHit", false);
        isBeingHit = false;
    }

    IEnumerator HandleComboAttack()
    {
        isAttacking = true;
        queuedCombo = false;

        // 1�� ����
        anim.Play("PlayerAttack1", 0, 0f);
        yield return new WaitForSeconds(attack1Duration);

        // 2�� �޺�
        if (queuedCombo)
        {
            anim.Play("PlayerAttack2", 0, 0f);
            yield return new WaitForSeconds(attack2Duration);
        }

        isAttacking = false;
    }

    // Ʈ���� ��� ��Ʈ ����
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        var slime = other.GetComponent<MiddleSlimeMove>();
        if (slime != null)
        {
            slime.TakeDamage(1, transform.position);
        }
    }

    // ����׿�: ���� ���� �ð�ȭ
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        var col = attackPoint.GetComponent<CircleCollider2D>();
        if (col == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, col.radius * attackPoint.localScale.x);
    }
}
