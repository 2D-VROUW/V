using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NER_bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 20f;
    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;

        // ���⿡ �°� �Ѿ� ȸ��
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.GetChild(0).transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void Update()
    {
        // ������ ���� �̵�
        transform.Translate(Vector3.right * bulletSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerHP PH = collision.GetComponent<PlayerHP>();
            // vector2�� �����
            PH.TakeDamage(1, transform.position);
            Destroy(this.gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
}
