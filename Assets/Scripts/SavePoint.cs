using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private int savePos;

    public static int diePoint; // ���� ����� üũ����Ʈ ��ġ
    private bool usedSave = false; // ���� ���� ����Ʈ�� ���Ǿ����� üũ

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // �±� �� ��� ����
        {
            if (!usedSave)
            {
                SaveLoad.savePointIndex = savePos;
                diePoint = savePos; // ���� üũ����Ʈ ������Ʈ
                usedSave = true; // �ߺ� ���� ����

                // �÷��̾� ü�� ȸ��
                PlayerHP hp = collision.GetComponent<PlayerHP>();
                if (hp != null)
                {
                    hp.currentHP = hp.maxHP;
                }
                else
                {
                    Debug.LogError("PlayerHP ������Ʈ�� ã�� �� �����ϴ�.");
                }
            }
        }
    }
}
