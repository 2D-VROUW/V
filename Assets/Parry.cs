using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour
{
    public GameObject parring; // �и� ����Ʈ ������Ʈ (�ʿ� �� ���)
    private bool whereParry;
    private Animator anim;

    private void Start()
    {
        anim = parring.GetComponent<Animator>();  // �÷��̾��� Animator ������Ʈ ��������
    }

    void Update()
    {
        if (Input.GetKey(KeySetting.Keys[KeyAction.LEFT])) // ���� �̵�
        {
            whereParry = true;
        }
        else if (Input.GetKey(KeySetting.Keys[KeyAction.RIGHT])) // ������ �̵�
        {
            whereParry = false;
        }
    }

    public void ParrySuccess()
    {
        Debug.Log("�и��ִ�");
        if (whereParry)
        {
            anim.SetTrigger("Parry2"); // "Parry" �ִϸ��̼� ����
        }
        else
        {
            anim.SetTrigger("Parry"); // "Parry" �ִϸ��̼� ����
        }
    }
}