using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleEnemy : BasicEnemy
{
    public GameObject obstacle;
    public Vector3 spawnPoint;
    public float coolTime=4f;

    private void Awake()
    {
        // SpriteRenderer ������Ʈ ��������
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Bounds�� ���� ���� ��� ��ġ ���
            Bounds bounds = spriteRenderer.bounds;

            // ���� �߾� ��ǥ ���
            spawnPoint = new Vector3(bounds.min.x, bounds.center.y, bounds.center.z);

            // ��� ���
            Debug.Log("���� �߾� ��ġ: " + spawnPoint);
        }
        else
        {
            Debug.LogError("SpriteRenderer�� �����ϴ�!");
        }
    }
    protected override void Attack()
    {
        StartCoroutine(ThrowObstacle());
    }

    private IEnumerator ThrowObstacle()
    {
        while (true)
        {
            Instantiate(obstacle,transform.position,Quaternion.identity);
            yield return new WaitForSeconds(coolTime);
        }
    }
}
