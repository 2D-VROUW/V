using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{//���� ���⿡ �ִ� ��ũ��Ʈ
    protected float WeaponDamage=0f;
    [SerializeField] protected GameObject thisObject;
    private BoxCollider2D hitBox;
    EnemyBehavior behavior;
    private void Awake()
    {
        EnemyStats stats = thisObject.GetComponent<EnemyStats>();
        WeaponDamage = stats.damage;
        behavior=thisObject.GetComponentInParent<EnemyBehavior>(); //enemy�� AttackMode�� ���� �÷��̾��� ü���� ��� �Ϸ��� ��
        hitBox=GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if(behavior.GetAttackMode()==true)
        {
            hitBox.enabled = true;
        }
        else
        {
            hitBox.enabled=false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHP playerScript = collision.gameObject.GetComponent<PlayerHP>();
            if (playerScript != null && behavior.GetAttackMode())
            {
                playerScript.TakeDamage((int)WeaponDamage, transform.position); // float�� int�� ��ȯ
                Debug.Log("Enemy hit Player  Damage: " + WeaponDamage);
            }
        }
    }

}
