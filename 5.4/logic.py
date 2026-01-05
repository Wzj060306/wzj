"""
核心逻辑：根据上升速率，推导到达关键高度的时间，并给出生理后果。
"""

from typing import List, Dict

# 1 英里/小时 ≈ 1.6 公里/小时 ≈ 0.027 公里/分钟
ASCENT_SPEED_KM_PER_MIN = 0.027


def _minutes_to_reach(alt_km: float) -> float:
    """计算到达指定高度所需时间（分钟）"""
    return alt_km / ASCENT_SPEED_KM_PER_MIN


def get_milestones() -> List[Dict[str, str]]:
    """
    关键高度里程碑及生理影响。
    返回列表，包含高度、时间、影响描述。
    """
    checkpoints = [
        (3.0, "轻度缺氧：思维变慢、视力下降，若无氧气面罩开始受影响。"),
        (5.5, "中度缺氧：头痛、气短，运动能力和判断力明显下降。"),
        (8.0, "严重缺氧：进入“死亡地带”，意识模糊，几分钟内会失去行动力。"),
        (12.0, "极低压 + 极寒：体温快速流失，可能出现高原肺水肿/脑水肿。"),
        (19.0, "阿姆斯特朗线附近：体液开始沸腾（气泡病），数十秒内失去意识并死亡。"),
        (30.0, "平流层更高：近真空与极寒，短时间内不可逆致死。"),
    ]

    result = []
    for alt, effect in checkpoints:
        minutes = _minutes_to_reach(alt)
        result.append(
            {
                "alt_km": f"{alt:.1f} km",
                "time_min": f"{minutes:.1f} 分钟",
                "effect": effect,
            }
        )
    return result


def build_report() -> str:
    """
    生成完整科普报告，按时间轴说明会发生什么以及致死原因。
    """
    lines = []
    lines.append("上升速率：1 英里/小时 ≈ 1.6 公里/小时 ≈ 0.027 公里/分钟。")
    lines.append("假设：无增压、无供氧、普通衣物。")
    lines.append("关键高度与致死机制：")

    for idx, item in enumerate(get_milestones(), 1):
        lines.append(
            f"{idx}. {item['alt_km']}（约 {item['time_min']} 到达）：{item['effect']}"
        )

    lines.append(
        "结论：在无防护、无氧条件下，该上升情景会在 8 km 后进入“死亡地带”，"
        "19 km（阿姆斯特朗线）附近因低压导致体液沸腾和缺氧，极短时间内死亡。"
    )

    return "\n".join(lines)

