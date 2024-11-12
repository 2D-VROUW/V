using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NER : MonoBehaviour
{
    public GameObject bullet;
    public GameObject player;
    private Bullet bulletScript;
    private Vector3 bulletDirection;

    public void Start()
    {
        Debug.Log("플레이어 공격");
    }
}
