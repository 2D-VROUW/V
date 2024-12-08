using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10f;
    private Vector3 direction; // 총알의 발사 방향

    // 방향 설정 메소드
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.Translate(direction * bulletSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            EnemyMove EA = collision.GetComponent<EnemyMove>();
            Destroy(this.gameObject);
            EA.TakeDamage(1);
            EA.StartCoroutine("Slow");
        }
        if (collision.tag == "Boss")
        {
            Boss BS = collision.GetComponent<Boss>();
            Destroy(this.gameObject);
            BS.TakeDamage(1);
            BS.StartCoroutine("Slow");
        }
    }
}