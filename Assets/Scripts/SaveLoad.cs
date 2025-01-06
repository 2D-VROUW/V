using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour
{
    public Button[] saveSlots; // 10���� ���� ��ư �迭
    public static int savePoint = 5; // ���̺� ����Ʈ ��ġ (�����÷��̿��� ����)
    public static int currentSavePoint;
    public GameObject slotPrefab;
    public Transform SaveSlotCanvas;

    float[,] uiPos = {
        { -400f,  100f },
        { -200f, 100f },
        { 0f, 100f },
        { 200f,  100f },
        { 400f, 100f },
        { -400f,  -100f },
        { -200f, -100f },
        { 0f, -100f },
        { 200f,  -100f },
        { 400f, -100f }
    };
    void Start()
    {
        UpdateSaveSlots();
        GenerateSaveSlots(10);
    }

    // ���̺� ���� Ȱ��ȭ/��Ȱ��ȭ ���� ������Ʈ
    protected void UpdateSaveSlots()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (i < savePoint)
            {
                saveSlots[i].interactable = true; // Ȱ��ȭ
            }
            else
            {
                saveSlots[i].interactable = false; // ��Ȱ��ȭ
            }
        }
    }
    void GenerateSaveSlots(int slotCount)
    {
        saveSlots = new Button[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            Vector3 uiPosition = new Vector3(uiPos[i,0], uiPos[i, 1], 0f);
            GameObject newSlot = Instantiate(slotPrefab, uiPosition, transform.rotation); // ���� ����
            saveSlots[i] = newSlot.GetComponent<Button>();
            newSlot.transform.SetParent(SaveSlotCanvas.transform, false); // ĵ������ Transform ����
            newSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(uiPos[i, 0], uiPos[i, 1]);
            // ������ ��ư Ŭ�� �̺�Ʈ ����
            int slotIndex = i; // Ŭ���� ���� ����
            saveSlots[i].onClick.AddListener(() => OnSlotClicked(slotIndex));
        }
    }

    // ���� ���� �� ȣ�� (�ε� ��� ���� ����)
    public void OnSlotClicked(int slotIndex)
    {
        if (slotIndex < savePoint)
        {
            Debug.Log($"Slot {slotIndex + 1} ���õ�. ���̺� �����͸� �ε��մϴ�.");
            SceneManager.LoadScene("test");
        }
        else
        {
            Debug.Log("�� ������ ���� ��� �ֽ��ϴ�.");
        }
    }
}
