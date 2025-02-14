using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour
{
    public GameObject player;  // �÷��̾� ������Ʈ
    public GameObject parring; // �и� ����Ʈ ������Ʈ (�ʿ� �� ���)

    private Animator anim;

    private void Start()
    {
        anim = player.GetComponent<Animator>();  // �÷��̾��� Animator ������Ʈ ��������
    }

    void Update()
    {
        // �и� ���� �� �ִϸ��̼� ����
        if (Input.GetKeyDown(KeyCode.P)) // ����: P Ű�� ������ �и� ����
        {
            ParrySuccess();
        }
    }

    void ParrySuccess()
    {
        anim.SetTrigger("Parry"); // "Parry" �ִϸ��̼� ����
    }
}