using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float speed = 2f; // �̵� �ӵ�

    void Update()
    {
        // ������ �������� �̵�
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}
