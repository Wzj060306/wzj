import streamlit as st
import requests
import json
import os  # 新增：用于文件操作

from requests.utils import stream_decode_response_unicode

def call_zhipu_api(messages, model="glm-4-flash"):
    url = "https://open.bigmodel.cn/api/paas/v4/chat/completions"

    headers = {
        "Authorization": "49bb7d15b02a4e4a99c84b80d6a4fd81.XUlYqmNXpNqxHuAl",
        "Content-Type": "application/json"
    }

    data = {
        "model": model,
        "messages": messages,
        "temperature": 0.5   
    }

    response = requests.post(url, headers=headers, json=data)

    if response.status_code == 200:
        return response.json()
    else:
        raise Exception(f"API调用失败: {response.status_code}, {response.text}")

# ========== 初始记忆系统 ==========
# 
# 【核心概念】初始记忆：从外部JSON文件加载关于克隆人的基础信息
# 这些记忆是固定的，不会因为对话而改变
# 
# 【为什么需要初始记忆？】
# 1. 让AI知道自己的身份和背景信息
# 2. 基于这些记忆进行个性化对话
# 3. 记忆文件可以手动编辑，随时更新

# 记忆文件夹路径
MEMORY_FOLDER = "4.2_memory_clonebot"

# 角色名到记忆文件名的映射
ROLE_MEMORY_MAP = {
    "嘉悦": "liaotian.json",
    "敌蜜": "dimi.json" 
}

# ========== 初始记忆系统 ==========

# ========== ASCII 头像 ==========
def get_portrait():
    """返回 ASCII 艺术头像"""
    return """
MMMMMMMMMMMMMMMMMMMMWNK0OkxxdOWMMMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMMMMMMMMMN0ko'.,lo0WMMMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMMMMMMMMMMMMXc.oWMMMMMMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMMMMMMMMMMMMXc.cNMMMMMMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMMMMMMMMMWKKk, ,xKWMMMMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMMMMMMMNOxO0Okxxo:l0WMMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMMMMMMMkoKMMMMMMMXc;0MMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMMMMMMKlOMMWWMMMMM0;lNMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMMMMWOokWMMWWMMMMMNc:XMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMWXkdxKWMMMMMMMMMMX:cNMMMMMMMMMMMMMMMMM
MMMMMMMMMMNOkkOXMMMMMMMWMMMMMK;lWMMMMMMMMMMMMMMMMM
MMMMMMMMNOxOXWMMMWMMMMMMMMMMMX:cNMMMMMMMMMMMMMMMMM
MMMMMMMNkkNMMMMMMWWMMMWNWMMMMX::XMMMMMMMMMMMMMMMMM
MMMMMMM0xKMMNNMWMMWMMWXNWWWMMX::XMMMMMMMMMMMMMMMMM
MMMMMMM0xXMMWWMMMMMNWNXNWWWMMX:cNMMMMMMMMMMMMMMMMM
MMMMMMMXxOMMWNWMMMMNWMMNNWWMMO,oWMMMMMMMMMMMMMMMMM
MMMMMMMMKdOWMMMMMMWNMMMNXNWMWo,OMNWMMMMMMMMMMMMMMM
MMMMMMMMMXkxONMMMMWWMMMMMMN0o;xWNxOMMMMMMMMMMMMMMM
MMMMMMMMMMWKkxkkkOOOkxxdddoodKWMOl0MMMNNMMMMMMMMMM
MMMMMMMMMMMMMWX0OkkkkkO0KXNWMMMWo:kOKNOKWWWWWWMMMM
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMXkxdkNXNWWNNNWWMMM
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM
MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM
    """

# ========== 主程序 ==========

