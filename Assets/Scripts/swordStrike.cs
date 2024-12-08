using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordStrike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            EnemyMove EA = collision.GetComponent<EnemyMove>();
            if (EA != null)
            {
                EA.TakeDamage(2);
            }
            else
            {
                EnemyStats ES = collision.GetComponent<EnemyStats>();
                ES.TakeDamage(2, transform);
            }
        }
        if (collision.tag == "Boss")
        {
            EnemyMove EA = collision.GetComponent<EnemyMove>();
            EA.TakeDamage(2);
        }
    }
}