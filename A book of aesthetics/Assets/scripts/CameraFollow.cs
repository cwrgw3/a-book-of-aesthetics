// CameraFollow.cs
using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);
    public float smoothSpeed = 5f;

    public float ShakeTime;    // 흔들릴 시간
    public float ShakePower;   // 흔들릴 강도

    // 이 변수 하나 추가
    private Vector3 shakeOffset = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // 1) 목표 위치 계산
        Vector3 desiredPos = target.position + offset;

        // 2) 부드러운 보간
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // 3) Shake 오프셋을 더해서 최종 위치 결정
        transform.position = smoothedPos + shakeOffset;
    }

    public IEnumerator ShakeCamera()
    {
        float elapsed = 0f;

        // 원래 offset은 그대로 두고, shakeOffset만 변경
        while (elapsed < ShakeTime)
        {
            Vector2 shake2D = Random.insideUnitCircle * ShakePower;
            shakeOffset = new Vector3(shake2D.x, shake2D.y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 흔들기 끝나면 오프셋 초기화
        shakeOffset = Vector3.zero;
    }
}
