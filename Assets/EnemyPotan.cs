using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPotan : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 50f; // �̵� �ӵ� ����
    PirateManager pirateManager;

    void Start()
    {
        pirateManager = GameObject.Find("GameManager").GetComponent<PirateManager>();
    }

    void Update()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // �±� �񱳴� CompareTag() ��� ����
        {
            pirateManager.DownHeart();
        }
    }
}
