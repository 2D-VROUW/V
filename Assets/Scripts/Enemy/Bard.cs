using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class Bard : EnemyMove
{
    public GameObject attackRange;
    public GameObject buffPrefab;    // ���� ����

    private CircleCollider2D rangeCollider;

    [SerializeField] float Attackspeed = 10f;
    [SerializeField] private float increaseRate = 1f;  // ��ġ ���� �ӵ�
    [SerializeField] private float decreaseRate = 0.5f; // ��ġ ���� �ӵ�

    private float chargeTime = 0;
    private bool isPlayerDetected = false;
    private bool Wait = true;
    private bool isChargingAnimPlayed = false;


    //�νĹ��� �ޱ�
    void Start()
    {
        rangeCollider = attackRange.GetComponent<CircleCollider2D>();
        rangeCollider.isTrigger = true;
    }
    private void Update()
    {
        //���� ���� ����
        if ( chargeTime >= 3f)
        {
            Music();
        }
        if (chargeTime >= 2.2f && !isChargingAnimPlayed)
        {
            anim.SetTrigger("isCharging");
            isChargingAnimPlayed = true; // �ִϸ��̼� �����
        }
        //���� ġ�� �ð�
        if (isPlayerDetected)
        {
            if (chargeTime < 3 && Wait)
                chargeTime += increaseRate * Time.deltaTime; // ���� �ð����� ��ġ ����
        }
        else
        {
            if (chargeTime > 0)
                chargeTime -= decreaseRate * Time.deltaTime; // ���� �ð����� ��ġ ����
        }
    }
    //�÷��̾� ����
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            speed = 0;
            isPlayerDetected = true;
        }
    }
    //�÷��̾� ��ħ
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            speed = 2.5f;
            Debug.Log("�÷��̾� ��ħ");
            isPlayerDetected = false;
        }
    }
    //���� ��
    private IEnumerator WaitAttack()
    {
        Wait = false;
        yield return new WaitForSeconds(Attackspeed);
        Wait = true;
    }
    //���� �ο�
    private void Music()
    {
        StartCoroutine("WaitAttack");
        Debug.Log("���� ����");
        chargeTime = 0;
        GameObject currentBuff = Instantiate(buffPrefab, transform.position, Quaternion.identity);
        Destroy(currentBuff, 1f);
        isChargingAnimPlayed = false; // �ִϸ��̼� �����
    }
}