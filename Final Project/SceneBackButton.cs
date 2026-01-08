using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneBackButton : MonoBehaviour
{
    // 拖拽赋值：返回按钮
    public Button backToCharacterButton;
    // 目标场景名（填写CharacterScene的实际名称）
    public string characterSceneName = "CharacterScene";

    void Start()
    {
        // 绑定按钮点击事件
        backToCharacterButton.onClick.AddListener(LoadCharacterScene);
    }

    // 加载Character场景
    void LoadCharacterScene()
    {
        // 同步加载场景（如需异步可改用SceneManager.LoadSceneAsync）
        SceneManager.LoadScene(characterSceneName);
    }
}