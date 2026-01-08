using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色介绍面板互斥显示控制器
/// 功能：点击按钮显隐对应面板、面板互斥显示、收起按钮随面板显隐
/// </summary>
public class CharacterPanelController : MonoBehaviour
{
    [Header("UI元素引用")]
    [Tooltip("小孩介绍面板")]
    public GameObject panelChild;
    [Tooltip("科学家介绍面板")]
    public GameObject panelScientist;
    [Tooltip("小孩简介按钮")]
    public Button btnChild;
    [Tooltip("科学家简介按钮")]
    public Button btnScientist;
    [Tooltip("全局收起按钮")]
    public GameObject btnClose;

    // 状态标记
    private bool isPanelShowing = false; // 是否有面板正在显示
    private GameObject currentActivePanel; // 当前显示的面板

    void Start()
    {
        // 初始化UI状态
        InitUIState();
        // 绑定按钮点击事件
        BindButtonEvents();
    }

    /// <summary>
    /// 初始化UI初始状态
    /// </summary>
    private void InitUIState()
    {
        panelChild.SetActive(false);
        panelScientist.SetActive(false);
        btnClose.SetActive(false);
        // 确保按钮初始可点击
        btnChild.interactable = true;
        btnScientist.interactable = true;
    }

    /// <summary>
    /// 绑定所有按钮的点击事件
    /// </summary>
    private void BindButtonEvents()
    {
        btnChild.onClick.AddListener(ShowChildPanel);
        btnScientist.onClick.AddListener(ShowScientistPanel);
        btnClose.GetComponent<Button>().onClick.AddListener(CloseAllPanels);
    }

    /// <summary>
    /// 显示小孩介绍面板
    /// </summary>
    private void ShowChildPanel()
    {
        if (!isPanelShowing)
        {
            panelChild.SetActive(true);
            currentActivePanel = panelChild;
            isPanelShowing = true;
            btnClose.SetActive(true);
            // 禁用按钮防止重复点击
            SetButtonsInteractable(false);
        }
    }

    /// <summary>
    /// 显示科学家介绍面板
    /// </summary>
    private void ShowScientistPanel()
    {
        if (!isPanelShowing)
        {
            panelScientist.SetActive(true);
            currentActivePanel = panelScientist;
            isPanelShowing = true;
            btnClose.SetActive(true);
            // 禁用按钮防止重复点击
            SetButtonsInteractable(false);
        }
    }

    /// <summary>
    /// 收起所有面板
    /// </summary>
    public void CloseAllPanels()
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
            currentActivePanel = null;
            isPanelShowing = false;
            btnClose.SetActive(false);
            // 重新启用按钮
            SetButtonsInteractable(true);
        }
    }

    /// <summary>
    /// 统一设置简介按钮的可点击状态
    /// </summary>
    /// <param name="isInteractable">是否可点击</param>
    private void SetButtonsInteractable(bool isInteractable)
    {
        btnChild.interactable = isInteractable;
        btnScientist.interactable = isInteractable;
    }

    // 可选：场景切换时清理引用
    void OnDestroy()
    {
        btnChild.onClick.RemoveListener(ShowChildPanel);
        btnScientist.onClick.RemoveListener(ShowScientistPanel);
        btnClose.GetComponent<Button>().onClick.RemoveListener(CloseAllPanels);
    }
}