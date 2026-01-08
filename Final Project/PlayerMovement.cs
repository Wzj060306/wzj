using UnityEngine;
using TMPro;

// 类名与脚本文件名保持一致，避免命名冲突
public class NewBehaviourScript : MonoBehaviour
{
    // 计数变量（私有变量，避免外部误改）
    private int oxygenCount = 0;
    private int coatCount = 0;

    // TMP文本组件引用（在Inspector面板赋值）
    [Header("UI引用")]
    public TextMeshProUGUI pickUpCountText;

    // 仅保留一个Start方法，初始化UI显示
    private void Start()
    {
        UpdatePickUpText();
    }

    // 仅保留一个触发碰撞方法，处理物品拾取
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 拾取氧气瓶
        if (other.CompareTag("Oxygen"))
        {
            oxygenCount++;
            Destroy(other.gameObject);
            UpdatePickUpText();
        }
        // 拾取棉衣
        else if (other.CompareTag("Coat"))
        {
            coatCount++;
            Destroy(other.gameObject);
            UpdatePickUpText();
        }
    }

    // 更新拾取计数的UI显示方法
    private void UpdatePickUpText()
    {
        if (pickUpCountText != null)
        {
            // TMP富文本格式化，让显示更清晰
            pickUpCountText.text = $"<color=blue>氧气瓶：</color>{oxygenCount}\n<color=gray>棉衣：</color>{coatCount}";
        }
        else
        {
            Debug.LogWarning("未赋值拾取计数的TMP文本组件！");
        }
    }
}