using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rigid;
    [SerializeField] protected SpriteRenderer render;

    [SerializeField] protected float speed = 2.5f;
    [SerializeField] protected float followDistance = 5f;    // �߰� ���� �Ÿ�
    [SerializeField] protected float stopChaseRange = 1.5f;    // ���� ���� �Ÿ� (���� �غ� �Ÿ�)
    [SerializeField] protected float Hp = 5f;
    EnemyWeapon weapon;

    Animator animator;
    [SerializeField] float attackTime = 2.5f;

    protected Transform player;
    protected bool isPlayerOnSamePlatform;
    protected bool isChasing;
    protected int nextMove;

    bool ableAttack = true;//true �� �� ���� ����
    void Awake()
    {
        //rigid = GetComponent<Rigidbody2D>();
        rigid = GetComponentInChildren<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        weapon = this.GetComponentInChildren<EnemyWeapon>();
        animator = GetComponentInChildren<Animator>();
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
            if (ableAttack)
            {
                MoveStop();
                StartCoroutine(Attack());
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

    protected void Patrol()
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

    protected void ChasePlayer()
    {
        isChasing = true;
        Vector2 direction = (player.position - transform.position).normalized;
        rigid.velocity = new Vector2(direction.x * speed, rigid.velocity.y);
        render.flipX = direction.x < 0;
    }

    protected void StopChasing()
    {
        isChasing = false;
        Think(); // ���� ���·� ��ȯ
    }
    protected void MoveStop()
    {
        rigid.velocity = Vector2.zero; // �� ����
    }
    IEnumerator Attack()
    {
        Debug.Log("�� �÷��̾� ����");
        //���� ������ �ȵǰ� �÷��� ���ֱ�
        ableAttack = false;
        // �÷��̾ ��ó�� �ִ��� Ȯ��
        animator.SetBool("IsAttack", true);
        yield return new WaitForSeconds(attackTime);
        animator.SetBool("IsAttack", false);
        //������ ������ ������
        ableAttack = true;
    }
    protected void Think()
    {
        nextMove = Random.Range(-1, 2);
        render.flipX = nextMove == -1;
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime); // ���� �ð� �� ���� ����
    }

    protected void Turn()
    {
        nextMove *= -1;
        render.flipX = nextMove == -1;
    }

    protected void CheckPlatform()
    {
        isPlayerOnSamePlatform = Mathf.Abs(player.position.y - transform.position.y) < 0.5f;
    }
    // ������ ���
    protected void TakeDamage(float damage)
    {
        Debug.Log("�ƾ�");
        Hp -= damage;
        if (Hp <= 0)
            Destroy(this.gameObject);
    }

    // ���ο�
    protected IEnumerator Slow()
    {
        speed -= 1.5f;
        yield return new WaitForSeconds(1.5f);
        speed += 1.5f;
    }
}
