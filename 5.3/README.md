# 粒子人物画像生成器

基于 p5.js 的粒子效果人物画像生成器，支持上传图片转换为粒子效果，或使用默认轮廓。

## 功能特性

- 🎨 **粒子效果**：将图片或轮廓转换为动态粒子效果
- 📸 **图片上传**：支持上传任意图片并转换为粒子
- 🎛️ **实时调节**：可调节粒子大小、密度、动画速度
- ✨ **动态效果**：粒子围绕原始位置轻微浮动，营造生动效果

## 本地运行

1. 使用 Live Server 或任何静态文件服务器
2. 打开 `index.html` 即可

## 部署到 Vercel

### 方法一：通过 Vercel CLI

```bash
# 安装 Vercel CLI
npm i -g vercel

# 在项目目录下运行
cd 5.3
vercel

# 按照提示完成部署
```

### 方法二：通过 GitHub

1. 将代码推送到 GitHub 仓库
2. 在 [Vercel](https://vercel.com) 导入项目
3. 选择项目根目录为 `5.3`
4. 点击部署

### 方法三：直接拖拽

1. 访问 [Vercel Dashboard](https://vercel.com/dashboard)
2. 点击 "New Project"
3. 选择 "Import Git Repository" 或直接拖拽 `5.3` 文件夹

## 文件结构

```
5.3/
├── index.html      # 主页面
├── style.css       # 样式文件
├── sketch.js       # p5.js 核心逻辑
├── vercel.json     # Vercel 部署配置
├── package.json    # 项目配置
└── README.md       # 说明文档
```

## 使用说明

1. **上传图片**：点击"上传图片"按钮，选择一张人物照片
2. **调节参数**：
   - **粒子大小**：控制每个粒子的大小（1-8）
   - **粒子密度**：控制粒子的密集程度（2-10，数值越大越稀疏）
   - **动画速度**：控制粒子的浮动速度（0-2）
3. **重置**：清空当前效果
4. **使用默认轮廓**：切换到预设的人物轮廓

## 技术栈

- p5.js - 创意编程库
- HTML5 Canvas - 画布渲染
- CSS3 - 现代化样式

## 浏览器支持

- Chrome/Edge (推荐)
- Firefox
- Safari

## 许可证

MIT

