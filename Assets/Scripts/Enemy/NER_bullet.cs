using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NER_bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 20f;
    private Vector3 moveDirection;
    
    //방향 설정
    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
    }

    void Update()
    {
        transform.position += moveDirection * bulletSpeed * Time.deltaTime;
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
