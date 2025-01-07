using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NER : EnemyMove
{
    [SerializeField] float attackSpeed = 2f;

    public GameObject attackRange;
    public GameObject NER_bullet;
    public GameObject target;
    private CircleCollider2D rangeCollider;

    private bool canAttack;
    private bool Wait = true;

    //���
    private void Update()
    {
        if (canAttack && Wait)
        {
            Attack();
            StartCoroutine("WaitAttack");
        }
    }
    //��Ÿ� �ޱ�
    void Start()
    {
        rangeCollider = attackRange.GetComponent<CircleCollider2D>();
        rangeCollider.isTrigger = true;
    }
    //�÷��̾� ����
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            speed = 0;
            StartCoroutine("Aim");
        }
    }
    //�÷��̾� ��ħ
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canAttack = false;
            speed = 2.5f;
            StopCoroutine("Aim");
        }
    }
    //���ؽð�
    private IEnumerator Aim()
    {
        yield return new WaitForSeconds(1f);
        canAttack = true;
    }
    //���� ��
    private IEnumerator WaitAttack()
    {
        Wait = false;
        yield return new WaitForSeconds(attackSpeed);
        Wait = true;
    }
    //���� ����
    private void Attack()
    {
        Vector3 directionToPlayer = target.transform.position - transform.position;
        directionToPlayer.z = 0f;
        directionToPlayer.Normalize();

        GameObject cpy_bullet = Instantiate(NER_bullet, transform.position, Quaternion.identity);
        Destroy(cpy_bullet, 5f);

        NER_bullet bulletComponent = cpy_bullet.GetComponent<NER_bullet>();
        bulletComponent.SetDirection(directionToPlayer);
    }
}
