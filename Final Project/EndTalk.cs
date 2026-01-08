using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimpleJumpToLetter : MonoBehaviour
{
    private Button jumpButton;

    void Start()
    {
        // 获取按钮组件
        jumpButton = GetComponent<Button>();
        if (jumpButton != null)
        {
            // 绑定点击事件，直接跳转到Letter场景
            jumpButton.onClick.AddListener(JumpToLetterScene);
        }
        else
        {
            Debug.LogError("当前对象没有Button组件！");
        }
    }

    /// <summary>
    /// 直接跳转Letter场景
    /// </summary>
    private void JumpToLetterScene()
    {
        // 直接加载场景名（与Build Settings中一致即可）
        SceneManager.LoadScene("Letter");
        // 若想通过场景索引跳转，先看Build Settings中Letter的索引（比如是5），则写：
        // SceneManager.LoadScene(5);
    }
}