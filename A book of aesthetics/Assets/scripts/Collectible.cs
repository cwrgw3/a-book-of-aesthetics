using UnityEngine;

public class Collectible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // UIManager ���ؼ� ȹ�� ���� 1 ����
        UIManager.Instance.AddCollectible(1);

        // �����ǰ� �ı�
        Destroy(gameObject);
    }
}
