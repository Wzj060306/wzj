using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JumpToEndSummary : MonoBehaviour
{
    private Button jumpButton;
    // 固定指向Build Settings中的End_Summary场景
    private readonly string targetScene = "End_Summary";

    void Start()
    {
        jumpButton = GetComponent<Button>();
        jumpButton.onClick.AddListener(JumpToScene);
    }

    // 同步跳转场景（适合小场景快速加载）
    private void JumpToScene()
    {
        if (SceneManager.GetSceneByName(targetScene) == null)
        {
            Debug.LogError($"场景{targetScene}未添加到Build Settings！");
            return;
        }
        SceneManager.LoadScene(targetScene);
    }

    // 可选：异步跳转（带加载状态，适合大场景）
    private async void JumpToSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        // 阻止场景加载完成后自动切换
        asyncLoad.allowSceneActivation = false;

        // 模拟加载等待（可结合进度条显示）
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"加载进度：{progress * 100:F1}%");

            // 进度100%时允许切换场景
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            await System.Threading.Tasks.Task.Yield();
        }
    }
}