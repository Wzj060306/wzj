using UnityEngine;
using UnityEngine.UI;

public class ToggleSmallImage : MonoBehaviour
{
    // 要显示/隐藏的小图片对象
    public Image targetSmallImage;
    // 小图片的素材
    public Sprite smallSprite;
    // 按钮组件
    private Button toggleButton;

    void Start()
    {
        // 获取按钮组件
        toggleButton = GetComponent<Button>();
        // 绑定点击事件
        toggleButton.onClick.AddListener(ToggleImage);
    }

    // 切换图片显示/隐藏的方法
    private void ToggleImage()
    {
        // 给图片赋值（首次点击时）
        if (targetSmallImage.sprite == null)
        {
            targetSmallImage.sprite = smallSprite;
        }
        // 切换激活状态
        targetSmallImage.gameObject.SetActive(!targetSmallImage.gameObject.activeSelf);
    }
}