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
    private float jumpingPower = 15f;//���� ����

    //�÷��̾� ���� �̵�
    private HingeJoint2D joint;
    private bool isOnRope = false;
    HingeJoint2D linkedHinge;
    [SerializeField] private float ropeForce = 15f;
    float ropeCooltime = 0.1f;
    bool ableRope = false;

    //�÷��̾� �뽬
    private bool isDash = false;
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
    public Slider HpBarSlider;
    Rigidbody2D rigid;

    //�и�
    bool isparrying = false;
    private float parryingCoolTime = 0.5f;
    bool successParrying=false;
    float DamageUpTime = 1f;
    public GameObject shield;//�ӽ� ���

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        joint = GetComponent<HingeJoint2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer �ʱ�ȭ
        rigid = GetComponent<Rigidbody2D>(); // Rigidbody2D �ʱ�ȭ
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeySetting.Keys[KeyAction.LEFT]))
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
        if (Input.GetKeyDown(KeySetting.Keys[KeyAction.UP]) && IsGrounded())
        {
            rb.velocity += new Vector2(0, jumpingPower);
        }
        if (Input.GetKey(KeySetting.Keys[KeyAction.UP]) && isOnRope)
        {
            if (!ableRope)
            {
                StartCoroutine(UpRope());
            }
        }
        if (Input.GetKey(KeySetting.Keys[KeyAction.DOWN]) && isOnRope)
        {
            if (!ableRope)
            {
                StartCoroutine(DownRope());
            }
        }
        if (Input.GetKeyUp(KeySetting.Keys[KeyAction.UP]) && rb.velocity.y > 0f)
        {
            //rb.velocity += new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        if (Input.GetKeyDown(KeySetting.Keys[KeyAction.DASH]) && Time.time >= lastDashTime + dashCooldown )
        {
            //StartCoroutine(dash());
            StartDash();
        }
        if (isDashing && Time.time >= dashTime)
        {
            EndDash();
        }
        if (Input.GetKeyDown(KeySetting.Keys[KeyAction.INTERACTION]) && isOnRope)
        {
            isOnRope = false;
            joint.enabled = false;
            //rb.velocity+=new Vector2(rb.velocity.x, rb.velocity.y);
            rb.velocity += rb.velocity.normalized * rb.velocity.magnitude * 1.5f;//1.5f�� �ݵ� ���
        }
        if (Input.GetKeyDown(KeySetting.Keys[KeyAction.PARRYING]) && !isparrying) 
        {

            StartCoroutine(Parrying());
        }

        Flip();
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
        rb.velocity = Vector2.zero;
        rb.velocity += new Vector2( dashDirection.x * dashSpeed*4f,0); // ��� �ӵ� ����
    }

    private void EndDash()
    {
        isDashing = false;
        //rb.velocity = Vector2.zero; // ��� �� ����
        rb.velocity -= new Vector2(dashDirection.x * dashSpeed * 3f, 0);
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
                Debug.Log(rb.velocity.x);
            }
        }
        

    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

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
    }
    public void SetUp(float amount)
    {
        maxHealth = amount;
        curHealth = maxHealth;
    }
   
    public void OnDamaged(Vector2 targetPos)
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerDamaged"); // ���� ���̾�
        
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);
      
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; //�˹�
        rigid.AddForce(new Vector2(dirc, 2) * 5, ForceMode2D.Impulse);

        Invoke("OffDamaged", 0.5f); //��������
    }
    void OffDamaged()
    {
        //�������� Ǯ��
        gameObject.layer = LayerMask.NameToLayer("Player"); ; // ���� ���̾� ����
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ��� �� ���� �浹�ϸ� ��� ����
        if (isDashing)
        {
            EndDash();
        }
    }

}
