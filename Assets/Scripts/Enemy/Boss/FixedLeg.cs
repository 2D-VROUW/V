using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedLeg : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] protected Rigidbody2D rigid;
    [SerializeField] protected SpriteRenderer render;
    [SerializeField] protected BoxCollider2D boxCollider;
    protected Animator anim; // �ִϸ��̼� �߰�
    //[SerializeField] private int attackPower; // ���ݷ� ����
    //[SerializeField] private float pushBackForce = 5f;
    [SerializeField] Boss boss;

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

        //���� ȿ����
        SoundManager.Instance.PlaySFX(19);

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
            if (player.transform.position.x < transform.position.x)
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
            else
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
    }

    // �浹 ó�� (�÷��̾��� ����)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack"))
        {
            TakeDamage(1); // ������ 1
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&&isAttack==false)
        {
            //TakeDamage(1); // ������ 1
            PlayerHP playerScript = collision.gameObject.GetComponent<PlayerHP>();
            if (playerScript != null )
            {
                playerScript.TakeDamage(1, transform.position); // float�� int�� ��ȯ
                Debug.Log("Enemy hit Player  Damage: 1" );
            }
        }
    }

    // ������ ó�� �� ���
    public virtual void TakeDamage(int damage)
    {
        Hp -= damage;
        Debug.Log("�����ٸ��� �������� ����. ���� HP: " + Hp);

        // �ǰ� �� ������ ȿ�� �߰�
        StartCoroutine(InvincibilityEffectCoroutine());

        if (Hp <= 0)
        {
            Debug.Log("�����ٸ� ���.");
            SoundManager.Instance.PlaySFX(20); // ��� ȿ���� ���
            boss.DieLeg(this);
            StartCoroutine(FadeOutAndDestroy()); // ���� ���� �� ����
        }
    }

    // �ǰ� �� ���������� ���ϰ� ���� ������ ���ƿ��� �ڷ�ƾ �߰�
    private IEnumerator InvincibilityEffectCoroutine()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null)
            yield break;

        Color originalColor = sr.color;
        Color hitColor = new Color(1f, 0.2f, 0.2f, originalColor.a); // ������ ����

        float elapsedTime = 0f;
        float duration = 0.2f; // ���� ���� ���� �ð�

        // ������ ���������� ��ȭ
        while (elapsedTime < duration)
        {
            sr.color = Color.Lerp(originalColor, hitColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        sr.color = hitColor; // ���� ������ ����

        elapsedTime = 0f;

        // ������ ���� ������ ����
        while (elapsedTime < duration)
        {
            sr.color = Color.Lerp(hitColor, originalColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        sr.color = originalColor; // ���� ���� �� ����
    }

    // ��� �� ������ ���������鼭 �����Ǵ� �ڷ�ƾ (���� �ڵ� ����)
    private IEnumerator FadeOutAndDestroy()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Destroy(gameObject);
            yield break;
        }

        float fadeDuration = 1.5f;
        float elapsedTime = 0f;
        Color originalColor = sr.color;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(gameObject);
    }
    public bool GetIsAttack()
    {
        return isAttack;
    }
    public void SetParent(Boss bs)
    {
        boss = bs;
    }
}