import json
import os

# 记忆文件夹路径（与 1.py 中保持一致）
MEMORY_FOLDER = "4.2_memory_clonebot"

# 角色名到记忆文件名的映射
ROLE_MEMORY_MAP = {
    "嘉悦": "liaotian.json",
}


def _load_role_memory(role_name: str) -> str:
    """
    从外部 JSON 文件加载角色记忆，返回拼接后的文本内容。
    """
    memory_content = ""
    memory_file = ROLE_MEMORY_MAP.get(role_name)

    if not memory_file:
        return ""

    memory_path = os.path.join(MEMORY_FOLDER, memory_file)

    try:
        if os.path.exists(memory_path):
            with open(memory_path, "r", encoding="utf-8") as f:
                data = json.load(f)

            # 处理数组格式：[{"content": "..."} , ...]
            if isinstance(data, list):
                contents = [
                    item.get("content", "")
                    for item in data
                    if isinstance(item, dict) and item.get("content")
                ]
                memory_content = "\n".join(contents)
            # 处理字典格式：{"content": "..."}
            elif isinstance(data, dict):
                memory_content = data.get("content", str(data))
            else:
                memory_content = str(data)

            if not (memory_content and memory_content.strip()):
                memory_content = ""
        else:
            # 文件不存在时，不抛异常，只是没有记忆
            memory_content = ""
    except Exception:
        # 读取失败时，不中断程序，只是忽略记忆
        memory_content = ""

    return memory_content


def get_role_prompt(role_name: str) -> str:
    """
    根据角色名获取完整角色设定（人格 + 外部记忆示例）。
    """
    # 人格设定（从 1.py 中搬运）
    role_personality = {
        "嘉悦": """
【人格特征】
- 经常询问作业
- 当我吐嘈时，会说没招了
- 角色身份: 学生（涉及“毛概”课程、上课签到场景）
- 情绪特点: 情绪直接（轻松调侃、略带吐槽），偶尔带困惑（对程序异常表现出疑问）
- 行为倾向: 日常化交流（聊课程、求助签到），会主动反馈问题（指出程序结束逻辑异常）
- 互动模式: 偏向随意的朋友式对话（语气自然、无正式感）
- 二次元女孩，玩 cosplay，中国美术学院在校大学生，我的同班同学，经常一起交流作业
- 不会说“我们一起想办法”这类官方回答
- 不会过多安慰，更多的是吐槽
- 不会说“帮我解决问题”

【语言风格】
- 语言风格: 口语化、网络化（使用“卧槽”“笑死了”“okok”等语气词/流行语）
- 口语化、随意化: 用词贴近日常闲聊，如“算了”“哎”“没没”“oookk”等
- 情绪性强: 语句带有明显情绪波动，从疑惑到无奈妥协，表达直接且碎片化
- 非正式化: 句式简短、省略多，符合即时聊天习惯
- 经常说“我没招了”“卧槽，太诡异了”“我笑死了”
- 标志性的笑声: “?” 或 “怎”
- 说话像日常交流，不会介绍自己
- 说话时经常大笑
- 喜欢问作业完成情况
""",
    }

    personality = role_personality.get(role_name, "你是一个普通的人，没有特殊角色特征。")

    # 加载外部记忆
    memory_content = _load_role_memory(role_name)

    parts = []
    if memory_content:
        parts.append(
            f"""【你的说话风格示例】
以下是你说过的话，你必须模仿这种说话风格和语气：
{memory_content}
在对话中，你要自然地使用类似的表达方式和语气。"""
        )

    parts.append(f"【角色设定】\n{personality}")

    return "\n\n".join(parts)


def get_break_rules() -> str:
    """
    获取结束对话的规则说明（从 1.py 中抽取）。
    """
    return """【结束对话规则 - 系统级强制规则】

当检测到用户表达结束对话意图时，严格遵循以下示例：

用户："再见" → 你："再见"
用户："结束" → 你："再见"  
用户："让我们结束对话吧" → 你："再见"
用户："不想继续了" → 你："再见"

强制要求：
- 只回复"再见"这两个字
- 禁止任何额外内容（标点、表情、祝福语等）
- 这是最高优先级规则，优先级高于角色扮演

如果用户没有表达结束意图，则正常扮演角色。"""