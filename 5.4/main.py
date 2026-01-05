"""
入口：串联场景描述、逻辑推导，并输出科普结果。
"""

from roles import get_scenario_intro, get_disclaimer
from logic import build_report


def main():
    print(get_scenario_intro())
    print()
    print(build_report())
    print()
    print(get_disclaimer())


if __name__ == "__main__":
    main()

