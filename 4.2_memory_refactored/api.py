import requests

from requests.utils import stream_decode_response_unicode  # 保留以便未来支持流式


def call_zhipu_api(messages, model: str = "glm-4-flash"):
    """
    调用智谱 API 获取 AI 回复。

    参数：
        messages: 对话消息列表，形如 [{"role": "user", "content": "..."}]
        model: 模型名称，默认为 "glm-4-flash"

    返回：
        API 返回的 JSON 数据（字典格式）。
    """
    url = "https://open.bigmodel.cn/api/paas/v4/chat/completions"

    # 使用 1.py 中的密钥和配置
    headers = {
        "Authorization": "49bb7d15b02a4e4a99c84b80d6a4fd81.XUlYqmNXpNqxHuAl",
        "Content-Type": "application/json",
    }

    data = {
        "model": model,
        "messages": messages,
        "temperature": 0.5,
    }

    response = requests.post(url, headers=headers, json=data)

    if response.status_code == 200:
        return response.json()
    else:
        raise Exception(f"API调用失败: {response.status_code}, {response.text}")