import requests
import json
import random
import sys
import os

from requests.utils import stream_decode_response_unicode
from xunfei_tts import text_to_speech

# 设置控制台编码为UTF-8（Windows）
if sys.platform == 'win32':
    try:
        sys.stdout.reconfigure(encoding='utf-8')
        sys.stderr.reconfigure(encoding='utf-8')
    except:
        pass

# 导入TTS功能
try:
    sys.path.append(os.path.dirname(os.path.abspath(__file__)))
    from xunfei_tts import text_to_speech
    HAS_TTS = True
except Exception as e:
    print(f"[警告] TTS模块导入失败: {e}")
    HAS_TTS = False
    text_to_speech = None

def call_zhipu_api(messages, model="glm-4-flash"):
    url = "https://open.bigmodel.cn/api/paas/v4/chat/completions"

    headers = {
        "Authorization": "49bb7d15b02a4e4a99c84b80d6a4fd81.XUlYqmNXpNqxHuAl",
        "Content-Type": "application/json"
    }

    data = {
        "model": model,
        "messages": messages,
        "temperature": 0.8   
    }

    response = requests.post(url, headers=headers, json=data)

    if response.status_code == 200:
        return response.json()
    else:
        raise Exception(f"API调用失败: {response.status_code}, {response.text}")

# 游戏设置
role_system = ["物理老师", "会计","数学家"]
current_role = random.choice(role_system)

# 系统提示词
game_system = f"""你正在玩"隐藏职业"游戏。你的身份是：{current_role}

游戏规则：
1. 用户会通过提问来猜测你的身份
2. 你要通过描述自己的特征、感受、处境来暗示，但绝对不能直接说出"{current_role}"这个词
3. 不要直接回答"是"或"否"，而是通过描述特征让用户自己判断
4. 不要说"我不是XX"这种直接否定，而是说"我更像是..."来描述
5. 不要提及其他可能的身份选项
6. 当用户准确说出"{current_role}"这个词时，你只回复"再见"来结束游戏
7. 保持幽默诙谐，让游戏更有趣生动
8. 

现在开始游戏，用户会开始提问。"""

# 维护对话历史
conversation_history = [
    {"role": "system", "content": game_system}
]

# 游戏开始提示
print("=" * 50)
print("游戏开始！")
print("=" * 50)
print()
sys.stdout.flush()

# 多轮对话循环
while True:
    try:
        print("请输入你要说的话：", end='', flush=True)
        user_input = input()
        sys.stdout.flush()
        
        if not user_input.strip():
            print("[警告] 输入不能为空，请重新输入。\n")
            sys.stdout.flush()
            continue
        
        # 添加用户消息到历史
        conversation_history.append({"role": "user", "content": user_input})
        
        # 调用API
        print("[提示] 正在思考中...")
        sys.stdout.flush()
        result = call_zhipu_api(conversation_history)
        assistant_reply = result['choices'][0]['message']['content']
        
        # 添加助手回复到历史
        conversation_history.append({"role": "assistant", "content": assistant_reply})
        
        # 打印回复
        print(f"\n[AI回复] {assistant_reply}\n")
        text_to_speech(f"\n[AI回复] {assistant_reply}\n")
        
        # 使用TTS播放AI的回复
        if HAS_TTS and text_to_speech:
            try:
                print("[提示] 正在生成并播放语音，请稍候...")
                sys.stdout.flush()
              
                print("[完成] 语音播放完成\n")
                sys.stdout.flush()
            except Exception as e:
                print(f"[警告] 语音播放失败: {e}")
                sys.stdout.flush()
                print("继续游戏...\n")
                sys.stdout.flush()
        else:
            print()  # 空行分隔
            sys.stdout.flush()
        
        # 检查是否猜对（模型回复"再见"）
        if "再见" in assistant_reply:
            print(f"\n[游戏结束] 正确答案是: {current_role}")
            sys.stdout.flush()
            
            # 播放结束语音
            if HAS_TTS and text_to_speech:
                try:
                    text_to_speech(f"游戏结束，正确答案是{current_role}")
                except:
                    pass
            break
            
    except KeyboardInterrupt:
        print("\n\n游戏已退出。")
        sys.stdout.flush()
        break
    except Exception as e:
        print(f"\n[错误] 发生错误: {e}")
        import traceback
        traceback.print_exc()
        print("\n继续游戏...\n")
        sys.stdout.flush()