using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightCloud : MonoBehaviour
{
    private bool isMoving = false;
    private float moveSpeed = 2f;

    private void Update()
    {
        if (isMoving)
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // �÷��̾ ���Դ��� Ȯ��
        {
            isMoving = true;
        }
    }
}
