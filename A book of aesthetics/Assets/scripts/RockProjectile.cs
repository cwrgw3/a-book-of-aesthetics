using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RockProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 5f;
    public LayerMask groundLayer;

    private Vector2 moveDir;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 dir)
    {
        moveDir = dir.normalized;
        rb.velocity = moveDir * speed;  // Rigidbody를 통해 이동
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Player"))
        {
            if (other.CompareTag("Player"))
                other.GetComponent<PlayerHealth>()?.TakeDamage(1, transform.position);

            Destroy(gameObject);
        }
    }
}
