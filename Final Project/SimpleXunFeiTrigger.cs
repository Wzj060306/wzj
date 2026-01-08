using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UniversalXunFeiTrigger : MonoBehaviour
{
    [Header("UI组件选择（二选一）")]
    public InputField nativePlayerInput;
    public Text nativeDialogueText;
    public TMP_InputField tmpPlayerInput;
    public TMP_Text tmpDialogueText;
    public Button sendBtn;

    private SceneSessionMemorySystem memorySystem;
    private bool isInDialogue = false;
    private TMP_InputField _actualInput;
    private TMP_Text _actualText;
    private InputField _nativeInput;
    private Text _nativeText;
    private bool useTMP = false;

    private void Start()
    {
        memorySystem = SceneSessionMemorySystem.Instance;
        if (memorySystem == null)
        {
            Debug.LogError("请先创建MemorySystem并挂载SceneSessionMemorySystem脚本！");
            return;
        }

        InitUIComponent();
        if (_actualInput == null && _nativeInput == null)
        {
            Debug.LogError("未绑定输入框组件！");
            return;
        }
        if (_actualText == null && _nativeText == null)
        {
            Debug.LogError("未绑定对话文本组件！");
            return;
        }
        if (sendBtn == null)
        {
            Debug.LogError("未绑定提交按钮！");
            return;
        }

        isInDialogue = true;
        sendBtn.onClick.AddListener(SendToXunFei);
        InitWelcomeMsg();
    }

    void InitUIComponent()
    {
        if (tmpPlayerInput != null && tmpDialogueText != null)
        {
            useTMP = true;
            _actualInput = tmpPlayerInput;
            _actualText = tmpDialogueText;
        }
        else if (nativePlayerInput != null && nativeDialogueText != null)
        {
            useTMP = false;
            _nativeInput = nativePlayerInput;
            _nativeText = nativeDialogueText;
        }
    }

    // 初始化欢迎语，去掉机器人前缀，使用原生人格的问候
    void InitWelcomeMsg()
    {
        string welcome = memorySystem.currentSession.currentScene == memorySystem.kidSceneName
            ? "你好呀，想问我什么有趣的问题？"
            : "你好，有什么想知道的吗？";

        memorySystem.AddDialogue(welcome);
        UpdateDialogueText(welcome);
    }

    void SendToXunFei()
    {
        string playerMsg = useTMP ? _actualInput.text.Trim() : _nativeInput.text.Trim();
        if (string.IsNullOrEmpty(playerMsg) || !isInDialogue) return;

        string playerRecord = playerMsg;
        memorySystem.AddDialogue(playerRecord);
        UpdateDialogueText(playerRecord, true);

        if (useTMP) _actualInput.text = "";
        else _nativeInput.text = "";

        string xunFeiRequest = memorySystem.GetXunFeiContext() + $"\n玩家当前提问：{playerMsg}";
        StartCoroutine(SendXunFeiRequest(xunFeiRequest));
    }

    IEnumerator SendXunFeiRequest(string requestText)
    {
        // 替换为你的讯飞API真实请求代码
        yield return new WaitForSeconds(1f);
        // 模拟回复也去掉前缀，仅保留内容
        string aiReply = memorySystem.currentSession.currentScene == memorySystem.kidSceneName
            ? "我记住你的问题啦，这是我的回答～"
            : "根据科学理论，对此的解答如下：";

        memorySystem.AddDialogue(aiReply);
        UpdateDialogueText(aiReply, true);
    }

    void UpdateDialogueText(string content, bool isAppend = false)
    {
        if (useTMP)
        {
            _actualText.text = isAppend ? _actualText.text + "\n" + content : content;
        }
        else
        {
            _nativeText.text = isAppend ? _nativeText.text + "\n" + content : content;
        }
    }

    private void OnDestroy()
    {
        isInDialogue = false;
    }
}