using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PirateManager : MonoBehaviour
{
    public GameObject diePanel;

    [Header("Hearts")]
    [SerializeField] public GameObject heart1;
    [SerializeField] public GameObject heart2;
    [SerializeField] public GameObject heart3;
    [SerializeField] public GameObject heart4;
    [SerializeField] public GameObject heart5;
    int heart = 5;

    [Header("ClearRate")]
    [SerializeField] float clearRate = 0f;
    [SerializeField] private float targetTime = 60f;
    private float ratePerFrame;
    private float targetRate = 100f;

    [Header("Player")]
    [SerializeField] private GameObject player;
    private SpriteRenderer playerSprite;

    [Header("SpawnManager")]
    [SerializeField] public SpawnEnemy spawnEnemy;
    [SerializeField] public SpawnObject spawnObject;

    private void Awake()
    {
        SoundManager.Instance.StopSFX();
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM(6);
        clearRate = 0f;
        heart = 5;
        ratePerFrame = targetRate / targetTime;
        if (spawnEnemy == null) spawnEnemy =FindObjectOfType<SpawnEnemy>();
        if (spawnObject == null) spawnObject = FindObjectOfType<SpawnObject>();
    }
    private void Start()
    {
        clearRate = 0f;
        ratePerFrame = targetRate / targetTime;

        if (player != null)
        {
            playerSprite = player.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("Player is Null");
        }
    }

    void FixedUpdate()
    {
        if (clearRate >= targetRate)
        {
            Debug.Log("clear");
            spawnObject.StopSpawn();
            spawnEnemy.StopSpawn();
            Invoke("Curtain", 8f);
        }
        else
        {
            clearRate += ratePerFrame * Time.deltaTime;
        }
    }
    void Curtain()
    {
        if (diePanel != null)
        {
            Debug.Log("diePanel 활성화됨");
            DiePanel panelScript = diePanel.GetComponent<DiePanel>();
            if (panelScript != null)
            {
                Debug.Log("Bravo6() 실행");
                panelScript.Bravo6(); // 애니메이션 실행
            }
            else
            {
                Debug.LogWarning("diePanel에 DiePanel 스크립트 없음!");
            }
        }
        else
        {
            Debug.LogWarning("diePanel이 null임!");
        }

        Invoke("ToSave", 2f);
    }

    void ToSave()
    {
        SaveLoad.savePointIndex = 10;
        SceneManager.LoadScene("SaveLoad");
    }

    public void ReStart()
    {
        clearRate = 0f;
        heart = 5;
        SceneManager.LoadScene("PirateShip");
    }

    public void UpHeart()
    {
        if (heart == 5) return;
        heart++;
        CheckHeart();
    }

    public void DownHeart()
    {
        heart--;
        CheckHeart();

        if (playerSprite != null)
        {
            StartCoroutine(BlinkEffect());
        }

        if (heart == 0) ReStart();
    }

    public int GetHeart()
    {
        return heart;
    }

    void CheckHeart()
    {
        heart1.SetActive(heart >= 1);
        heart2.SetActive(heart >= 2);
        heart3.SetActive(heart >= 3);
        heart4.SetActive(heart >= 4);
        heart5.SetActive(heart >= 5);
    }

    private IEnumerator BlinkEffect()
    {
        float blinkDuration = 1f;
        float blinkInterval = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            playerSprite.enabled = !playerSprite.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        playerSprite.enabled = true;
    }
}
