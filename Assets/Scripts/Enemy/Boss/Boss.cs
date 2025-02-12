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

    private float groggyTime = 3f;
    bool isGroggy=false;


    [Header("Leg Patern")]
    public GameObject patern2;//Leg Patern
    BossPatern2 legAttack;


    [Header("Fixed Leg")]
    [SerializeField] private GameObject FixedLegPrefab;
    [SerializeField] private GameObject fixedLeg1;
    [SerializeField] private GameObject fixedLeg2;
    [SerializeField] private FixedLeg fLeg1;
    [SerializeField] private FixedLeg fLeg2;//--------------------------------------------------------------------------�����ٸ� ��ũ��Ʈ
    [SerializeField] List<Vector2> leftFixedLegSpawnPoint;
    [SerializeField] List<Vector2> RightFixedLegSpawnPoint;

    [Header("Head Patern")]
    [SerializeField] private GameObject bossHead;

    [Header("Eye Patern")]
    [SerializeField] private BossEyePattern eyeAttack;


    private void Awake()
    {
        legAttack = patern2.GetComponent<BossPatern2>();
        InstantiateFixedLeg();
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
        attackPatern = 2;//Random.Range(1, 5);
        if(page==1)
        {
            attackPatern = Random.Range(1, 4);//1~3���� ����
            switch (attackPatern)
            {
                case 1://�����ٸ� �����ε� 
                case 2://�Ϲ� �ٸ� ����//case1�� 2�� �� �Ϲ� �ٸ� ������ ������ ���� �߽��ϴ�.
                    legAttack.Attack();
                    break;
                case 3://���ǻ� 5��° ����ġ�� ������ 3������ ����
                    //B5.Attack();
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
                        eyeAttack.SpawnEye(this.gameObject);
                        StartCoroutine(ReinforceAttack());
                        break;
                        
                    case 1:
                        //���� ����
                        break;

                    case 2:
                        //����ġ��
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
        /*if (page == 2)//���� �޴� ��� 1. �׷α� ������ �� 2. 2�������� �� �÷��̾��� ������ collider�� ������ ��
            //bossHead�� collider�� �ھƳ��� �׷α� ���� �Ǵ� 2������ �϶� setActive�� �ؼ� �ϰ� ����
        {
            hp -= damage;
            if (hp <= 0)
                Destroy(this.gameObject);
        }*/
        hp -= damage;
        if (hp <= 60 && page == 1) 
        {
            Debug.Log("Page 2");
            page = 2;
            bossHead.SetActive(true);
        }
        if (hp <= 0)
        {
            Debug.Log("Boss die");
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
        bossHead.SetActive(true);//�ӽ÷� setActive�س��� ���� �׷α� ��� ���;���
        canAttack = false;
        isGroggy = true;
        Debug.LogError("Start Groggy");

        yield return new WaitForSeconds(groggyTime);

        bossHead.SetActive(false);
        canAttack = true;
        isGroggy = false;
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
        attackSpeed = 3f;
        yield return new WaitForSeconds(prevAtkSpeed);
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
