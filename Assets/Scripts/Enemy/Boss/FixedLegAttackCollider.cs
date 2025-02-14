using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedLegAttackCollider : MonoBehaviour
{
    FixedLeg fixedLeg;
    private void Awake()
    {
        fixedLeg = GetComponentInParent<FixedLeg>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")&&fixedLeg.GetIsAttack())
        {
            PlayerHP playerScript = collision.gameObject.GetComponent<PlayerHP>();
            if (playerScript != null )
            {
                Debug.Log("Fixed Leg Hand");
                playerScript.TakeDamage(1, transform.position); // float�� int�� ��ȯ
                fixedLeg.SetAttacked();
            }
        }
    }
}
