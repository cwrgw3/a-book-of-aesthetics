using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    public int maxHealth = 5;
    int currentHealth;

    [Header("Hit Knockback")]
    public float hitKnockbackForce = 5f;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Vector2 sourcePosition)
    {
        // ü�� ����
        currentHealth -= damage;
        Debug.Log($"Player HP = {currentHealth}/{maxHealth}");

        // �˹�
        Vector2 dir = ((Vector2)transform.position - sourcePosition).normalized;
        rigid.velocity = Vector2.zero;
        // rigid.AddForce(dir * hitKnockbackForce, ForceMode2D.Impulse);
        rigid.AddForce((dir + Vector2.up) * hitKnockbackForce, ForceMode2D.Impulse);


        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Player Died");
        // ���� ó�� (������, ���� ���� ��)
    }
}
