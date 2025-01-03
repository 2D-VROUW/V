using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullAttack : MonoBehaviour
{
    [SerializeField] public float pullForce = 10f; // ���Ƶ��̴� ��
    [SerializeField] public float resistanceFactor = 0.5f; // ���� ���

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // ���� �߽ɰ� �÷��̾� ��ġ
                Vector2 direction = (transform.position - collision.transform.position).normalized;

                // �÷��̾� �Է� ���� ��������
                float horizontalInput = Input.GetAxis("Horizontal");
                Vector2 inputDirection = new Vector2(horizontalInput, 0);

                // ���� ���
                float resistance = Vector2.Dot(direction, inputDirection) < 0 ? resistanceFactor : 1f;

                // �� ����
                playerRb.AddForce(direction * pullForce * resistance);
            }
        }
    }
}
