using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneSessionMemorySystem : MonoBehaviour
{
    public static SceneSessionMemorySystem Instance;

    [Header("双对话场景配置")]
    public string kidSceneName = "KidScene";
    public string scientistSceneName = "ScientistScene";
    [Header("记忆配置")]
    public int maxSessionHistory = 10; // 限制上下文长度，避免讯飞token超限

    // 会话记忆（仅内存，切场景重置）
    public class SessionMemory
    {
        public List<string> dialogueHistory = new List<string>();
        public bool isSessionActive = false;
        public string currentScene = "";
    }
    public SessionMemory currentSession = new SessionMemory();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 监听场景加载重置记忆
        }
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 场景加载时重置会话记忆
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == kidSceneName || scene.name == scientistSceneName)
        {
            // 进入对话场景，初始化新会话
            currentSession = new SessionMemory();
            currentSession.isSessionActive = true;
            currentSession.currentScene = scene.name;
            Debug.Log($"进入{scene.name}，会话记忆已初始化");
        }
        else
        {
            // 离开对话场景，清空记忆
            currentSession.isSessionActive = false;
            currentSession.dialogueHistory.Clear();
            currentSession.currentScene = "";
            Debug.Log("离开对话场景，会话记忆已清空");
        }
    }

    // 添加对话记录（玩家/机器人消息都要调用）
    public void AddDialogue(string line)
    {
        if (!currentSession.isSessionActive) return;
        currentSession.dialogueHistory.Add(line);
        // 限制历史条数，防止讯飞token超限
        if (currentSession.dialogueHistory.Count > maxSessionHistory)
            currentSession.dialogueHistory.RemoveAt(0);
    }

    // 获取带场景风格的讯飞上下文
    public string GetXunFeiContext()
    {
        if (!currentSession.isSessionActive || currentSession.dialogueHistory.Count == 0)
            return GetScenePrompt();

        string prompt = GetScenePrompt();
        return $"{prompt}\n对话历史：\n{string.Join("\n", currentSession.dialogueHistory)}";
    }

    // 场景专属语气提示（让讯飞按角色回复）
    string GetScenePrompt()
    {
        return currentSession.currentScene == kidSceneName
            ? "[角色：儿童机器人，语气活泼、简单易懂，用儿童化语言回复]"
            : "[角色：科学家机器人，语气严谨专业，用科学术语解答问题]";
    }
}