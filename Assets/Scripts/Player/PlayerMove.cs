using JetBrains.Annotations;
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
    [SerializeField] private float speed = 8f;//�÷��̾� ���ǵ�
    private float moveInput = 0f;//�÷��̾� �¿��̵� input
    private bool isFacingRight = true;//�¿� ó�ٺ��°�
    //�÷��̾� ����
    private float jumpingPower = 16f;//���� ����

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
    [SerializeField] private float dashDuration = 0.2f;//�뽬 ���ӽð�
    [SerializeField] private float dashCoolTime = 2.0f;//�뽬 ��Ÿ��
    [SerializeField] private float dashSpeed = 10.0f;//�뽬 �ӵ�

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
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
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
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        if (Input.GetKeyDown(KeySetting.Keys[KeyAction.DASH]) && canDash && !isDash)
        {
            StartCoroutine(dash());
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
    IEnumerator dash()
    {
        isDash = true;
        canDash = false;

        Debug.Log("Dash!");

        //rb.AddForce(new Vector2(horizontal* dashSpeed, 1f), ForceMode2D.Impulse);  // ��� �� ���ϱ�

        float dashDirection = transform.localScale.x > 0 ? 1 : -1;
        rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);

        yield return new WaitForSeconds(dashDuration);
        isDash = false;
        yield return new WaitForSeconds(dashCoolTime);
        canDash = true;
    }
    private void FixedUpdate()
    {

        if (isOnRope)
        {
            rb.AddForce(new Vector2(ropeForce * moveInput, 0f));
        }
        else
        {
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
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

}
