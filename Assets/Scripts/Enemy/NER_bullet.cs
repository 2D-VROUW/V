using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NER_bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10f;
    private Vector3 direction;
    
    //���� ����
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += direction * bulletSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerHP PH = collision.GetComponent<PlayerHP>();
            Destroy(this.gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
}
