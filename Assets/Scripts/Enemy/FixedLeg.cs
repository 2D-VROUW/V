using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedLeg : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] protected Rigidbody2D rigid;
    [SerializeField] protected SpriteRenderer render;
    [SerializeField] protected BoxCollider2D boxCollider;
    [SerializeField] protected Transform foot;
    protected Animator anim; // �ִϸ��̼� �߰�
    //[SerializeField] private int attackPower; // ���ݷ� ����
    //[SerializeField] private float pushBackForce = 5f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0f; // ���� ��Ÿ�� ����
    [SerializeField] private float attackDelay = 1.5f; // ���� ������
    bool isAttack=false;

    [SerializeField] protected float attackRange = 5f;    //���� �غ� �Ÿ�

    [Header("Hp")]
    [SerializeField] protected int Hp = 3;
    //[SerializeField] protected float knockbackForce = 10f;   // �˹� ��

    [Header("About Player")]
    [SerializeField] protected Transform player;//�÷��̾� ��ġ
    [SerializeField] protected Renderer playerRenderer;
    protected bool isPlayerOnSamePlatform=false;
    

    protected void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>(); // BoxCollider2D ������Ʈ ��������
        player = GameObject.FindWithTag("Player").transform;
        if(player) playerRenderer = player.GetComponent<Renderer>();
        anim = GetComponent<Animator>(); // Animator ��������
    }

    protected void FixedUpdate()
    {
        attackCooldown += Time.deltaTime;
        CheckPlatform();
        if (Vector2.Distance(transform.position, player.position) <= attackRange && attackCooldown > attackDelay&&isAttack==false) 
        {
            for (int i = 0;i<10;i++)
                Debug.Log("Attack");
            Attack();
            attackCooldown = 0f;
            isAttack =true;
        }
        Turn();
    }
    protected void Attack()
    {
        anim.SetBool("IsAttack",true);
        anim.SetBool("IsIdle", false);
        Debug.Log("Attack");
    }
    public void TriggerAttackFinish()
    {
        anim.SetBool("IsAttack", false);
        anim.SetBool("IsIdle", true);
        isAttack = false;
        attackCooldown = 0f;
        Debug.Log("FinshAttack");
    }
    protected void Turn()
    {
        if (player != null)
        {
            if (player.transform.position.x < transform.position.x) render.flipX = true;
            else render.flipX = false;
        }
    }

    protected void CheckPlatform()
    {
        isPlayerOnSamePlatform = Mathf.Abs(player.position.y - foot.position.y) < 0.5f;
    }

    // �浹 ó�� (�÷��̾��� ����)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            TakeDamage(1); // ������ 1
        }
    }

    // ������ ó�� �� ���
    public virtual void TakeDamage(int damage)
    {
        Hp -= damage;

        Debug.Log("���� �������� ����. ���� HP: " + Hp);

        // �˹� ���� ���
        Vector2 knockbackDirection = (transform.position - player.position).normalized;

        // �˹� ����
        rigid.velocity = Vector2.zero; // ���� �ӵ� �ʱ�ȭ
        //rigid.AddForce(new Vector2(knockbackDirection.x * knockbackForce, rigid.velocity.y), ForceMode2D.Impulse);

        if (Hp <= 0)
        {
            Debug.Log("���� ����߽��ϴ�.");
            Destroy(this.gameObject);
        }
    }

    //�̵� �ִϸ��̼� ����
    protected virtual void StartMoving()
    {
        if (anim != null)
            anim.SetBool("isMoving", true);
    }

    protected virtual void StopMoving()
    {
        if (anim != null)
            anim.SetBool("isMoving", false);
    }

    


}