using UnityEngine;
using UnityEngine.SceneManagement;

// 必须将代码包裹在类中，且类需继承MonoBehaviour
public class EscQuit : MonoBehaviour
{
    // 场景白名单：这些场景中屏蔽Esc退出功能
    public string[] disableQuitScenes = { "GameLevel1", "GameLevel2" };

    // 单例模式，保证跨场景生效
    private static EscQuit _instance;
    public static EscQuit Instance => _instance;

    void Awake()
    {
        // 单例逻辑：确保全局唯一且场景切换不销毁
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Unity生命周期方法，每帧执行
    void Update()
    {
        // 获取当前激活的场景名称
        string currentScene = SceneManager.GetActiveScene().name;

        // 检查当前场景是否在白名单中，若是则不响应Esc
        if (System.Array.IndexOf(disableQuitScenes, currentScene) != -1)
        {
            return;
        }

        // 监听Esc键按下，触发退出
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TriggerQuit();
        }
    }

    // 退出游戏的核心方法（定义缺失的TriggerQuit）
    private void TriggerQuit()
    {
#if UNITY_EDITOR
        // 编辑器中停止游戏播放
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 打包后退出应用程序
        Application.Quit();
#endif
    }
}