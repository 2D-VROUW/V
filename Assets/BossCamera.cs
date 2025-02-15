using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCamera : MonoBehaviour
{
    [SerializeField] private Transform player; // �÷��̾��� ��ġ
    [SerializeField] private float zoomSize = 15f; // �⺻ �� ũ��
    [SerializeField] private float followStrength = 0.1f; // �÷��̾ ���󰡴� ���� (�������� �� ������)
    [SerializeField] private Vector2 moveLimit = new Vector2(2f, 1f); // ī�޶� ������ �� �ִ� �ִ� �ѵ� (X, Y)

    private Vector3 initialPosition; // ī�޶��� �ʱ� ��ġ

    void Start()
    {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM(9);
        SoundManager.Instance.PlaySFX(17);
        Camera.main.orthographicSize = zoomSize; // �⺻ �� ����
        initialPosition = transform.position; // �ʱ� ��ġ ����
        Invoke("KrakenCry", 7f);
    }
    void KrakenCry()
    {
        SoundManager.Instance.PlaySFX(18);
        Invoke("KrakenCry", 120f);
    }
    void LateUpdate()
    {
        if (player == null) return;

        // �÷��̾���� �Ÿ� ��� (X, Y�ุ �ݿ�)
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);

        // �ʱ� ��ġ���� �Ÿ� ���� ����
        float limitedX = Mathf.Clamp(targetPosition.x, initialPosition.x - moveLimit.x, initialPosition.x + moveLimit.x);
        float limitedY = Mathf.Clamp(targetPosition.y, initialPosition.y - moveLimit.y, initialPosition.y + moveLimit.y);

        // �ε巴�� �̵�
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(limitedX, limitedY, transform.position.z), followStrength);

        transform.position = smoothedPosition; // ���� ��ġ ����
    }
}