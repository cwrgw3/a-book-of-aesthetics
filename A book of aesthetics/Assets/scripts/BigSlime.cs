using UnityEngine;

public class BigSlimeAttack : MonoBehaviour
{
    public GameObject rockPrefab;         // 발사할 돌 프리팹
    public float shootInterval = 3f;      // 발사 주기 (초)
    public float rockSpeed = 7f;          // 돌 속도

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

        // 돌 생성 위치 = BigSlime 위치
        Vector2 spawnPos = transform.position;
        GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);

        // 방향 = 플레이어를 향한 단위 벡터
        Vector2 direction = (player.position - transform.position).normalized;

        // 속도 적용
        Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * rockSpeed;
        }

        // (선택) 애니메이션 재생
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }
    }
}
