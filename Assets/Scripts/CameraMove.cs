using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;  // �÷��̾�
    public Vector3 offset = new Vector3(0, 2.5f, -10);
    public float smoothSpeed = 0.125f;  // �ε巯�� �̵� �ӵ�

    void LateUpdate()
    {
        if (target != null)
        {
            // ����� ��ġ�� ������ ����
            Vector3 desiredPosition = target.position + offset;
            // �ε巴�� �̵�
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
