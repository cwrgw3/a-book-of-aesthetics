using System.Collections;
using UnityEngine;

public class BigSlimeAttack : MonoBehaviour
{
    [Header("프리팹 & 발사 위치")]
    public GameObject rockPrefab;    // 실제 바위 프리팹
    public Transform firePoint;      // 바위가 튀어나올 위치

    [Header("발사 설정")]
    public float shootInterval = 3f;
    public float rockSpeed = 7f;
    public float extraAnimDelay = 1f;

    [Header("Animation Settings")]
    // public AnimationClip shootClip;  // 발사 애니메이션 클립 (Inspector에서 할당)

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
        if (rockPrefab == null || firePoint == null)
        {
            Debug.LogWarning("rockPrefab 또는 firePoint가 할당되지 않았습니다!");
            return;
        }

        // 1) rockPrefab 인스턴스화
        GameObject rock = Instantiate(
            rockPrefab,
            firePoint.position,
            Quaternion.identity
        );

        // 2) 방향 계산
        Vector2 dir = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 170f;
        rock.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 3) Rigidbody2D에 힘 적용 (중력 무시)
        Rigidbody2D rb = rock.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.AddForce(dir * rockSpeed, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogWarning("rockPrefab에 Rigidbody2D가 없습니다!");
        }

    }

    
}
