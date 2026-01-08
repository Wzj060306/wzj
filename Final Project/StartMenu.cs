using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SafeStartMenu : MonoBehaviour
{
    public Button startBtn; // 关联开始按钮
    public string targetSceneName = "SampleScene"; // 目标游戏场景名称

    private ColorBlock originalColorBlock; // 原始颜色块
    private bool hasBeenClicked = false; // 是否已被点击过

    void Start()
    {
        // 第一步：检测按钮是否绑定
        if (startBtn == null)
        {
            Debug.LogError("【检测失败】请在Inspector面板为SafeStartMenu脚本绑定Start按钮！");
            return;
        }

        // 保存原始的ColorBlock（包含Normal、Hover等状态的颜色）
        originalColorBlock = startBtn.colors;

        // 第二步：绑定按钮点击事件
        startBtn.onClick.AddListener(TryLoadScene);

        // 第三步：提前检测场景是否在Build Settings中
        if (!IsSceneInBuildSettings(targetSceneName))
        {
            Debug.LogWarning($"【检测提醒】场景{targetSceneName}未添加到Build Settings，点击按钮将无法加载！");
        }
        else
        {
            Debug.Log($"【检测成功】场景{targetSceneName}已在Build Settings中，可正常加载");
        }
    }

    // 尝试加载场景的方法（包含检测逻辑）
    void TryLoadScene()
    {
        // 只有在点击时才从Normal状态变为Hover状态，且不会自动重置
        if (!hasBeenClicked)
        {
            ColorBlock newColorBlock = startBtn.colors;
            // 将Normal颜色设置为Hover（Highlighted）颜色，使按钮保持Hover状态
            newColorBlock.normalColor = originalColorBlock.highlightedColor;
            startBtn.colors = newColorBlock;
            hasBeenClicked = true;
            Debug.Log("按钮状态已从Normal变为Hover，不会自动重置");
        }

        // 再次校验场景是否存在
        if (IsSceneInBuildSettings(targetSceneName))
        {
            
            Debug.Log($"开始加载场景：{targetSceneName}");
        }
        else
        {
            Debug.LogError($"【加载失败】场景{targetSceneName}未添加到Build Settings，请先添加后重试！");
        }
    }

    // 核心检测方法：判断场景是否在Build Settings中
    bool IsSceneInBuildSettings(string sceneName)
    {
        // 遍历Build Settings中的所有场景
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            int lastSlash = path.LastIndexOf('/');
            string fileName = path.Substring(lastSlash + 1);
            int dotIndex = fileName.LastIndexOf('.');
            string sceneFileName = fileName.Substring(0, dotIndex);

            // 匹配场景名称
            if (sceneFileName == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}