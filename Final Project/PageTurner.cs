using UnityEngine;
using UnityEngine.UI;
using TMPro; // 新增TextMeshPro命名空间

public class PageTurner : MonoBehaviour
{
    // 替换为TextMeshProUGUI类型
    public TextMeshProUGUI textPage1;
    public TextMeshProUGUI textPage2;
    public TextMeshProUGUI textPage3;

    private int currentPage = 1;

    void Start()
    {
        // 初始只显示第一页文字
        textPage1.gameObject.SetActive(true);
        textPage2.gameObject.SetActive(false);
        textPage3.gameObject.SetActive(false);
    }

    // 点击文字框翻页方法
    public void TurnPage()
    {
        switch (currentPage)
        {
            case 1:
                textPage1.gameObject.SetActive(false);
                textPage2.gameObject.SetActive(true);
                currentPage = 2;
                break;
            case 2:
                textPage2.gameObject.SetActive(false);
                textPage3.gameObject.SetActive(true);
                currentPage = 3;
                break;
            case 3:
                // 第三页点击后逻辑：隐藏所有文字/返回主界面等
                textPage3.gameObject.SetActive(false);
                // 如需返回主界面，添加：SceneManager.LoadScene("主界面场景名");
                // 记得顶部添加 using UnityEngine.SceneManagement;
                break;
        }
    }
}