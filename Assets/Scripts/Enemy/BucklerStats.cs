using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BucklerStats : EnemyStats
{
    protected override void Awake()
    {
        maxHp = 9f;
        curHp = maxHp;
        damage = 2f;
    }

    public override void TakeDamage(float damage, Transform player)
    {
        if (isDie) return;
        Debug.Log("Enemy hit" + damage); ;
        if (this.gameObject.transform.rotation.y == 0)//���� ������ ���� ���� ��
        {
            if (player.transform.position.x > gameObject.transform.position.x)//�÷��̾ ���� ���ʿ� ���� ��� 
            {
                curHp -= damage + 1;
            }
        }
        else //���� �������� �������� �� 
        {
            if (player.transform.position.x < gameObject.transform.position.x)//�÷��̾ ���� ���ʿ� ���� ��� 
            {
                curHp -= damage + 1;//�����
            }
        }

        Vector2 targetPos = new Vector2(player.transform.position.x, transform.position.y);

        // �˹� ���� ��� (targetPos���� ���� ��ġ�� ���� ���Ͱ� ����)
        Vector2 knockbackDirection = ((Vector2)transform.position - targetPos).normalized;
        Debug.Log((Vector3)knockbackDirection * knockbackForce);
        // �˹��� ������ ���ο� ��ġ�� �̵�
        transform.position += (Vector3)knockbackDirection * knockbackForce;


        CheckHp();
    }
}
