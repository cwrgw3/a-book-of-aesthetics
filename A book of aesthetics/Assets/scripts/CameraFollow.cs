using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;         // 따라갈 대상 (플레이어)
    public Vector3 offset = new Vector3(0, 0, -10f); // 카메라 기본 위치 오프셋
    public float smoothSpeed = 5f;   // 따라가는 속도 (부드럽게)

    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산 (플레이어 위치 + 오프셋)
        Vector3 desiredPosition = target.position + offset;

        // 부드럽게 보간하여 따라감
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 카메라 위치 적용
        transform.position = smoothedPosition;
    }
}
