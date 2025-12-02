import json
import os
from typing import List, Dict, Any


def load_memory(file_path: str) -> List[Dict[str, Any]]:
    """
    从 JSON 文件加载对话历史。

    返回值为一个列表，元素形如：
        {"role": "user" | "assistant", "content": "..."}
    如果文件不存在或内容异常，则返回空列表。
    """
    if not file_path:
        return []

    if not os.path.exists(file_path):
        return []

    try:
        with open(file_path, "r", encoding="utf-8") as f:
            data = json.load(f)

        if isinstance(data, list):
            # 简单校验每个元素
            history = []
            for item in data:
                if isinstance(item, dict) and "role" in item and "content" in item:
                    history.append(
                        {"role": str(item["role"]), "content": str(item["content"])}
                    )
            return history

        # 其他格式一律忽略
        return []
    except Exception:
        # 读取失败时不抛异常，避免影响主流程
        return []


def save_memory(file_path: str, data: List[Dict[str, Any]]) -> None:
    """
    将对话历史保存到 JSON 文件。

    会自动创建中间目录。
    """
    if not file_path:
        return

    try:
        os.makedirs(os.path.dirname(file_path), exist_ok=True)
        with open(file_path, "w", encoding="utf-8") as f:
            json.dump(data, f, ensure_ascii=False, indent=2)
    except Exception:
        # 写入失败时静默处理，避免中断对话
        return