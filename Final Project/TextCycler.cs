using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 文本循环切换脚本
/// 功能：1. 背景激活时自动显示文本框 2. 点击文本框/按空格切换多段文字 3. 最后一段文字点击后跳转场景
/// 挂载对象：ClickableTextBox
/// </summary>
public class TextCycler : MonoBehaviour
{
    [Header("手动拖拽赋值（核心：直接拖入场景对象）")]
    public GameObject targetBg; // 拖入场景中的 NewBackground 对象
    public GameObject textBoxContainer; // 拖入场景中的 ClickableTextBox 对象
    public TextMeshProUGUI textBoxText; // 拖入 ClickableTextBox 下的 Text (TMP) 组件

    [Header("文字内容配置")]
    public string[] textOptions = {
        "你好！你是一个意外启动“低空装置”的人类，你将以恒定每小时1英里的速度匀速上升，这听起来十分有趣吧，你第一时间会想到什么？你或许有无数的想法在脑中掠过，没关系这都是可以被支持的……",
        "或许你觉得自己拥有了翅膀，又或者你很害怕很慌张，不知道接下来该怎么办，或者觉得自己会死翘翘，想知道自己是怎么死的吗？这些问题这里都可以解决",
        "选择你想要的人物去大胆提问吧！",
        "点击此处，进入人物选择界面"
    };

    [Header("场景跳转配置")]
    public string characterSelectSceneName = "CharacterSelect";
    public bool hideTextBoxOnLastText = true;

    [Header("手动输入文本的保存键名")]
    public string saveKey = "CustomTextContent";

    [Header("调试开关（可选）")]
    public bool skipBgCheck = false;
    public bool clearCustomTextOnStart = true;

    private int currentIndex = 0;
    private bool isCustomText = false;
    private string customText;

    void Start()
    {
        if (clearCustomTextOnStart)
        {
            PlayerPrefs.DeleteKey(saveKey);
            Debug.Log("启动时已清空自定义文本缓存");
        }

        if (textBoxContainer != null)
        {
            textBoxContainer.SetActive(false);
        }
        else
        {
            Debug.LogError("textBoxContainer未赋值，无法初始化文本框显隐！");
            return;
        }

        if (textBoxText != null)
        {
            if (PlayerPrefs.HasKey(saveKey))
            {
                customText = PlayerPrefs.GetString(saveKey);
                textBoxText.text = customText;
                isCustomText = true;
                Debug.Log($"初始化：加载自定义文本，内容为：{customText}");
            }
            else
            {
                if (textOptions.Length > 0)
                {
                    textBoxText.text = textOptions[currentIndex];
                    Debug.Log($"初始化：显示默认文本第{currentIndex + 1}段");
                }
                else
                {
                    Debug.LogError("文本数组textOptions为空，请在Inspector中填写文字内容！");
                    textBoxText.text = "请配置文字内容";
                }
            }
        }
        else
        {
            Debug.LogError("textBoxText未赋值，请拖拽绑定Text (TMP)组件！");
        }

        UpdateTextBoxVisibility();
    }

    void Update()
    {
        UpdateTextBoxVisibility();

        // 空格键触发
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnTextBoxClicked();
            Debug.Log("空格键触发文字切换");
        }

