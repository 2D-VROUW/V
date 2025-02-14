using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� �߰�

public class LastInterval : MonoBehaviour
{
    private Interval interval;
    private bool isStageClear = false;
    public GameObject diePanel;

    void Start()
    {
        interval = GetComponent<Interval>(); // Interval ������Ʈ ��������
    }

    void Update()
    {
        if (isStageClear || interval == null) return; // �ߺ� ���� ����

        if (interval.stageClear) // Interval���� stageClear Ȯ��
        {
            isStageClear = true;
            Invoke("ChangeScene", 2f); // 1�� �� �� ��ȯ
            DiePanel panelScript = diePanel.GetComponent<DiePanel>();
            if (panelScript != null)
            {
                panelScript.Bravo6(); // �ִϸ��̼� ����
            }
        }
    }

    void ChangeScene()
    {
        SaveLoad.savePointIndex = 9;
        SceneManager.LoadScene("SaveLoad"); // ���� �� �ε�
    }
}