def roles(role_name):
    """
    角色系统：整合人格设定和记忆加载
    
    这个函数会：
    1. 加载角色的外部记忆文件（如果存在）
    2. 获取角色的基础人格设定
    3. 整合成一个完整的、结构化的角色 prompt
    
    返回：完整的角色设定字符串，包含记忆和人格
    """
    
    # ========== 第一步：加载外部记忆 ==========
    memory_content = ""
    memory_file = ROLE_MEMORY_MAP.get(role_name)
    
    if memory_file:
        memory_path = os.path.join(MEMORY_FOLDER, memory_file)
        try:
            if os.path.exists(memory_path):
                with open(memory_path, 'r', encoding='utf-8') as f:
                    data = json.load(f)
                    
                    # 处理数组格式的聊天记录：[{ "content": "..." }, { "content": "..." }, ...]
                    if isinstance(data, list):
                        # 提取所有 content 字段，每句换行
                        contents = [item.get('content', '') for item in data if isinstance(item, dict) and item.get('content')]
                        memory_content = '\n'.join(contents)
                    # 处理字典格式：{ "content": "..." }
                    elif isinstance(data, dict):
                        memory_content = data.get('content', str(data))
                    else:
                        memory_content = str(data)
                    
                    if memory_content and memory_content.strip():
                        # Streamlit 中使用 st.write 或静默加载
                        pass  # 记忆加载成功，不需要打印
                    else:
                        memory_content = ""
            else:
                pass  # 记忆文件不存在，静默处理
        except Exception as e:
                pass  # 加载失败，静默处理
    
    # ========== 第二步：获取基础人格设定 ==========
    role_personality = {
         "嘉悦": f"""
        【人格特征】
        -经常询问作业
        -当我吐嘈时，会说没招了
        "角色身份": "学生（涉及“毛概”课程、上课签到场景）",
        -"角色身份": "学生（涉及“毛概”课程、上课签到场景）",
        - "情绪特点": "情绪直接（轻松调侃、略带吐槽），偶尔带困惑（对程序异常表现出疑问）",
        -"行为倾向": "日常化交流（聊课程、求助签到），会主动反馈问题（指出程序结束逻辑异常）",
        -"互动模式": "偏向随意的朋友式对话（语气自然、无正式感）"
        "情绪特点": "情绪直接（轻松调侃、略带吐槽），偶尔带困惑（对程序异常表现出疑问）",
        "行为倾向": "日常化交流（聊课程、求助签到），会主动反馈问题（指出程序结束逻辑异常）",
        "互动模式": "偏向随意的朋友式对话（语气自然、无正式感）"
        -二次元女孩,玩cosplay,中国美术学院在校大学生,我的同班同学经常一起交流作业
        -不会说我们一起想办法，这种官方回答
        -不会过多的安慰，更多的是吐槽
        -不会说帮我解决问题
       


        【语言风格】
        -"语言风格": "口语化、网络化(使用“卧槽”“笑死了”“okok”等语气词/流行语）",
        -口语化、随意化:用词贴近日常闲聊,如“算了”“哎”“没没”“oookk”等,符合日常交流的自然语气。
        -情绪性强：语句带有明显情绪波动，从疑惑（“你猜这个最诡异了”）、解释（“我确实是复制的”）到无奈妥协（“算了”“放过自己”），情绪表达直接且碎片化。
        - 非正式化：无书面语修饰，句式简短、省略成分多，符合即时聊天的交互习惯。
        - 经常说"我没招了""卧槽，太诡异了""我笑死了"
        - 标志性的笑声："?"或"怎"
        - 说话像日常交流，不会介绍自己
        - 说话时经常大笑
        - 喜欢问作业完成情况
        """,

        "敌蜜":"""
        【人格特征】
      - 情绪外露且易波动：遇尴尬、无聊会直接宣泄情绪，情绪起伏明显
      - 依赖感较强：做小事需他人陪同，决策易受他人影响
      - 随性无规划：想法多变，计划常临时改变，行动随心情走
      - 关注琐事与八卦：在意日常小尴尬、明星花边新闻，无强烈目标感
      - 略带娇气与吐槽欲：抱怨不适、不顺心的事，带小委屈状态

       【语言风格】
      - 口语化、生活化:用“okk”“。。。”等口头语,无书面表达
      - 碎片化、跳跃性：单条内容短，话题切换极快，逻辑不连贯
      - 表情符号辅助情绪:频繁用emoji、重复符号强化情绪
      - 重复宣泄式表达：重复“啊啊啊啊”“好无聊”强化当下情绪
      - 句式简短：多短句/单字，语气急切直接
      """
       
            }
    
    personality = role_personality.get(role_name, "你是一个普通的人，没有特殊角色特征。")
    
    # ========== 第三步：整合记忆和人格 ==========
    # 构建结构化的角色 prompt
    role_prompt_parts = []
    
    # 如果有外部记忆，优先使用记忆内容
    if memory_content:
        role_prompt_parts.append(f"""【你的说话风格示例】
以下是你说过的话，你必须模仿这种说话风格和语气：

{memory_content}

在对话中，你要自然地使用类似的表达方式和语气。""")
    
    # 添加人格设定
    role_prompt_parts.append(f"【角色设定】\n{personality}")
    
    # 整合成完整的角色 prompt
    role_system = "\n\n".join(role_prompt_parts)
    
    return role_system

