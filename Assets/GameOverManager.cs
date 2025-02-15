using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject arrowMark;

    private void Start()
    {
        SoundManager.Instance.StopSFX();
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySFX(4);

        arrowMark.SetActive(false);
    }

    public void Redo()
    {
        SaveLoad.currentSelectedSlot = SavePoint.diePoint - 1;
        // ����� ������ ����
        if (!string.IsNullOrEmpty(PlayerHP.lastSceneName))
        {
            SceneManager.LoadScene(PlayerHP.lastSceneName);
        }
        else
        {
            SceneManager.LoadScene("LobbyScene"); // �⺻��
        }
    }

    public void ArrowMark()
    {
        arrowMark.SetActive(true);
    }

    public void NoArrowMark()
    {
        arrowMark.SetActive(false);
    }
}
