using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour
{

    [SerializeField] protected float hp = 100;
    [SerializeField] protected float attackSpeed = 3f;

    protected int page = 1;
    private int attackPatern = 0;
    private bool canAttack = true;
    private int attackStack = 0;
    bool eyeattack = false;
    public Animator anim; 

    public Collider2D headCollider;

    private float groggyTime = 10f;
    bool isGroggy=false;


    [Header("Leg Patern")]
    public GameObject patern2;//Leg Patern
    BossPatern2 legAttack;


    [Header("Fixed Leg")]
    [SerializeField] private GameObject FixedLegPrefab;
    private GameObject fixedLeg1;
    private GameObject fixedLeg2;
    private FixedLeg fLeg1;
    private FixedLeg fLeg2;//--------------------------------------------------------------------------�����ٸ� ��ũ��Ʈ
    [SerializeField] List<Vector2> leftFixedLegSpawnPoint;
    [SerializeField] List<Vector2> RightFixedLegSpawnPoint;


    [Header("Arousal Patern")]
    [SerializeField] private BossEyePattern eyeAttack;
    [SerializeField] private ThunderEffect thunderEffect;

    [Header("FallDown Patern")]
    [SerializeField] private BossFallDownPatern fallDownAttack;

    [Header("Suck Patern")]
    [SerializeField] private BossSuckPatern suckAttack;


    private void Awake()
    {
        legAttack = patern2.GetComponent<BossPatern2>();
        InstantiateFixedLeg();
        if (anim != null) GetComponent<Animator>();
        canAttack = false;
        CanAttack();
    }
    //���� ���
    private void Update()
    {
        if (canAttack)
        {
            Patern();
            StartCoroutine(CanAttack());
        }
        //�� �ٸ� ��� Hp�� 0���� ���� ��� �׷α� ����
        if (fLeg1== null && fLeg2== null && page == 1&&isGroggy==false) //-------------------------------------------------------------------
        {
            Debug.Log("Groggy");
            StartCoroutine(Groggy());
        }


    }
    //���� ����
    private void Patern()
    {
        if(page==1)
        {
            attackPatern = Random.Range(1, 4);//1~3���� ����
            switch (attackPatern)
            {
                case 1:
                case 2:
                    legAttack.Attack();
                    break;
                case 3:
                    fallDownAttack.Attack();
                    break;
                default:
                    break;

            }
        }
        else if(page==2) 
        {
            if(attackStack==2)
            {
                attackPatern = Random.Range(0, 3);//0~2���� ����
                attackStack = 0;
                if (eyeattack) attackPatern = 2;//���� ���� �߿��� ����ġ��� �⺻���ݸ� ����

                switch(attackPatern)
                {
                    case 0:
                        //���� ����
                        eyeattack = true;
                        eyeAttack.SpawnEye(this.gameObject);
                        StartCoroutine(ReinforceAttack());
                        break;
                        
                    case 1:
                        anim.SetBool("IsSuck", true);
                        suckAttack.SuckAttack();
                        anim.SetBool("IsSuck", false);
                        break;

                    case 2:
                        fallDownAttack.Attack();
                        break;
                }
            }
            else//a-a-b
            {
                legAttack.Attack();
                attackStack++;
            }
        }

    }
    //�ǰ�
    public virtual void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 60 && page == 1) 
        {
            Debug.Log("Page 2");
            headCollider.enabled = true;
            page = 2;
        }
        if (hp <= 0)
        {
            Debug.Log("Boss die");


            //ȿ����
            SoundManager.Instance.PlaySFX(23);

            Destroy(this.gameObject);

        }
           
    }
    //���� ��
    private IEnumerator CanAttack()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackSpeed);
        canAttack = true;
    }

    public IEnumerator Groggy()//�� ���� �ٸ��� ������� ��� �̰� �����// �����ٸ� �κп��� �̰� ������Ѿߵ�
    {

        canAttack = false;
        isGroggy = true;
        headCollider.enabled = true;
        Debug.LogError("Start Groggy");
        anim.SetBool("IsGroggy",true);
        yield return new WaitForSeconds(groggyTime);
        headCollider.enabled = false;
        canAttack = true;
        isGroggy = false;
        anim.SetBool("IsGroggy", false);
        Debug.LogError("End Groggy");
        if (hp >= 70) 
        {
            InstantiateFixedLeg();
        }
    }

    private void InstantiateFixedLeg()
    {
        int randomNum1 = Random.Range(0, leftFixedLegSpawnPoint.Count);
        int randomNum2 = Random.Range(0, RightFixedLegSpawnPoint.Count);
        Debug.Log(randomNum1 + " " + randomNum2);
        fixedLeg1 = Instantiate(FixedLegPrefab, leftFixedLegSpawnPoint[randomNum1], Quaternion.identity);
        fixedLeg2 = Instantiate(FixedLegPrefab, RightFixedLegSpawnPoint[randomNum2], Quaternion.identity);

        fLeg1 = fixedLeg1.GetComponent<FixedLeg>();
        fLeg2 = fixedLeg2.GetComponent<FixedLeg>();
        fLeg1.SetParent(this);
        fLeg2.SetParent(this);
    }

    public IEnumerator ReinforceAttack()
    {
        Debug.Log("���� ���� ��ȭ");

        float prevAtkSpeed = attackSpeed;

        anim.SetBool("IsArousal", true);
        thunderEffect.TriggerThunderOn();
        attackSpeed = 2f;

        yield return new WaitForSeconds(prevAtkSpeed);

        anim.SetBool("IsArousal",false);
        thunderEffect.TriggerThunderOff();
        attackSpeed = prevAtkSpeed;
    }

    public float GetAtkSpeed()
    {
        return attackSpeed;
    }
    public Transform GetTransform()
    {
        return transform;
    }
    public void DieLeg(FixedLeg fixedLeg)
    {
        if(fLeg1==fixedLeg)
        {
            fLeg1=null;
            fixedLeg1=null;
            Debug.Log("leg1 die");
        }
        else
        {
            fLeg2=null;
            fixedLeg2=null;
            Debug.Log("leg2 die");
        }
    }
}
