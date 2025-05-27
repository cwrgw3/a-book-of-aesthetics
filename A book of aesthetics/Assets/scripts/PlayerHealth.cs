using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    public int maxHealth = 5;
    int currentHealth;

    [Header("Hit Knockback")]
    public float hitKnockbackForce = 10f;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Vector2 sourcePosition)
    {
        // 체력 차감
        currentHealth -= damage;
        Debug.Log($"Player HP = {currentHealth}/{maxHealth}");

        // 넉백
        Vector2 dir = ((Vector2)transform.position - sourcePosition).normalized;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(dir * hitKnockbackForce, ForceMode2D.Impulse);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Player Died");
        // 죽음 처리 (리스폰, 게임 오버 등)
    }
}
