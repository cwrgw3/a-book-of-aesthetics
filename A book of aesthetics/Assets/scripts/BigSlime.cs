using UnityEngine;

public class BigSlimeAttack : MonoBehaviour
{
    public GameObject rockPrefab;         // �߻��� �� ������
    public float shootInterval = 3f;      // �߻� �ֱ� (��)
    public float rockSpeed = 7f;          // �� �ӵ�

    private Transform player;
    private float shootTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        shootTimer = shootInterval;
    }

    void Update()
    {
        if (player == null) return;

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            ShootAtPlayer();
            shootTimer = shootInterval;
        }
    }

    void ShootAtPlayer()
    {
        if (rockPrefab == null || player == null) return;

        // �� ���� ��ġ = BigSlime ��ġ
        Vector2 spawnPos = transform.position;
        GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);

        // ���� = �÷��̾ ���� ���� ����
        Vector2 direction = (player.position - transform.position).normalized;

        // �ӵ� ����
        Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * rockSpeed;
        }

        // (����) �ִϸ��̼� ���
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }
    }
}
