using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatern2 : MonoBehaviour
{
    public GameObject leg;
    private GameObject legAttack;

    [SerializeField] private float speed = 40f; 

    private int legWhere;
    private int currentLegWhere = 0;

    private float randomX;
    private float randomY;
    private bool move;

    
    public void Attack()
    {
        Debug.Log("∆–≈œ2");
        legWhere = Random.Range(1, 6);
        switch (legWhere)
        {
            case 1:
                randomX = -22f;
                randomY = 3f;
                move = true;
                break;
            case 2:
                randomX = -22f;
                randomY = -2f;
                move = true;
                break;
            case 3:
                randomX = -22f;
                randomY = -7f;
                move = true;
                break;
            case 4:
                randomX = 22f;
                randomY = 3f;
                move = false;
                break;
            case 5:
                randomX = 22f;
                randomY = -2f;
                move = false;
                break;
            case 6:
                randomX = 22f;
                randomY = -7f;
                move = false;
                break;
        }
        currentLegWhere = legWhere;
        Vector2 SpawnPos = new Vector2(randomX, randomY);
        legAttack = Instantiate(leg, SpawnPos, transform.rotation);
        Destroy(legAttack, 1f);
    }
    void Update()
    {
        if (move)
        {
            legAttack.transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else
        {
            legAttack.transform.Translate(speed * Time.deltaTime * -Vector3.right);
        }
    }
}
