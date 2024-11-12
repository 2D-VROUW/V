using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private SpriteRenderer render;

    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float followDistance = 5f;    // �߰� ���� �Ÿ�
    [SerializeField] private float stopChaseRange = 2f;    // ���� ���� �Ÿ� (���� �غ� �Ÿ�)

    private Transform player;
    private bool isPlayerOnSamePlatform;
    private bool isChasing;
    private int nextMove;

    NER EA;
    void Start()
    {
        EA = GetComponentInChildren<NER>();
    }
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        Think(); // �ʱ� �̵� ���� ����
    }

    void FixedUpdate()
    {
        CheckPlatform(); // �÷��̾�� ���� �÷����� �ִ��� Ȯ��

        if (isPlayerOnSamePlatform && Vector2.Distance(transform.position, player.position) <= followDistance)
        {
            // �÷��̾ ���� ���� ���� ������
            if (Vector2.Distance(transform.position, player.position) > stopChaseRange)
            {
                // ���� �Ÿ� �̻��� �� �߰�
                ChasePlayer();
            }
            else
            {
                // ���� ���� ���� �����ϸ� ����
                StopAndPrepareAttack();
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

    private void Patrol()
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

    private void ChasePlayer()
    {
        isChasing = true;
        Vector2 direction = (player.position - transform.position).normalized;
        rigid.velocity = new Vector2(direction.x * speed, rigid.velocity.y);
        render.flipX = direction.x < 0;
    }

    private void StopChasing()
    {
        isChasing = false;
        Think(); // ���� ���·� ��ȯ
    }

    private void StopAndPrepareAttack()
    {
        rigid.velocity = Vector2.zero;
        EA.Start();
    }

    private void Think()
    {
        nextMove = Random.Range(-1, 2);
        render.flipX = nextMove == -1;
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime); // ���� �ð� �� ���� ����
    }

    private void Turn()
    {
        nextMove *= -1;
        render.flipX = nextMove == -1;
    }

    private void CheckPlatform()
    {
        isPlayerOnSamePlatform = Mathf.Abs(player.position.y - transform.position.y) < 0.5f;
    }
}