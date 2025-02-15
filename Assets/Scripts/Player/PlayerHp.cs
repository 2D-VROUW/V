using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHP : MonoBehaviour
{
    public static string lastSceneName = "";
    public GameObject heartPrefab;  // ��Ʈ ������
    public GameObject diePanel; // ��� �г�
    public int maxHP = 30;  // �ִ� ü��
    public int currentHP = 30;  // ���� ü��
    public List<GameObject> heartObjects = new List<GameObject>();  // ��Ʈ GameObject ����Ʈ

    private bool isInvincible = false;  // ���� ���� ����
    public float invincibilityDuration = 1f;  // ���� ���� ���� �ð�
    public Rigidbody2D rb;
    public PlayerMove pm;

    void Start()
    {
        CreateHearts();  // ���� ���� �� ��Ʈ ��ü ����
        UpdateHearts();  // �ʱ� ��Ʈ �̹��� ������Ʈ
        rb=gameObject.GetComponent<Rigidbody2D>();
        if (pm==null) pm=gameObject.GetComponent<PlayerMove>();
    }

    // ��Ʈ �������� �̿��� ��Ʈ ��ü ����
    void CreateHearts()
    {
        for (int i = 0; i < maxHP; i++)
        {
            //GameObject heart = Instantiate(heartPrefab, transform); // �������� �ν��Ͻ�ȭ�Ͽ� �θ�� ����
            //heartObjects.Add(heart);  // ��Ʈ ��ü ����Ʈ�� �߰�
        }
    }

    // ü�� ���� ó��
    public void TakeDamage(int damage, Vector2 targetpos)
    {
        Debug.Log("����");
        // ���� ������ ��� ������ ��ȿȭ
        if (isInvincible) return;
        if (pm.GetParrying())
        {
            StartCoroutine(pm.ParryingSuccess());
            return;
        }
        return;
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
        // �˹� ���� ��� (��ǥ�� ���� ��ġ�� ����)
        Vector2 knockbackDirection = ((Vector2)transform.position - targetpos).normalized;

        // �˹� ���� ���� (���� �����Ͽ� �˹��� ������ ����)
        float knockbackStrength = 7f;

        // ���� Rigidbody2D�� velocity�� �ݴ� �������� �˹� ����
        rb.velocity += knockbackDirection * knockbackStrength;


        // ��Ʈ ������Ʈ
        UpdateHearts();

        // ���� ���� ����
        StartCoroutine(InvincibilityCoroutine());
    }

    // ��Ʈ ���� ������Ʈ
    public void UpdateHearts()
    {
        for (int i = 0; i < heartObjects.Count; i++)
        {
            // ���� ü�¿� �´� ��Ʈ�� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
            heartObjects[i].SetActive(i < currentHP);
        }
        if (currentHP <= 1)
        {
            SoundManager.Instance.PlaySFX(1);
        }
    }

    // �÷��̾� ��� ó��
    public void Die()
    {
        Debug.Log("�÷��̾� ���");
        lastSceneName = SceneManager.GetActiveScene().name;
        if (diePanel != null)
        {
            DiePanel panelScript = diePanel.GetComponent<DiePanel>();
            if (panelScript != null)
            {
                panelScript.Bravo6(); // �ִϸ��̼� ����
            }
            else
            {
                Debug.LogWarning("diePanel�� DiePanel ��ũ��Ʈ�� ����.");
            }
        }
        else
        {
            Debug.LogWarning("diePanel�� �Ҵ���� �ʾ���.");
        }

        Invoke("LoadGameOverScene", 0.5f);
    }

    // ���� ���� �� �ε�
    private void LoadGameOverScene()
    {
        SceneManager.LoadScene("Gameover");
    }

    // ���� ���� �ڷ�ƾ (������ �������ٰ� ���� ������ ����)
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color; // ���� ���� ����
            Color targetColor = new Color(1f, 0.2f, 0.2f, originalColor.a); // ������ (R�� ����)

            float elapsedTime = 0f;
            float duration = invincibilityDuration / 3f; // �� ��ȯ �ӵ� ����

            // ������ ���������� ��ȭ
            while (elapsedTime < duration)
            {
                spriteRenderer.color = Color.Lerp(originalColor, targetColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            spriteRenderer.color = targetColor; // ���� ������ ����

            elapsedTime = 0f;

            // ������ ���� ������ ����
            while (elapsedTime < duration)
            {
                spriteRenderer.color = Color.Lerp(targetColor, originalColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            spriteRenderer.color = originalColor; // ���� ���� ���� ����
        }

        yield return new WaitForSeconds(invincibilityDuration / 3f); // ���� ���� �ð� ����
        isInvincible = false;
    }
}