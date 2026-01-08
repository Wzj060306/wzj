using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using TMPro;
using System.Text; // 新增：引入Encoding所在的命名空间

[Serializable]
public class ZhipuResponse { public Choice[] choices; }
[Serializable]
public class Choice { public Message message; public int index; }
[Serializable]
public class Message { public string role; public string content; }

public class ZhipuApiHandler : MonoBehaviour
{
    [HideInInspector] public TMP_InputField playerInput;
    [HideInInspector] public TextMeshProUGUI answerText;
    public string apiKey = "替换为你的智谱API密钥";
    public string characterType = "小孩";
    private readonly string apiUrl = "https://open.bigmodel.cn/api/paas/v4/chat/completions";
    private readonly string model = "glm-4-flash";

    void Start()
    {
        BindUIElements();
    }

    private void BindUIElements()
    {
        GameObject inputObj = GameObject.Find("PlayerInput");
        if (inputObj != null)
        {
            playerInput = inputObj.GetComponent<TMP_InputField>();
            if (playerInput == null)
            {
                Debug.LogError("PlayerInput对象上未找到TMP_InputField组件！");
            }
        }
        else
        {
            Debug.LogError("未找到名为PlayerInput的UI对象，请检查命名！");
        }

        GameObject textObj = GameObject.Find("AnswerText");
        if (textObj != null)
        {
            answerText = textObj.GetComponent<TextMeshProUGUI>();
            if (answerText == null)
            {
                Debug.LogError("AnswerText对象上未找到TextMeshProUGUI组件！");
            }
        }
        else
        {
            Debug.LogError("未找到名为AnswerText的UI对象，请检查命名！");
        }
    }

    public void OnSubmitInput()
    {
        if (playerInput == null)
        {
            ShowMessage("输入框未绑定！");
            return;
        }

        string inputContent = playerInput.text.Trim();
        if (string.IsNullOrEmpty(inputContent))
        {
            ShowMessage("请输入内容后再提交！");
            return;
        }

        ShowMessage("正在回复中...");
        playerInput.text = "";
        StartCoroutine(SendZhipuRequest(inputContent));
    }

    IEnumerator SendZhipuRequest(string userInput)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey == "替换为你的智谱API密钥")
        {
            ShowMessage("请先填写智谱API密钥！");
            yield break;
        }

        string prompt = GetCharacterPrompt(userInput);
        string jsonData = "{\"model\":\"" + model + "\",\"messages\":[{\"role\":\"user\",\"content\":\"" + prompt + "\"}],\"temperature\":0.8}";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                ShowMessage("请求失败：" + request.error);
                Debug.LogError("API错误：" + request.downloadHandler.text);
            }
            else
            {
                try
                {
                    ZhipuResponse response = JsonUtility.FromJson<ZhipuResponse>(request.downloadHandler.text);
                    if (response?.choices != null && response.choices.Length > 0)
                    {
                        ShowMessage(response.choices[0].message.content);
                    }
                    else
                    {
                        ShowMessage("模型未返回有效内容");
                    }
                }
                catch (Exception e)
                {
                    ShowMessage("数据解析失败！");
                    Debug.LogError("解析错误：" + e.Message);
                }
            }
        }
    }

    private string GetCharacterPrompt(string input)
    {
        switch (characterType)
        {
            case "小孩":
                return "你现在是6岁的小朋友，说话天真可爱、用词简单，不能说明自己是AI,或者小助手，想象力丰富，把科学变成童话，问题比答案多，话语可以不切实际，相信自己能走到太空，像一个真实的人类一样说话，回复用户的问题：" + input;
            case "科学家":
                return "你现在是资深科学家，把自己当成人类，不能说明自己是AI，不能说自己是小助手，说话严谨理性、用专业知识解答问题，调用物理学知识和生物学知识，严谨的风险预判者，回复用户的问题：" + input;
            default:
                return input;
        }
    }

    private void ShowMessage(string content)
    {
        if (answerText != null)
        {
            answerText.text = content;
        }
    }
}