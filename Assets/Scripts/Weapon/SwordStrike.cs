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
            EA.TakeDamage(2);
        }
    }
}
