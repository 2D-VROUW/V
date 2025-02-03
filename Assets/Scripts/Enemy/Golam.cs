using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golam : EnemyMove
{
    [SerializeField] private int attackPower; // ���ݷ� ����
    [SerializeField] public BoxCollider2D attackRange; // ���� ���� ����
    [SerializeField] private float attackDelay = 1.5f; // ���� ������
    [SerializeField] private float pushBackForce = 5f;

    private float attackCooldown = 0f; // ���� ��Ÿ�� ����

    protected override void Awake()
    {
        base.Awake(); // �θ� Ŭ������ Awake �޼��带 ȣ��
    }

    protected override void FixedUpdate()
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
        PlayerMove playerMove = player.GetComponent<PlayerMove>();
        if (playerScript != null && playerMove != null)
        {
            // ���� �������� Ȯ��
            if (player.gameObject.layer != LayerMask.NameToLayer("PlayerDamaged"))
            {
                // ���� �������� �÷��̾�� ������ ����
                playerScript.TakeDamage(attackPower, transform.position); // position�� targetpos�� ����
                playerMove.OnDamaged(transform.position); // �˹� �� ���� ���� Ȱ��ȭ
            }
            else
            {
                Debug.Log("�÷��̾ ���� �����̹Ƿ� �������� �ʽ��ϴ�.");
            }
        }
    }


    // ������ ó��
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

}