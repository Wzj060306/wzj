from typing import List, Dict, Any

from api import call_zhipu_api
from roles import get_role_prompt, get_break_rules


def chat_once(history: List[Dict[str, Any]], user_input: str) -> str:
    """
    进行一次对话交互，返回 AI 的回复内容。

    参数：
        history: 当前累积的对话历史（不包含 system 消息）
        user_input: 本轮用户输入

    说明：
        - 本函数会就地修改 history（追加 user / assistant 消息）
        - 每次调用都会重新构造 system 消息（角色设定 + 结束规则）
    """
    # 追加用户消息
    history.append({"role": "user", "content": user_input})

    # 系统消息：角色设定 + 结束规则
    role_prompt = get_role_prompt("嘉悦")
    system_message = role_prompt + "\n\n" + get_break_rules()

    api_messages = [{"role": "system", "content": system_message}] + history

    # 调用 API
    result = call_zhipu_api(api_messages)
    reply = result["choices"][0]["message"]["content"]

    # 追加 AI 回复到历史
    history.append({"role": "assistant", "content": reply})

    return reply