using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullAttack : MonoBehaviour
{
    [SerializeField] public float pullForce = 10f; // 빨아들이는 힘
    [SerializeField] public float resistanceFactor = 0.5f; // 저항 계수

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // 공격 중심과 플레이어 위치
                Vector2 direction = (transform.position - collision.transform.position).normalized;

                // 플레이어 입력 방향 가져오기
                float horizontalInput = Input.GetAxis("Horizontal");
                Vector2 inputDirection = new Vector2(horizontalInput, 0);

                // 저항 계산
                float resistance = Vector2.Dot(direction, inputDirection) < 0 ? resistanceFactor : 1f;

                // 힘 적용
                playerRb.AddForce(direction * pullForce * resistance);
            }
        }
    }
}
