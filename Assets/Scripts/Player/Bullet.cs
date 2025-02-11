using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10f;
    private Vector3 direction; // �Ѿ��� �߻� ����

    PlayerMove pm;
    float damage = 1f;

    private void Start()
    {
        pm = FindObjectOfType<PlayerMove>();
    }
    // ���� ���� �޼ҵ�
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

            if (EA != null)
            {
                Debug.Log("EA");
                if (pm.GetSuccessParrying())
                    EA.TakeDamage((int)damage + 1);
                else
                    EA.TakeDamage((int)damage);
                EA.StartCoroutine("Slow");
            }
            else
            {
                EnemyStats es = collision.gameObject.GetComponentInParent<EnemyStats>();
                if (es != null)
                {
                    if (pm.GetSuccessParrying())
                        es.TakeDamage(damage + 1f, pm.gameObject.transform);
                    else
                    {
                        es.TakeDamage(damage, pm.gameObject.transform);
                    }
                    EnemyBehavior EB = collision.GetComponentInParent<EnemyBehavior>();
                    EB.StartCoroutine(EB.Slow());
                }
                else
                {
                    FixedLeg fixedLeg = collision.gameObject.GetComponent<FixedLeg>();
                    fixedLeg.TakeDamage(1);
                }
            }
        }
    }
}