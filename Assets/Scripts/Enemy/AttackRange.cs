using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    NER ner;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("�� �� �־�! ���ܰ�");

        if (collision.tag == "Player")
            ner.Invoke();
    }
}
