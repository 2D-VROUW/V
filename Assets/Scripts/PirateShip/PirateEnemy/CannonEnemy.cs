using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonEnemy : MonoBehaviour
{
    protected float Hp = 5f;
    protected float speed = 3f;

    bool arrive = false;
    protected GameObject boundary;
    public GameObject cannonField;

    private void Start()
    {
        StartCoroutine(Move());
        Attack();
    }

    public void SetBoundary(GameObject bndry)
    {
        boundary = bndry;
    }
    private void OnDestroy()
    {
        SpawnEnemy spawner = GetComponentInParent<SpawnEnemy>();
        spawner.decreaseNumOfEnemies();
    }

    protected IEnumerator Move()
    {
        while (!boundary) yield return new WaitForSeconds(0.1f);
        float stopLine = boundary.transform.position.x + transform.localScale.x / 2;
        while (!arrive)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.left);
            if (this.transform.position.x <= stopLine)
            {
                arrive = true;
                StopCoroutine(Move());
            }
            yield return new WaitForSeconds(0.002f);
        }
        Attack();
    }
    protected virtual void Attack()
    {
        if (Hp <= 0)
        {
            CancelInvoke("Attack");
            return;
        }
        Vector3 spawnPosition = new Vector3(transform.position.x - 8.5f, transform.position.y, transform.position.z);
        // 대포알 생성
        GameObject CF = Instantiate(cannonField, spawnPosition, transform.rotation);
        Debug.Log("적 : 대포 발사!");
        Destroy(CF, 2f);
        Invoke("Attack", 4f);
    }
}
