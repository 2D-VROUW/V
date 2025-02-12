using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;  // �÷��̾�
    public Vector3 offset = new Vector3(0, 3f, -10); // �⺻ ��ġ
    public Vector3 downOffset = new Vector3(0, 3f, -10); // �Ʒ� ���� �̵� �� ��ġ

    public float smoothSpeed = 0.125f;  // �ε巯�� �̵� �ӵ�

    public float normalZoom = 8f;  // �⺻ ��
    public float idleZoom = 10f;   // �÷��̾ ������ ���� �� ��
    public float downZoom = 12f;   // �Ʒ� ����Ű�� ������ �� ���� ��
    public float zoomSpeed = 2f;   // �� ��ȭ �ӵ�
    public float idleDelay = 2f;   // ���� ���� �� �� �ƿ����� �ɸ��� �ð�

    private Camera cam;
    private float targetZoom;
    private bool isMoving = false; // �÷��̾ �̵� ������ �Ǻ�
    private float idleTimer = 0f;  // ���� ���¿��� ��� �ð�

    // ȭ�� ���� ���� ���� �߰�
    private bool isShaking = false;
    private float shakeDuration = 0.5f;  // ��鸮�� ���� �ð�
    private float shakeMagnitude = 0.2f; // ��鸮�� ����
    private Vector3 originalPosition;

    // �Ʒ� ����Ű ���� ���� ����
    private float downKeyHoldTime = 0f;
    private float holdDuration = 1f; // 1�� �̻� ������ ȿ�� ����
    private bool isLookingDown = false;

    void Start()
    {
        cam = Camera.main;
        cam.orthographicSize = normalZoom;  // �ʱ� �� ����
        targetZoom = normalZoom;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            if (!isShaking) // ��鸮�� ���ȿ��� ī�޶� ������ ���ع��� �ʵ���
            {
                // �Ʒ� ����Ű ���� �� ����
                if (Input.GetKey(KeySetting.Keys[KeyAction.DOWN]))
                {
                    downKeyHoldTime += Time.deltaTime;
                    if (downKeyHoldTime >= holdDuration)
                    {
                        isLookingDown = true;
                    }
                }
                else
                {
                    downKeyHoldTime = 0f;
                    isLookingDown = false;
                }

                // �÷��̾� �̵� ���� �Ǻ�
                if (Input.GetKey(KeySetting.Keys[KeyAction.LEFT]) || Input.GetKey(KeySetting.Keys[KeyAction.RIGHT]))
                {
                    isMoving = true;
                    idleTimer = 0f; // �̵��ϸ� Ÿ�̸� �ʱ�ȭ
                }
                else
                {
                    if (!isMoving)
                    {
                        idleTimer += Time.deltaTime; // ���� ���¿��� �ð� ����
                    }
                    isMoving = false;
                }

                // ī�޶� �� ����
                if (isLookingDown)
                {
                    targetZoom = downZoom; // �Ʒ� ����Ű ������ ����
                }
                else
                {
                    targetZoom = (idleTimer >= idleDelay) ? idleZoom : normalZoom;
                }

                // ���� �ε巴�� ����
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

                // ī�޶� ��ġ ����
                Vector3 desiredPosition = target.position + (isLookingDown ? downOffset : offset);
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }
        }
    }

    // ȭ�� ���� ��� �߰�
    public void StartShake()
    {
        if (!isShaking)
        {
            StartCoroutine(Shake());
        }
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        originalPosition = transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float xOffset = Random.Range(-1f, 1f) * shakeMagnitude;
            float yOffset = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = originalPosition + new Vector3(xOffset, yOffset, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition; // ���� ��ġ�� ����
        isShaking = false;
    }
}