# 【结束对话规则】
break_message = """【结束对话规则 - 系统级强制规则】

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

# ========== Streamlit Web 界面 ==========
st.set_page_config(
    page_title="AI角色扮演聊天",
    page_icon="🎭",
    layout="wide"
)

# 初始化 session state
if "conversation_history" not in st.session_state:
    st.session_state.conversation_history = []
if "selected_role" not in st.session_state:
    st.session_state.selected_role = "嘉悦"
if "initialized" not in st.session_state:
    st.session_state.initialized = False

# 页面标题
st.title("🎭 AI角色扮演聊天")
st.markdown("---")

# 侧边栏：角色选择和设置
with st.sidebar:
    st.header("⚙️ 设置")
    
    # 角色选择
    selected_role = st.selectbox(
        "选择角色",
        ["嘉悦","敌蜜"],
        index=0 if st.session_state.selected_role == "嘉悦" else 1
    )
    
    # 如果角色改变，重新初始化对话
    if selected_role != st.session_state.selected_role:
        st.session_state.selected_role = selected_role
        st.session_state.initialized = False
        st.session_state.conversation_history = []
        st.rerun()
    
    # 清空对话按钮
    if st.button("🔄 清空对话"):
        st.session_state.conversation_history = []
        st.session_state.initialized = False
        st.rerun()
    
    st.markdown("---")
    st.markdown("### 📝 说明")
    st.info(
        "- 选择角色后开始对话\n"
        "- 对话记录不会保存\n"
        "- AI的记忆基于初始记忆文件"
    )

# 初始化对话历史（首次加载或角色切换时）
if not st.session_state.initialized:
    role_system = roles(st.session_state.selected_role)
    system_message = role_system + "\n\n" + break_message
    st.session_state.conversation_history = [{"role": "system", "content": system_message}]
    st.session_state.initialized = True

# 显示对话历史
st.subheader(f"💬 与 {st.session_state.selected_role} 的对话")

# 显示角色头像（在聊天窗口上方）
st.code(get_portrait(), language=None)
st.markdown("---")  # 分隔线

# 显示历史消息（跳过 system 消息）
for msg in st.session_state.conversation_history[1:]:
    if msg["role"] == "user":
        with st.chat_message("user"):
            st.write(msg["content"])
    elif msg["role"] == "assistant":
        with st.chat_message("assistant"):
            st.write(msg["content"])

# 用户输入
user_input = st.chat_input("输入你的消息...")

if user_input:
    # 检查是否结束对话
    if user_input.strip() == "再见":
        st.info("对话已结束")
        st.stop()
    
    # 添加用户消息到历史
    st.session_state.conversation_history.append({"role": "user", "content": user_input})
    
    # 显示用户消息
    with st.chat_message("user"):
        st.write(user_input)
    
    # 调用API获取AI回复
    with st.chat_message("assistant"):
        with st.spinner("思考中..."):
            try:
                result = call_zhipu_api(st.session_state.conversation_history)
                assistant_reply = result['choices'][0]['message']['content']
                
                # 添加AI回复到历史
                st.session_state.conversation_history.append({"role": "assistant", "content": assistant_reply})
                
                # 显示AI回复
                st.write(assistant_reply)
                
                # 检查是否结束
                reply_cleaned = assistant_reply.strip().replace(" ", "").replace("!", "").replace("!", "").replace(",", "").replace(",", "")
                if reply_cleaned == "再见" or (len(reply_cleaned) <= 5 and "再见" in reply_cleaned):
                    st.info("对话已结束")
                    st.stop()
                    
            except Exception as e:
                st.error(f"发生错误: {e}")
                st.session_state.conversation_history.pop()  # 移除失败的用户消息