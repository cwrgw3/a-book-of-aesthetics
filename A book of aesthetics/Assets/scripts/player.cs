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
    public float attack1Duration = 0.2f;   // ���� 3������ �ִ� ����
    public float attack2Duration = 0.2f;   // ���� 3������ �ִ� ����
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;

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
        {
            Debug.LogWarning("[PlayerMove] attackPoint not assigned, default to self.");
            attackPoint = transform;
        }
    }

    void Update()
    {
        if (isBeingHit) return;

        // ���� ���� ���� �޺� ����
        if (isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.Z) && !queuedCombo)
            {
                queuedCombo = true;
            }
            return; // �� ���¿����� �� �̻��� �Է�(�̵�/����/ù ����) ����
        }

        // ù ����
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �� ���� �˻�
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }
        // ���� �浹 �� �ǰ� ó��
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

        // ������ 1�� ���� ������
        anim.Play("PlayerAttack1", 0, 0f);    // PlayerAttack1 ������Ʈ(Ŭ��) ��� ���
        DoHit();
        yield return new WaitForSeconds(attack1Duration);

        // ������ 2�� �޺� ������
        if (queuedCombo)
        {
            queuedCombo = false;
            anim.Play("PlayerAttack2", 0, 0f);  // PlayerAttack2 ������Ʈ(Ŭ��) ��� ���
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
