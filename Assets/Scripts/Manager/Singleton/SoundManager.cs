using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bk_source; // ������� ����� �ҽ�
    public AudioSource ef_source; // ȿ���� ����� �ҽ�

    [Header("Audio Clips")]
    [SerializeField] public List<AudioClip> bk_music; // BGM ����Ʈ
    [SerializeField] public List<AudioClip> ef_music; // ȿ���� ����Ʈ

    [Header("Volume Controls")]
    public Slider bkSlider;
    public Slider efSlider;

    [Header("Default Settings")]
    [SerializeField] private int defaultBGMIndex = 0; // ������ BGM �ε���

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ���� AudioSource �������� (�ʿ��ϸ� �߰�)
        if (bk_source == null) bk_source = gameObject.AddComponent<AudioSource>();
        if (ef_source == null) ef_source = gameObject.AddComponent<AudioSource>();

        bk_source.loop = true; // BGM �ݺ� ���

        // ������ �� �ڵ� ����� BGM
        if (bk_music.Count > 0 && defaultBGMIndex >= 0 && defaultBGMIndex < bk_music.Count)
        {
            PlayBGM(defaultBGMIndex);
        }
    }

    // ��� ���� ��� �Լ�
    public void PlayBGM(int index)
    {
        if (index >= 0 && index < bk_music.Count)
        {
            bk_source.clip = bk_music[index];
            bk_source.Play();
        }
        else
        {
            Debug.LogError($"[SoundManager] BGM Index {index} is out of range!");
        }
    }

    // ȿ���� ��� �Լ� (PlayOneShot ���)
    public void PlaySFX(int index)
    {
        if (index >= 0 && index < ef_music.Count)
        {
            ef_source.PlayOneShot(ef_music[index]);
        }
        else
        {
            Debug.LogError($"[SoundManager] SFX Index {index} is out of range!");
        }
    }
    public void StopBGM()
    {
        bk_source.Stop();  // BGM ����
    }
    public void StopSFX()
    {
        ef_source.Stop();  // BGM ����
    }

    // BGM ���� ���� (�����̴� ����)
    public void SetBGMVolume()
    {
        if (bkSlider != null)
        {
            bk_source.volume = bkSlider.value;
        }
    }

    // ȿ���� ���� ���� (�����̴� ����)
    public void SetSFXVolume()
    {
        if (efSlider != null)
        {
            ef_source.volume = efSlider.value;
        }
    }
}
