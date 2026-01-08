using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BtnShowImage : MonoBehaviour
{
    // 点击后要显示的图片对象
    public Image targetShowImage;
    // 要显示的图片素材
    public Sprite showSprite;
    // 要切换的下一个场景名称
    public string nextSceneName;

    private Button showButton;

    void Start()
    {
        // 获取按钮组件并绑定显示图片事件
        showButton = GetComponent<Button>();
        showButton.onClick.AddListener(ShowTargetImage);

        // 给显示的图片添加点击切换场景事件
        Button imageButton = targetShowImage.GetComponent<Button>();
        imageButton.onClick.AddListener(SwitchToNextScene);
    }

    // 显示图片的方法
    private void ShowTargetImage()
    {
        targetShowImage.sprite = showSprite;
        targetShowImage.gameObject.SetActive(true);
    }

    // 切换到下一个场景的方法
    private void SwitchToNextScene()
    {
        // 检查场景名称是否为空
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("请在Inspector面板设置nextSceneName！");
        }
    }

    // 可选：切换显示/隐藏图片的方法（替换ShowTargetImage用）
    private void ToggleTargetImage()
    {
        targetShowImage.sprite = showSprite;
        targetShowImage.gameObject.SetActive(!targetShowImage.gameObject.activeSelf);
    }
}