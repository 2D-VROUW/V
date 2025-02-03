using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBullet : MonoBehaviour
{
    private Vector3 direction; // �Ѿ��� �̵� ����
    [SerializeField] private float speed = 20f; // �⺻ �ӵ�

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized; // ���� ����
    }

    private void Update()
    {
        // �Ѿ� �̵�
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾ �ٸ� ������Ʈ�� �浹 �� ó��
        if (collision.CompareTag("Player"))
        {
            // �÷��̾�� �������� �� �� �ִ� ���� �߰�
            Debug.Log("Player Hit!");
            Destroy(gameObject); // �浹 �� �Ѿ� �ı�
        }
        else if (collision.CompareTag("Obstacle"))
        {
            // ��ֹ��� ������ �Ѿ� �ı�
            Destroy(gameObject);
        }
    }
}