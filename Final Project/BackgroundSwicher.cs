using UnityEngine;
using UnityEngine.UI;

public class BackgroundSwitcher : MonoBehaviour
{
    // 在Inspector中拖入对应的对象
    public GameObject newBackground; // 拖入NewBackground
    public GameObject clickableTextBox; // 拖入ClickableTextBox

    void Start()
    {
        // 初始隐藏新背景和文本框
        newBackground.SetActive(false);
        clickableTextBox.SetActive(false);
    }

    // 绑定到Btn_Start的点击事件
    public void OnStartButtonClick()
    {
        newBackground.SetActive(true); // 显示新背景
        clickableTextBox.SetActive(true); // 激活文本框
    }
}