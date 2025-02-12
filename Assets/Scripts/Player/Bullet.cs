using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] private TrailRenderer trailRenderer; // Trail Renderer �߰�
    private Vector3 direction; // �Ѿ��� �߻� ����

    PlayerMove pm;
    float damage = 1f;

    private void Start()
    {
        pm = FindObjectOfType<PlayerMove>();

        // Trail Renderer ���� (������ �߰�)
        if (trailRenderer == null)
        {
            trailRenderer = gameObject.AddComponent<TrailRenderer>();
        }

        trailRenderer.time = 0.2f; // �ܻ��� ���� �ð�
        trailRenderer.startWidth = 0.2f;
        trailRenderer.endWidth = 0f;
        trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        trailRenderer.startColor = new Color(1f, 1f, 1f, 0.8f); // �ణ ������ ���
        trailRenderer.endColor = new Color(1f, 1f, 1f, 0f);
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
            return;
        }

        if (collision.tag == "Enemy")
        {
            EnemyMove EA = collision.GetComponent<EnemyMove>();

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
                        es.TakeDamage(damage, pm.gameObject.transform);

                    EnemyBehavior EB = collision.GetComponentInParent<EnemyBehavior>();
                    if (EB != null)
                    {
                        EB.StartCoroutine(EB.Slow());
                    }
                }
                else
                {
                    FixedLeg fixedLeg = collision.gameObject.GetComponent<FixedLeg>();
                    if (fixedLeg != null)
                    {
                        fixedLeg.TakeDamage(1);
                    }
                }
            }
        }
    }
}