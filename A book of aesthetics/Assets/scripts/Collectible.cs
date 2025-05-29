using UnityEngine;

public class Collectible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // UIManager 통해서 획득 개수 1 증가
        UIManager.Instance.AddCollectible(1);

        // 수집되고 파괴
        Destroy(gameObject);
    }
}
