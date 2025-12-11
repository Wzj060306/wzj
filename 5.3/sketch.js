// 粒子人物画像生成器
let particles = [];
let img = null;
let useImage = false;
let particleSize = 3;
let particleDensity = 5;
let animationSpeed = 0.5;
let defaultShape = null;

// 默认人物轮廓（简化版侧脸）
const defaultShapePoints = [
    // 头部轮廓
    {x: 200, y: 100}, {x: 220, y: 120}, {x: 240, y: 140}, {x: 250, y: 160},
    {x: 255, y: 180}, {x: 258, y: 200}, {x: 260, y: 220}, {x: 258, y: 240},
    {x: 255, y: 260}, {x: 250, y: 280}, {x: 240, y: 300}, {x: 225, y: 315},
    {x: 210, y: 325}, {x: 195, y: 330}, {x: 180, y: 332}, {x: 165, y: 330},
    {x: 150, y: 325}, {x: 135, y: 315}, {x: 125, y: 300}, {x: 120, y: 280},
    {x: 115, y: 260}, {x: 112, y: 240}, {x: 110, y: 220}, {x: 112, y: 200},
    {x: 115, y: 180}, {x: 120, y: 160}, {x: 130, y: 140}, {x: 150, y: 120},
    {x: 170, y: 110}, {x: 185, y: 105}, {x: 200, y: 100},
    // 颈部
    {x: 180, y: 332}, {x: 175, y: 360}, {x: 200, y: 365}, {x: 225, y: 360},
    {x: 220, y: 332},
    // 肩膀
    {x: 140, y: 365}, {x: 120, y: 380}, {x: 110, y: 400}, {x: 260, y: 400},
    {x: 250, y: 380}, {x: 230, y: 365}
];

function setup() {
    const canvas = createCanvas(800, 600);
    canvas.parent('canvas-container');
    pixelDensity(1);
    
    // 初始化控件
    setupControls();
    
    // 创建默认轮廓
    createDefaultShape();
}

function setupControls() {
    // 图片上传
    const imageUpload = document.getElementById('imageUpload');
    imageUpload.addEventListener('change', handleImageUpload);
    
    // 粒子大小
    const sizeSlider = document.getElementById('particleSize');
    const sizeValue = document.getElementById('sizeValue');
    sizeSlider.addEventListener('input', (e) => {
        particleSize = parseFloat(e.target.value);
        sizeValue.textContent = particleSize;
        regenerateParticles();
    });
    
    // 粒子密度
    const densitySlider = document.getElementById('particleDensity');
    const densityValue = document.getElementById('densityValue');
    densitySlider.addEventListener('input', (e) => {
        particleDensity = parseInt(e.target.value);
        densityValue.textContent = particleDensity;
        regenerateParticles();
    });
    
    // 动画速度
    const speedSlider = document.getElementById('animationSpeed');
    const speedValue = document.getElementById('speedValue');
    speedSlider.addEventListener('input', (e) => {
        animationSpeed = parseFloat(e.target.value);
        speedValue.textContent = animationSpeed;
    });
    
    // 重置按钮
    document.getElementById('resetBtn').addEventListener('click', () => {
        particles = [];
        img = null;
        useImage = false;
        createDefaultShape();
    });
    
    // 默认轮廓按钮
    document.getElementById('defaultBtn').addEventListener('click', () => {
        img = null;
        useImage = false;
        createDefaultShape();
    });
}

function handleImageUpload(e) {
    const file = e.target.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = (event) => {
            loadImage(event.target.result, (loadedImg) => {
                img = loadedImg;
                useImage = true;
                generateParticlesFromImage();
            });
        };
        reader.readAsDataURL(file);
    }
}

function createDefaultShape() {
    particles = [];
    useImage = false;
    
    // 根据默认轮廓点生成粒子
    for (let i = 0; i < defaultShapePoints.length; i++) {
        const p1 = defaultShapePoints[i];
        const p2 = defaultShapePoints[(i + 1) % defaultShapePoints.length];
        
        const distance = dist(p1.x, p1.y, p2.x, p2.y);
        const numParticles = Math.floor(distance / particleDensity);
        
        for (let j = 0; j < numParticles; j++) {
            const t = j / numParticles;
            const x = lerp(p1.x, p2.x, t);
            const y = lerp(p1.y, p2.y, t);
            
            particles.push(new Particle(
                x + width / 2 - 185,
                y + height / 2 - 250,
                random(100, 255),
                random(150, 255),
                random(200, 255)
            ));
        }
    }
    
    // 填充内部区域（简化处理）
    fillInterior();
}

function fillInterior() {
    // 在轮廓内部随机生成一些粒子
    const centerX = width / 2;
    const centerY = height / 2 - 50;
    
    for (let i = 0; i < 200; i++) {
        const angle = random(TWO_PI);
        const radius = random(0, 120);
        const x = centerX + cos(angle) * radius;
        const y = centerY + sin(angle) * radius;
        
        // 简单检查是否在轮廓内（简化版）
        if (isInsideShape(x, y)) {
            particles.push(new Particle(
                x,
                y,
                random(150, 255),
                random(180, 255),
                random(200, 255)
            ));
        }
    }
}

function isInsideShape(x, y) {
    // 简化的点内判断（基于中心距离）
    const centerX = width / 2;
    const centerY = height / 2 - 50;
    const distToCenter = dist(x, y, centerX, centerY);
    return distToCenter < 100;
}

function generateParticlesFromImage() {
    if (!img) return;
    
    particles = [];
    
    // 调整图片大小以适应画布
    const scale = min(width / img.width, height / img.height) * 0.8;
    const imgW = img.width * scale;
    const imgH = img.height * scale;
    const offsetX = (width - imgW) / 2;
    const offsetY = (height - imgH) / 2;
    
    // 采样图片像素生成粒子
    img.loadPixels();
    const step = particleDensity;
    
    for (let y = 0; y < img.height; y += step) {
        for (let x = 0; x < img.width; x += step) {
            const index = (y * img.width + x) * 4;
            const r = img.pixels[index];
            const g = img.pixels[index + 1];
            const b = img.pixels[index + 2];
            const a = img.pixels[index + 3];
            
            // 只处理不透明的像素
            if (a > 128) {
                const px = (x / img.width) * imgW + offsetX;
                const py = (y / img.height) * imgH + offsetY;
                
                particles.push(new Particle(px, py, r, g, b));
            }
        }
    }
}

function regenerateParticles() {
    if (useImage && img) {
        generateParticlesFromImage();
    } else {
        createDefaultShape();
    }
}

function draw() {
    background(26, 26, 46);
    
    // 绘制所有粒子
    for (const particle of particles) {
        particle.update();
        particle.display();
    }
}

class Particle {
    constructor(x, y, r = 255, g = 255, b = 255) {
        this.originalX = x;
        this.originalY = y;
        this.x = x;
        this.y = y;
        this.r = r;
        this.g = g;
        this.b = b;
        this.offsetX = random(-20, 20);
        this.offsetY = random(-20, 20);
        this.phase = random(TWO_PI);
        this.speed = random(0.01, 0.03);
    }
    
    update() {
        // 粒子围绕原始位置轻微浮动
        this.phase += this.speed * animationSpeed;
        this.x = this.originalX + cos(this.phase) * this.offsetX * animationSpeed;
        this.y = this.originalY + sin(this.phase) * this.offsetY * animationSpeed;
    }
    
    display() {
        noStroke();
        fill(this.r, this.g, this.b, 200);
        circle(this.x, this.y, particleSize);
    }
}

