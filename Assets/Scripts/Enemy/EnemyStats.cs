using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyStats : MonoBehaviour
{
    public float curHp;
    public float maxHp=3f;
    public float damage=1f;
    public float knockbackForce = 0.3f;
    public Rigidbody2D rigid;
    protected bool isDie=false;
    virtual protected void Awake()
    {
        maxHp = 3f;
        curHp = maxHp;
        damage = 1f;
        rigid = GetComponent<Rigidbody2D>();
    }
    virtual public void TakeDamage(float damage,Transform player)
    {
        if (isDie) return;
        
        
        if (this.gameObject.transform.rotation.y == 0)//���� ������ ���� ���� ��
        {
            if (player.transform.position.x < gameObject.transform.position.x)//�÷��̾ ���� ���ʿ� ���� ��� 
            {
                Debug.Log("Enemy hit" + damage);
                curHp -= damage;
            }
            else
            {
                Debug.Log("BackAttack!! Enemy hit" + (damage+1));
                curHp -= damage + 1;
            }
        }
        else //���� �������� �������� �� 
        {
            if (player.transform.position.x < gameObject.transform.position.x)//�÷��̾ ���� ���ʿ� ���� ��� 
            {
                curHp -= damage + 1;//�����
                Debug.Log("BackAttack!! Enemy hit" + (damage + 1));
            }
            else
            {
                Debug.Log("Enemy hit" + damage);
                curHp -= damage ;
            }
        }

        Vector2 targetPos = new Vector2(player.transform.position.x, transform.position.y);

        // �˹� ���� ��� (targetPos���� ���� ��ġ�� ���� ���Ͱ� ����)
        Vector2 knockbackDirection = ((Vector2)transform.position - targetPos).normalized;
        Debug.Log((Vector3)knockbackDirection * knockbackForce);
        // �˹��� ������ ���ο� ��ġ�� �̵�
        transform.position += (Vector3)knockbackDirection * knockbackForce;

        CheckHp();
    }

    virtual protected void CheckHp()
    {
        if (curHp <= 0)
        {
            isDie = true;

            gameObject.GetComponent<EnemyBehavior>().StopCor();
            this.StopAllCoroutines();

            StartCoroutine(FadeOutAndDestroy()); // �߰�: ������ ��������� ��

            Destroy(this.gameObject, 0.5f); // ���� �ڵ� ����
        }
    }
    private IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 0.5f; // ����ȭ �ӵ� (���� ���� Ÿ�ְ̹� �����ϰ� ����)
        float elapsedTime = 0f;
        SpriteRenderer render = GetComponent<SpriteRenderer>();

        if (render == null) yield break; // ���� ó��: �������� ������ �ߴ�

        Color originalColor = render.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            render.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckHp();
        }
    }
    protected bool IsDie()//use different script
    {
        return isDie;
    }
}
