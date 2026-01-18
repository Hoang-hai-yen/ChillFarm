using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource; 
    public AudioSource sfxSource; 

    [Header("BGM Clips")]
    public AudioClip loginMusic;  
    public AudioClip gameMusic;  

    [Header("SFX Clips")]
    public AudioClip digPlant;
    public AudioClip fishing;
    public AudioClip upgrade;
    public AudioClip dialogVoice;
    public AudioClip click;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void PlayLoginBGM()
    {
        PlayMusic(loginMusic);
    }

    public void PlayGameBGM()
    {
        PlayMusic(gameMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (bgmSource == null) 
        {
            Debug.LogError("LỖI: Chưa gắn Bgm Source vào AudioManager!");
            return;
        }
        if (clip == null)
        {
            Debug.LogError("LỖI: Chưa kéo file nhạc vào ô Music!");
            return;
        }

        Debug.Log("Đang cố phát nhạc: " + clip.name);

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    // ================== SFX LOGIC (Giữ nguyên) ==================
    public void PlayDigPlant()
    {
        if(sfxSource != null) sfxSource.PlayOneShot(digPlant);
    }

    public void PlayFishing()
    {
        if (sfxSource != null) sfxSource.PlayOneShot(fishing);
    }

    public void PlayUpgrade()
    {
        if (sfxSource != null) sfxSource.PlayOneShot(upgrade);
    }

    public void PlayDialogVoice()
    {
        if (sfxSource == null) return;
        sfxSource.clip = dialogVoice;
        sfxSource.loop = true;
        sfxSource.Play();
    }

    public void StopDialogVoice()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
        }
    }

    public void PlayClick()
    {
        if (sfxSource != null) sfxSource.PlayOneShot(click);
    }
}