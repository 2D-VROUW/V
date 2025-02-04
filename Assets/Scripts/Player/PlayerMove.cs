using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    //�÷��̾� �¿� �̵�
    [SerializeField] private float speed = 2f;//�÷��̾� ���ǵ�
    private float moveInput = 0f;//�÷��̾� �¿��̵� input
    private bool isFacingRight = true;//�¿� ó�ٺ��°�
    //�÷��̾� ����
    private float jumpingPower = 25f;//���� ����

    //�÷��̾� ���� �̵�
    private HingeJoint2D joint;
    private bool isOnRope = false;
    HingeJoint2D linkedHinge;
    [SerializeField] private float ropeForce = 15f;
    float ropeCooltime = 0.1f;
    bool ableRope = false;

    //�÷��̾� �뽬
    //private bool isDash = false;
    private bool canDash = true;

    [Header("Dash Settings")]
    [SerializeField] private float dashDuration = 0.2f;//�뽬 ���ӽð�
    [SerializeField] private float dashCoolTime = 2.0f;//�뽬 ��Ÿ��
    [SerializeField] private float dashSpeed = 20.0f;//�뽬 �ӵ�


    public float dashCooldown = 1f; // ��� ���� ��� �ð�
    private Vector2 dashDirection;

    private bool isDashing = false;
    private float dashTime;
    private float lastDashTime;
    //�׿�
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    //���� ü��
    [SerializeField] private float curHealth;
    //�ִ� ü��
    [SerializeField] public float maxHealth;
    //HP ����
    private PlayerHP playerHP; // PlayerHP ���� ���� �߰�
    Rigidbody2D rigid;
   

    //�и�
    bool isparrying = false;
    private float parryingCoolTime = 0.5f;
    bool successParrying = false;
    float DamageUpTime = 1f;
    public GameObject shield;//�ӽ� ���

    private SpriteRenderer spriteRenderer;

    private float originalGravityScale; //��� �߷�

    Animator anim;

    private void Start()
    {
        joint = GetComponent<HingeJoint2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer �ʱ�ȭ
        rigid = GetComponent<Rigidbody2D>(); // Rigidbody2D �ʱ�ȭ
        playerHP = GetComponent<PlayerHP>();
        originalGravityScale = rigid.gravityScale;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeySetting.Keys[KeyAction.LEFT]))//�⺻ �¿� �̵�
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(KeySetting.Keys[KeyAction.RIGHT]))
        {
            moveInput = 1f;
        }
        else
        {
            moveInput = 0f;
        }

       

        if (Input.GetKeyDown(KeySetting.Keys[KeyAction.UP]) && IsGrounded())//�⺻ ����
        {
            rb.velocity += new Vector2(0, jumpingPower);
            anim.SetTrigger("JumpStart"); // Ʈ���� ���
        }

        float yVelocity = rb.velocity.y;
        anim.SetFloat("yVelocity", yVelocity);

        if (!IsGrounded()) // ���߿� �ִ� ����
        {
            if (yVelocity > 0.1f)
            {
                anim.SetBool("IsJump", true); // ���� ��
            }
            else if (yVelocity < -0.1f)
            {
                anim.SetBool("IsFalling", true); // ���� ��
            }
        }
        else // ���� ��
        {
            anim.SetBool("IsJump", false);
            anim.SetBool("IsFalling", false);
            anim.SetTrigger("JumpOver"); // ���� �ִϸ��̼�
        }

        /*
        if (Input.GetKeyUp(KeySetting.Keys[KeyAction.UP]) && rb.velocity.y > 0f)
        {
            //rb.velocity += new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
<<<<<<< Updated upstream
        }
        if (Input.GetKeyDown(KeySetting.Keys[KeyAction.DASH]) && Time.time >= lastDashTime + dashCooldown)
=======
        }*/



        if (Input.GetKeyDown(KeySetting.Keys[KeyAction.DASH]) && Time.time >= lastDashTime + dashCooldown )//�뽬
        {
            //StartCoroutine(dash());
            StartDash();
        }
        if (isDashing && Time.time >= dashTime)
        {
            EndDash();
        }


        if (Input.GetKey(KeySetting.Keys[KeyAction.UP]) && isOnRope)//���� �ö󰡱�
        {
            if (!ableRope)
            {
                StartCoroutine(UpRope());
            }
        }
        if (Input.GetKey(KeySetting.Keys[KeyAction.DOWN]) && isOnRope)//���� ��������
        {
            if (!ableRope)
            {
                StartCoroutine(DownRope());
            }
        }
        if (Input.GetKeyDown(KeySetting.Keys[KeyAction.INTERACTION]) && isOnRope)//���� ������
        {
            isOnRope = false;
            joint.enabled = false;
            //rb.velocity+=new Vector2(rb.velocity.x, rb.velocity.y);
            rb.velocity += rb.velocity.normalized * rb.velocity.magnitude * 1.5f;//1.5f�� �ݵ� ���

        }

        if (Input.GetKeyDown(KeySetting.Keys[KeyAction.PARRYING]) && !isparrying) //�и�
        {

            StartCoroutine(Parrying());
        }

        Flip();

        if(rigid.velocity.normalized.x == 0)
        {
            anim.SetBool("IsRun", false);
        }
        else
        {
            anim.SetBool("IsRun", true);
        }

    }
    private void StartDash()
    {
        isDashing = true;
        dashTime = Time.time + dashDuration;
        lastDashTime = Time.time;

        // ��� ���� ���� (���� �̵� ���� ����)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput == 0 && verticalInput == 0)
        {
            // ��� ������ ������ ������ �̵� �������� ����
            dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            dashDirection = new Vector2(horizontalInput, verticalInput).normalized;
        }

        rigid.velocity = Vector2.zero;
        rigid.velocity += new Vector2(dashDirection.x * dashSpeed * 4f, 0); // ��� �ӵ� ����

        rigid.gravityScale = 0; // �߷� ��Ȱ��ȭ
        IgnoreEnemyCollision(true); // Enemy���� �浹 ��Ȱ��ȭ
    }

    private void EndDash()
    {
        isDashing = false;
        rigid.velocity -= new Vector2(dashDirection.x * dashSpeed * 3f, 0); // ��� ���� �� �ӵ� ����
        rigid.gravityScale = originalGravityScale; // ���� �߷°� ����
        IgnoreEnemyCollision(false); // Enemy���� �浹 Ȱ��ȭ
    }

    private void IgnoreEnemyCollision(bool ignore)
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (playerLayer == -1 || enemyLayer == -1)
        {
            Debug.LogError("Layer names 'Player' or 'Enemy' are not defined in the Tags and Layers settings.");
            return;
        }

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, ignore);
    }

    IEnumerator Parrying()
    {
        isparrying = true;
        //���� �ð����� �и� true�� ��Ȳ���� PlayerHp�� �ִ�TakeDamage�Լ��� ����ȴٸ�
        //isparrying�� false�� �ٲ�� ���ݷ��� 1.5�ʰ� �ø�
        shield.SetActive(true);
        yield return new WaitForSeconds(parryingCoolTime);
        shield.SetActive(false);
        isparrying = false;
    }
    public bool GetParrying()
    {
        return isparrying;
    }
    public bool GetSuccessParrying()
    {
        return successParrying;
    }
    public IEnumerator ParryingSuccess()
    {
        Debug.Log("�и� ����");
        successParrying = true;
        shield.SetActive(false);
        isparrying = false;
        //animator.SetBool("IsParrying",false);
        yield return new WaitForSeconds(DamageUpTime);
        successParrying = false;
    }
    IEnumerator UpRope()
    {

        if (Rope.FindHead(linkedHinge) != linkedHinge.connectedBody)
        {
            ableRope = true;
            Rigidbody2D connectedRigidbody = linkedHinge.connectedBody;
            //���� ����Ǿ��ִ� ������Ʈ(1)���� ������Ʈ(1)�� ����ִ� ������Ʈ(2)�� ����
            joint.connectedBody = connectedRigidbody;//������Ʈ(2)�� �÷��̾ ����

            joint.anchor = new Vector2(0, 0.5f);//�÷��̾��� anchor�� ������Ʈ�� �Ʒ��κ����� ����
            joint.connectedAnchor = new Vector2(0, -0.5f);
            linkedHinge = connectedRigidbody.GetComponent<HingeJoint2D>();
            //���� ���� �� ������Ʈ(2)�� ������Ʈ(1)�� �ִ� ������ �����
        }
        yield return new WaitForSeconds(ropeCooltime);
        ableRope = false;
    }
    IEnumerator DownRope()
    {
        ableRope = true;
        Rigidbody2D connectedRigidbody = Rope.FindBefore(linkedHinge);
        //����Ǿ��ִ� ������Ʈ(1)�� ����ִ� ������Ʈ(0)�� ����
        joint.connectedBody = connectedRigidbody;//������Ʈ(0)�� �÷��̾ ����

        joint.anchor = new Vector2(0, 0.5f);//�÷��̾��� anchor�� ������Ʈ�� �Ʒ��κ����� ����
        joint.connectedAnchor = new Vector2(0, -0.5f);
        linkedHinge = connectedRigidbody.GetComponent<HingeJoint2D>();
        //���� ���� �� ������Ʈ(0)�� ������Ʈ(1)�� �ִ� ������ �����
        yield return new WaitForSeconds(ropeCooltime);
        ableRope = false;
    }

    private void FixedUpdate()
    {

        if (isOnRope)
        {
            rb.AddForce(new Vector2(ropeForce * moveInput, 0f));
        }

        else if (rb.velocity.x >= -speed && rb.velocity.x <= speed)
        {
            if (moveInput != 0)
            {
                rb.velocity += new Vector2(moveInput * speed / 8, 0);
            }
        }


    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    /* private void Flip()
     {
         if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
         {
             isFacingRight = !isFacingRight;
             Vector3 localScale = transform.localScale;
             localScale.x *= -1f;
             transform.localScale = localScale;

         }
     }
    */

    private void Flip()
    {
        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

            // Flip ���� ���� �� �ִϸ��̼� ������Ʈ
            anim.SetBool("IsRun", true); // �ִϸ��̼� ���� ��ȯ
        }
    }



    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Rope") && !isOnRope && Input.GetKey(KeySetting.Keys[KeyAction.UP]))
        {
            joint.enabled = true;
            Rigidbody2D ropeRb = coll.GetComponent<Rigidbody2D>();
            joint.connectedBody = ropeRb;

            joint.anchor = new Vector2(0, 0.5f);
            joint.connectedAnchor = new Vector2(0, -0.5f);

            isOnRope = true;
            linkedHinge = coll.GetComponent<HingeJoint2D>();


        }
    }

    public void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (rigid == null)
        {
            Debug.LogError("Rigidbody2D not found on Player!");
        }
    }
    public void SetUp(float amount)
    {
        maxHealth = amount;
        curHealth = maxHealth;
    }

    public void OnDamaged(Vector2 targetPos)
    {
        if (gameObject.layer == LayerMask.NameToLayer("PlayerDamaged"))
        {
            // �̹� ���� ������ ��� ó������ ����
            return;
        }

        gameObject.layer = LayerMask.NameToLayer("PlayerDamaged"); // ���� ���̾� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.3f); // ���� ���� �� ���� ����

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; // �˹� ���� ����
        rigid.AddForce(new Vector2(dirc, 2) * 5, ForceMode2D.Impulse); // �˹� ����

        StartCoroutine(HandleTemporaryInvincibility(1.5f)); // ���� ���� ���� �ڷ�ƾ ȣ��
    }

    void OffDamaged()
    {
        gameObject.layer = LayerMask.NameToLayer("Player"); // ���� ���̾� ����
        spriteRenderer.color = new Color(1, 1, 1, 1); // ���� ���·� ����
    }

    IEnumerator HandleTemporaryInvincibility(float duration)
    {
        int playerLayer = LayerMask.NameToLayer("PlayerDamaged");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (playerLayer == -1 || enemyLayer == -1)
        {
            Debug.LogError("Layer names 'PlayerDamaged' or 'Enemy' are not defined in the Tags and Layers settings.");
            yield break;
        }

        // �浹�� �����ϵ��� ����
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        // ���� Ÿ�̸�
        yield return new WaitForSeconds(duration);

        // �浹 �ٽ� Ȱ��ȭ
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        OffDamaged(); // ���� ����
    }


    IEnumerator TemporarilyIgnoreEnemyCollision(float duration)
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (playerLayer == -1 || enemyLayer == -1)
        {
            Debug.LogError("Layer names 'Player' or 'Enemy' are not defined in the Tags and Layers settings.");
            yield break;
        }

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true); // �÷��̾�� ���� �浹 ����
        yield return new WaitForSeconds(duration);
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false); // �浹 ����
    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            EndDash();
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 targetPos = collision.transform.position;

            // �˹� ���� ����
            OnDamaged(targetPos);

            // PlayerHP�� TakeDamage ȣ��
            playerHP.TakeDamage(1, targetPos); // ����� ó�� �� �˹� ����
        }
    }

}