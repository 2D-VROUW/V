using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golam : EnemyMove
{
    [SerializeField] private int attackPower; // ���ݷ� ����
    [SerializeField] public CircleCollider2D attackRange; // ���� ���� ����
    [SerializeField] private float attackDelay = 1.5f; // ���� ������
    [SerializeField] private float pushBackForce = 5f;

    private float attackCooldown = 0f; // ���� ��Ÿ�� ����

    protected override void Awake()
    {
        base.Awake(); // �θ� Ŭ������ Awake �޼��带 ȣ��
    }

    protected override void  FixedUpdate()
    {
        attackCooldown -= Time.deltaTime;

        base.CheckPlatform(); // �θ� Ŭ������ �÷��� Ȯ�� �޼��� ȣ��

        if (isPlayerOnSamePlatform && Vector2.Distance(transform.position, player.position) <= followDistance)
        {
            // �÷��̾ ���� ���� ���� ������
            if (Vector2.Distance(transform.position, player.position) > stopChaseRange)
            {
                ChasePlayer(); // ���� �Ÿ� �̻��� �� �߰�
            }
            else
            {
                StopAndPrepareAttack(); // ���� ���� ���� �����ϸ� ����
            }
        }
        else if (isChasing && Vector2.Distance(transform.position, player.position) > followDistance)
        {
            // ���� ����
            StopChasing();
        }
        else if (!isChasing)
        {
            // ���� ����
            Patrol();
        }
    }

    // Patrol �޼���� �ʿ��ϸ� Golam������ Ŀ���͸���¡
    protected override void Patrol()
    {
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);
        Vector2 frontVector = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVector, Vector3.down, Color.green);

        RaycastHit2D rayHit = Physics2D.Raycast(frontVector, Vector3.down, 2f, LayerMask.GetMask("Ground"));

        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    // StopAndPrepareAttack �޼���� Golam���� ���� �غ� ������ �߰�
    protected override void StopAndPrepareAttack()
    {
        rigid.velocity = Vector2.zero; // ���� ���߰� ��
        render.flipX = player.position.x < transform.position.x; // �÷��̾� �������� �ٶ󺸱�

        if (attackCooldown <= 0) // ��Ÿ���� ���� ��
        {
            StartCoroutine(PrepareAttack()); // ���� �غ�
        }
        else
        {
            Debug.Log("���� ��� ��, ���� ��Ÿ��: " + attackCooldown);
        }
    }

    private IEnumerator PrepareAttack()
    {
        attackCooldown = attackDelay; // ��Ÿ�� �ʱ�ȭ
        Debug.Log("���� �غ� ��...");

        // ���� �غ� �ð� ���� ����
        rigid.velocity = Vector2.zero;

        yield return new WaitForSeconds(attackDelay); // ���� �غ� �ð�

        Debug.Log("���� ����!");
        Attack(); // ���� ����
    }

    private void Attack()
    {
        Debug.Log("���� �÷��̾ �����մϴ�!");

        // �÷��̾ �������� ����
        PlayerHP playerScript = player.GetComponent<PlayerHP>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(attackPower, transform.position);
        }
    }

    // ������ ó��
    public override void TakeDamage(int damage)
    {
        Debug.Log("�ƾ�");
        Hp -= damage;

        // �˹� ���� ���
        Vector2 knockbackDirection = (transform.position - player.position).normalized; // �÷��̾�� �ݴ� ����
        float knockbackForce = 5f; // �˹� ���� (�ʿ信 ���� �� ����)

        // �˹� ����
        rigid.velocity = new Vector2(knockbackDirection.x * knockbackForce, rigid.velocity.y);

        if (Hp <= 0)
        {
            Destroy(this.gameObject); // ü���� 0 ������ �� �� ���
        }
    }

}