        // 鼠标点击文本框区域触发（修复坐标检测逻辑）
        if (Input.GetMouseButtonDown(0) && textBoxContainer != null && textBoxContainer.activeSelf)
        {
            // 获取Canvas和对应的UI相机
            Canvas canvas = textBoxContainer.GetComponentInParent<Canvas>();
            Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

            Vector2 localPoint;
            RectTransform containerRect = textBoxContainer.GetComponent<RectTransform>();
            // 将屏幕坐标转换为UI本地坐标
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                containerRect,
                Input.mousePosition,
                uiCamera,
                out localPoint
            ) && containerRect.rect.Contains(localPoint))
            {
                OnTextBoxClicked();
                Debug.Log("鼠标点击文本框区域，触发切换");
            }
        }
    }

    private void UpdateTextBoxVisibility()
    {
        if (textBoxContainer == null) return;

        if (skipBgCheck)
        {
            textBoxContainer.SetActive(true);
            return;
        }

        if (targetBg != null)
        {
            textBoxContainer.SetActive(targetBg.activeSelf);
            if (Application.isPlaying && Time.frameCount % 30 == 0)
            {
                Debug.Log($"帧更新：背景激活状态={targetBg.activeSelf}，文本框显隐状态={textBoxContainer.activeSelf}");
            }
        }
        else
        {
            Debug.LogError("targetBg未赋值，请拖拽绑定NewBackground对象！");
            textBoxContainer.SetActive(false);
        }
    }

    public void OnTextBoxClicked()
    {
        Debug.Log("==========点击事件触发==========");

        if (textBoxText == null)
        {
            Debug.LogError("textBoxText未赋值，无法切换文字！");
            return;
        }
        if (textOptions.Length == 0)
        {
            Debug.LogError("文本数组textOptions为空，无法切换文字！");
            textBoxText.text = "请配置文字内容";
            return;
        }

        if (!skipBgCheck)
        {
            if (targetBg == null)
            {
                Debug.LogError("targetBg未赋值，无法执行点击逻辑！");
                return;
            }
            if (!targetBg.activeSelf)
            {
                Debug.LogWarning("背景未激活，点击逻辑跳过！若需测试可开启skipBgCheck调试开关");
                return;
            }
        }

        if (isCustomText)
        {
            currentIndex = 1;
            isCustomText = false;
            textBoxText.text = textOptions[currentIndex];
            PlayerPrefs.DeleteKey(saveKey);
            Debug.Log($"自定义文本状态：切换到默认文本第{currentIndex + 1}段");
            return;
        }

        if (currentIndex == textOptions.Length - 1)
        {
            Debug.Log($"当前是最后一段文字，准备跳转到场景：{characterSelectSceneName}");
            LoadCharacterSelectScene();
            if (hideTextBoxOnLastText && textBoxContainer != null)
            {
                textBoxContainer.SetActive(false);
                Debug.Log("最后一段文字点击后，隐藏文本框");
            }
            return;
        }

        currentIndex = (currentIndex + 1) % textOptions.Length;
        textBoxText.text = textOptions[currentIndex];
        PlayerPrefs.DeleteKey(saveKey);
        Debug.Log($"文本切换成功：当前显示第{currentIndex + 1}段文字");
    }

    private void LoadCharacterSelectScene()
    {
        if (string.IsNullOrEmpty(characterSelectSceneName))
        {
            Debug.LogError("人物选择场景名未配置，请在Inspector中填写characterSelectSceneName");
            textBoxText.text = "场景名未配置！";
            return;
        }

        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == characterSelectSceneName)
            {
                sceneExists = true;
                break;
            }
        }

        if (!sceneExists)
        {
            Debug.LogError($"场景{characterSelectSceneName}未添加到Build Settings中！");
            textBoxText.text = $"场景{characterSelectSceneName}未配置！";
            return;
        }

        try
        {
            SceneManager.LoadScene(characterSelectSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载场景失败：{e.Message}");
            textBoxText.text = "场景加载失败！";
        }
    }

    [ContextMenu("保存当前手动输入的文本")]
    public void SaveCustomText()
    {
        if (textBoxText != null && (skipBgCheck || (targetBg != null && targetBg.activeSelf)))
        {
            customText = textBoxText.text;
            PlayerPrefs.SetString(saveKey, customText);
            isCustomText = true;
            PlayerPrefs.Save();
            Debug.Log($"自定义文本已保存：{customText}");
        }
        else
        {
            Debug.LogWarning("保存自定义文本失败：文本组件未赋值，或背景未激活（可开启skipBgCheck）");
        }
    }

    void OnDestroy()
    {
        PlayerPrefs.Save();
    }

    void OnValidate()
    {
        if (Application.isPlaying && textBoxText != null && (skipBgCheck || (targetBg != null && targetBg.activeSelf)))
        {
            SaveCustomText();
        }
    }
}