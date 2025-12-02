from datetime import datetime
from memory import load_memory, save_memory
from roles import get_role_prompt, get_break_rules
from logic import should_exit_by_user, should_exit_by_ai
from chat import chat_once

# 全局配置：对话记忆文件（相对于当前工作目录）
MEMORY_FILE = "3.1_memory_101/conversation_memory.json"


def main():
    """主程序入口：初始化对话历史，运行主循环，保存记忆"""
    print("欢迎使用【嘉悦记忆聊天机器人】！")
    print("输入 '再见' / '结束' / '退出' 可以结束对话。\n")

    # 加载历史记忆
    history = load_memory(MEMORY_FILE)
    if history:
        print(f"已加载历史对话 {len(history)} 条，继续在此基础上聊天。\n")
    else:
        print("暂无历史记忆，将从零开始新的对话。\n")

    # 主循环
    try:
        while True:
            user_input = input("你：")

            # 用户主动结束
            if should_exit_by_user(user_input):
                print("再见")
                break

            # 一轮对话
            try:
                reply = chat_once(history, user_input)
                print(f"嘉悦：{reply}\n")

                # AI 主动结束
                if should_exit_by_ai(reply):
                    print("对话结束")
                    break

            except Exception as e:
                print(f"发生错误: {e}")
                break

    finally:
        # 结束时保存记忆（不包含 system，只保存 user/assistant 历史）
        save_memory(MEMORY_FILE, history)
        print(f"对话已保存到 {MEMORY_FILE}")


if __name__ == "__main__":
    main()