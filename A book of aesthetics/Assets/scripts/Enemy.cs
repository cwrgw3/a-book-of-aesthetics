using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp = 3;

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        Debug.Log($"{gameObject.name} took {dmg} damage. (HP: {hp})");
        if (hp <= 0)
            Die();
    }

    void Die()
    {
        // ��: ��ƼŬ, ����
        Destroy(gameObject);
    }
}
