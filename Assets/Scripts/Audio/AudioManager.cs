using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Background Music")]
    public AudioSource bgmSource;

    [Header("Sound Effects")]
    public AudioSource sfxSource;

    public AudioClip backgroundMusic;

    [Header("SFX Clips")]
    public AudioClip digPlant;
    public AudioClip fishing;
    public AudioClip upgrade;
    public AudioClip dialogVoice;
    //public AudioClip click;

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

    void Start()
    {
    }

    // ================== BGM ==================
    public void PlayBGM()
    {
        if (bgmSource == null || backgroundMusic == null) return;

        bgmSource.clip = backgroundMusic;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // ================== SFX ==================
    public void PlayDigPlant()
    {
        sfxSource.PlayOneShot(digPlant);
    }

    public void PlayFishing()
    {
        sfxSource.PlayOneShot(fishing);
    }

    public void PlayUpgrade()
    {
        sfxSource.PlayOneShot(upgrade);
    }

    public void PlayDialogVoice()
    {
        sfxSource.clip = dialogVoice;
        sfxSource.loop = true;
        sfxSource.Play();
    }
    public void StopDialogVoice()
    {
        sfxSource.Stop();
        sfxSource.loop = false;
    }

    //public void PlayClick()
    //{
    //    sfxSource.PlayOneShot(click);
    //}
}
