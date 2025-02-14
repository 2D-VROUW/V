using System.Collections;
using UnityEngine;

public class BossSuckPatern : MonoBehaviour
{
    [SerializeField] public float pullForce = 25f; // ���Ƶ��̴� ��
    [SerializeField] public float suckTime = 4.5f;
    [SerializeField] CircleCollider2D Range;
    [SerializeField] CircleCollider2D DieArea;
    bool isSuck = false;
    private void Start()
    {
        Range.enabled=false;
        DieArea.enabled=false;
    }
    public void SuckAttack()
    {
        StartCoroutine(Suck());
    }

    IEnumerator Suck()
    {
        isSuck = true;
        Range.enabled = true;
        DieArea.enabled = true;
        yield return new WaitForSeconds(suckTime);
        Range.enabled = false;
        DieArea.enabled=false;
        isSuck = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
        if (playerRb != null&&isSuck)
        {
            // ���� �߽ɰ� �÷��̾� ��ġ
            Vector2 direction = (transform.position - collision.transform.position).normalized;

            // �� ����
            playerRb.velocity += (direction * pullForce * Time.deltaTime);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player")&&isSuck)
        {
            PlayerHP playerScript = collision.gameObject.GetComponent<PlayerHP>();
            if (playerScript != null )
            {
                playerScript.TakeDamage(100, transform.position); // float�� int�� ��ȯ
                Debug.Log("Suck");
            }
        }
    }

}