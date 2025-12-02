def should_exit_by_user(user_input: str) -> bool:
    """
    判断用户是否想要结束对话。

    兼容多种退出指令，例如：再见 / 退出 / 结束 / bye / exit。
    """
    if not isinstance(user_input, str):
        return False

    exit_words = ["再见", "退出", "结束", "bye", "exit"]
    return user_input.strip().lower() in exit_words


def should_exit_by_ai(ai_reply: str) -> bool:
    """
    判断 AI 的回复是否表示要结束对话。

    逻辑参考 1.py：
    - 去掉空格和中英文常见标点
    - 如果只包含“再见”或长度很短且包含“再见”，则视为结束。
    """
    if not isinstance(ai_reply, str):
        return False

    reply_cleaned = (
        ai_reply.strip()
        .replace(" ", "")
        .replace("！", "")
        .replace("!", "")
        .replace("，", "")
        .replace(",", "")
    )

    if reply_cleaned == "再见" or (len(reply_cleaned) <= 5 and "再见" in reply_cleaned):
        return True

    return False