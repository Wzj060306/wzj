using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicController_TMP : MonoBehaviour
{
    [Header("音频资源")]
    public AudioClip bgmClip; // 拖拽你的MP3音频文件
    [Header("UI组件")]
    public Button musicButton; // 拖拽音乐按钮

    private AudioSource audioSource;
    private bool isMusicPlaying = false; // 标记音乐播放状态

    void Awake()
    {
        // 单例模式：确保场景切换音乐不中断且仅一个音乐管理器
        MusicController_TMP[] managers = FindObjectsOfType<MusicController_TMP>();
        if (managers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // 初始化音频源组件
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = bgmClip;
        audioSource.loop = true; // 背景音乐循环播放
        audioSource.volume = 0.5f; // 初始音量

        // 绑定按钮点击事件
        musicButton.onClick.AddListener(OnMusicButtonClicked);
    }

    // 按钮点击触发的核心方法
    private void OnMusicButtonClicked()
    {
        if (bgmClip == null)
        {
            Debug.LogError("未设置背景音乐文件，请为Bgm Clip字段赋值！");
            return;
        }

        if (!isMusicPlaying)
        {
            // 第一次点击：播放音乐
            audioSource.Play();
            isMusicPlaying = true;
        }
        else
        {
            // 第二次点击：停止音乐（完全停止，而非暂停）
            audioSource.Stop();
            isMusicPlaying = false;
        }
    }
}