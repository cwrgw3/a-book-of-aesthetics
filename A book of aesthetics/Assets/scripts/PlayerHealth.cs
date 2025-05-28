using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    public int maxHealth = 5;
    public Image[] hearts;
    private int currentHealth;

    [Header("Knockback")]
    public float hitKnockbackForce = 5f;
    private Rigidbody2D rigid;

    [Header("Death UI")]
    public GameObject deathBackgroundUI;
    public GameObject deathMessageUI;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (hearts.Length != maxHealth)
        {
            Debug.LogWarning("하트 개수와 maxHealth가 일치하지 않습니다.");
        }

        // 죽음 UI는 처음에 꺼둠
        if (deathBackgroundUI != null) deathBackgroundUI.SetActive(false);
        if (deathMessageUI != null) deathMessageUI.SetActive(false);
    }

    public void TakeDamage(int damage, Vector2 sourcePosition)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log($"Player HP = {currentHealth}/{maxHealth}");

        // Knockback
        Vector2 dir = ((Vector2)transform.position - sourcePosition).normalized;
        rigid.velocity = Vector2.zero;
        rigid.AddForce((dir + Vector2.up) * hitKnockbackForce, ForceMode2D.Impulse);

        // 하트 UI 끄기
        if (currentHealth >= 0 && currentHealth < hearts.Length && hearts[currentHealth] != null)
        {
            hearts[currentHealth].enabled = false;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died");

        // YOU DIED UI 표시
        if (deathBackgroundUI != null) deathBackgroundUI.SetActive(true);
        if (deathMessageUI != null) deathMessageUI.SetActive(true);

        // TODO: 입력 중단, 재시작 버튼 활성화 등 추가 가능
    }
}
