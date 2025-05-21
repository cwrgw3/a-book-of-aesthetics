using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;         // ���� ��� (�÷��̾�)
    public Vector3 offset = new Vector3(0, 0, -10f); // ī�޶� �⺻ ��ġ ������
    public float smoothSpeed = 5f;   // ���󰡴� �ӵ� (�ε巴��)

    void LateUpdate()
    {
        if (target == null) return;

        // ��ǥ ��ġ ��� (�÷��̾� ��ġ + ������)
        Vector3 desiredPosition = target.position + offset;

        // �ε巴�� �����Ͽ� ����
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // ī�޶� ��ġ ����
        transform.position = smoothedPosition;
    }
}
