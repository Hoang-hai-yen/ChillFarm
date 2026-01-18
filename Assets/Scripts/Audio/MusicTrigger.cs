using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [Header("Chọn loại nhạc muốn phát")]
    public bool playLoginMusic = false;
    public bool playGameMusic = false;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            if (playLoginMusic)
            {
                AudioManager.Instance.PlayLoginBGM();
                Debug.Log("MusicTrigger: Đã yêu cầu phát nhạc Login");
            }
            else if (playGameMusic)
            {
                AudioManager.Instance.PlayGameBGM();
                Debug.Log("MusicTrigger: Đã yêu cầu phát nhạc Game");
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy AudioManager! Hãy chắc chắn bạn đã chạy từ Scene Login.");
        }
    }
}