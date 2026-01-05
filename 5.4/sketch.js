// 全局变量定义
let character = null; // 当前角色对象
let startBtn; // 启动按钮
let charButtons = []; // 角色选择按钮
let equipBar; // 装备栏

// 图片资源变量
let BJ, KXJ, SW, XJ, XH, XJF, YQG, BD, MY;

// 角色类型枚举
const CHAR_TYPES = {
    CHILD: 'child',
    SCIENTIST: 'scientist',
    BIRD: 'bird'
};

// 死亡原因映射
const DEATH_TEXT = {
    suffocation: '窒息死亡',
    cold: '低温致死（冻成冰块）',
    fluid_boil: '体液沸腾死亡',
    radiation: '辐射病死亡',
    decompression_sickness: '减压病死亡',
    reach_star: '抵达星球！'
};

// 预加载函数：加载所有图片资源
function preload() {
    BJ = loadImage("./assets/BJ.jpg");
    KXJ = loadImage("./assets/KXJ.png");
    SW = loadImage("./assets/SW.png");
    XJ = loadImage("./assets/XJ.png");
    XH = loadImage("./assets/XH.png");
    XJF = loadImage("./assets/XJF.png");
    YQG = loadImage("./assets/YQG.png");
    BD = loadImage("./assets/BD.png");
    MY = loadImage("./assets/MY.png");
    console.log('图片资源加载中...');
}

// 初始化函数
function setup() {
     let cnv = createCanvas(1200, 720); // 固定画布尺寸
     // 初始化圆形按钮（位置、尺寸、文字、关联角色类型）
    startBtn = new Button(width - 80, height - 80, 60, 60, '启动');
    charButtons.push(new Button(50, 50, 60, 60, '小孩', CHAR_TYPES.CHILD));
    charButtons.push(new Button(130, 50, 60, 60, '科学家', CHAR_TYPES.SCIENTIST));
    charButtons.push(new Button(210, 50, 60, 60, '小鸡', CHAR_TYPES.BIRD));
    // 初始化装备栏
    equipBar = new EquipmentBar(50, height - 80, 200, 60);

    // 绑定重置按钮
    const resetBtn = document.getElementById('resetBtn');
    if (resetBtn) {
        resetBtn.addEventListener('click', resetScene);
    }
}

// 帧绘制函数（持续执行）
function draw() {
    // 绘制背景（如果有背景图片则使用，否则使用渐变）
    if (BJ && BJ.width > 0) {
        imageMode(CORNER);
        image(BJ, 0, 0, width, height);
    } else {
        // 绘制渐变天空背景（图片未加载时的备用方案）
        for (let y = 0; y < height; y++) {
            let shade = map(y, 0, height, 255, 155);
            let c = color(135, 206, shade);
            stroke(c);
            line(0, y, width, y);
        }
    }

    // 绘制所有按钮
    startBtn.update(mouseX, mouseY);
    startBtn.draw();
    charButtons.forEach(btn => {
        btn.update(mouseX, mouseY);
        btn.draw();
    });

    // 仅当选择科学家时，绘制装备栏
    if (character && character.type === CHAR_TYPES.SCIENTIST) {
        equipBar.draw();
    }

    // 绘制当前角色（若已选择）
    if (character) {
        character.update();
        character.draw();
    } else {
        // 提示选择角色
        fill(255, 255, 255, 180);
        textAlign(CENTER, CENTER);
        textSize(18);
        text('请先选择一个角色', width / 2, height / 2);
    }
}

// 重置整个场景
function resetScene() {
    character = null;
    // 重新初始化按钮悬浮状态
    startBtn.hovered = false;
    charButtons.forEach(btn => (btn.hovered = false));
}

// 鼠标点击事件
function mousePressed() {
    // 角色选择逻辑
    charButtons.forEach(btn => {
        if (btn.isHovered()) {
            character = new Character(btn.data);
        }
    });

    // 启动上升逻辑
    if (startBtn.isHovered()) {
        if (character) {
            character.startRising();
            console.log('启动上升！');
        } else {
            console.log('请先选择一个角色！');
            alert('请先选择一个角色（小孩/科学家/小鸡）');
        }
    }

    // 科学家装备切换逻辑
    if (character && character.type === CHAR_TYPES.SCIENTIST) {
        if (equipBar.coatBtn.isHovered()) {
            character.toggleEquip('coat');
        }
        if (equipBar.oxygenBtn.isHovered()) {
            character.toggleEquip('oxygen');
        }
    }
}

// 圆形按钮类（封装按钮的交互和绘制）
class Button {
    constructor(x, y, w, h, text, data = null) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        this.text = text;
        this.data = data; // 存储关联的角色类型
        this.hovered = false; // 鼠标悬浮状态
    }

    // 更新鼠标悬浮状态（圆形碰撞检测）
    update(mx, my) {
        const centerX = this.x + this.w / 2;
        const centerY = this.y + this.h / 2;
        const distance = Math.sqrt((mx - centerX) ** 2 + (my - centerY) ** 2);
        this.hovered = distance < this.w / 2;
    }

    // 绘制按钮
    draw() {
        push(); // 保存绘图状态
        const centerX = this.x + this.w / 2;
        const centerY = this.y + this.h / 2;

        // 绘制圆形按钮背景
        fill(this.hovered ? color(0, 200, 0, 200) : color(220, 220, 220, 200));
        stroke(0);
        strokeWeight(2);
        ellipse(centerX, centerY, this.w, this.h);

        // 绘制按钮文字
        fill(this.text === '启动' ? 255 : 0);
        noStroke();
        textSize(14);
        textAlign(CENTER, CENTER);
        text(this.text, centerX, centerY);

        pop(); // 恢复绘图状态
    }

    // 返回鼠标悬浮状态
    isHovered() {
        return this.hovered;
    }
}

// 装备栏类（封装装备栏的绘制和交互）
class EquipmentBar {
    constructor(x, y, w, h) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        // 初始化装备按钮（棉衣、氧气罐）
        this.coatBtn = new Button(x + 30, y + 30, 40, 40, '');
        this.oxygenBtn = new Button(x + 100, y + 30, 40, 40, '');
    }

    // 绘制装备栏
    draw() {
        push(); // 保存绘图状态
        // 绘制装备栏背景（圆角矩形）
        fill(240, 200);
        stroke(0);
        strokeWeight(2);
        rect(this.x, this.y, this.w, this.h, 10);

        // 绘制装备栏标题
        fill(0);
        noStroke();
        textSize(14);
        textAlign(LEFT, TOP);
        text('装备栏', this.x + 10, this.y - 15);

        // 更新并绘制装备按钮
        this.coatBtn.update(mouseX, mouseY);
        this.coatBtn.draw();
        this.oxygenBtn.update(mouseX, mouseY);
        this.oxygenBtn.draw();

        // 绘制棉衣图标（代码绘图）
        push();
        translate(this.coatBtn.x + 20, this.coatBtn.y + 20);
        fill(0, 150, 255);
        stroke(0);
        rect(-15, -15, 30, 30);
        pop();

        // 绘制氧气罐图标（代码绘图）
        push();
        translate(this.oxygenBtn.x + 20, this.oxygenBtn.y + 20);
        fill(0, 200, 0);
        stroke(0);
        rect(-10, -10, 20, 20);
        strokeWeight(1);
        line(0, 10, 0, 20);
        pop();

        // 绘制装备文字说明（放在图标上方，颜色对比更明显）
        fill(255);
        noStroke();
        textSize(13);
        textAlign(CENTER, BOTTOM);
        text('棉衣', this.coatBtn.x + this.coatBtn.w / 2, this.coatBtn.y - 6);
        text('氧气罐', this.oxygenBtn.x + this.oxygenBtn.w / 2, this.oxygenBtn.y - 6);

        pop(); // 恢复绘图状态
    }
}

// 角色类（封装角色的属性、动画和绘制）
class Character {
    constructor(type) {
        this.type = type; // 角色类型
        this.x = width / 2; // 初始x坐标（画布居中）
        this.y = height - 100; // 初始y坐标（画布下方）
        this.speed = 0.5; // 上升速度（视觉速度，已适配画布尺寸）
        this.isRising = false; // 是否开始上升
        this.equipped = { coat: false, oxygen: false }; // 装备状态
        this.deathCause = null; // 死亡原因（null表示存活）
        this.risingHeight = 0; // 上升高度累计
        // 动画参数
        this.wingFlap = 0; // 小鸡翅膀扇动角度
        this.equipFlash = 0; // 装备切换闪光帧数
        this.iceScale = 0; // 冰块缩放动画参数（低温致死专用）
    }

    // 开始上升
    startRising() {
        this.isRising = true;
    }

    // 切换装备状态
    toggleEquip(equipType) {
        this.equipped[equipType] = !this.equipped[equipType];
        this.equipFlash = 10; // 装备切换时闪光10帧
    }

    // 更新角色状态（动画、位置、死亡判断）
    update() {
        if (this.isRising && !this.deathCause) {
            this.y -= this.speed; // 上升移动
            this.risingHeight += this.speed; // 累计上升高度
            this.checkEffects(); // 检测触发的效果（星球、翅膀、死亡）
            
            // 仅对科学家进行“锁定在中部”的处理，其他角色保持原始上升轨迹
            if (this.type === CHAR_TYPES.SCIENTIST) {
                const midBandTop = height * 0.3;
                const midBandBottom = height * 0.7;
                if (this.y < midBandTop) this.y = midBandTop;
                if (this.y > midBandBottom) this.y = midBandBottom;
            }
        }

        // 小鸡翅膀扇动动画更新
        if (this.type === CHAR_TYPES.BIRD && this.wings) {
            this.wingFlap = (this.wingFlap + 0.1) % Math.PI;
        }

        // 装备闪光动画更新
        if (this.equipFlash > 0) {
            this.equipFlash--;
        }

        // 冰块缩放动画更新（低温致死时）
        if (this.deathCause === 'cold') {
            this.iceScale = Math.min(this.iceScale + 0.02, 1); // 缩放至1倍后停止
        }
    }

    // 检测角色触发的效果
    checkEffects() {
        // 小孩：上升高度>400触发星球效果
        if (this.type === CHAR_TYPES.CHILD && this.risingHeight > 400) {
            this.deathCause = 'reach_star';
        }
        // 小鸡：上升高度>200展开翅膀
        else if (this.type === CHAR_TYPES.BIRD && this.risingHeight > 200) {
            this.wings = true;
        }
        // 科学家：根据装备和高度触发死亡效果
        else if (this.type === CHAR_TYPES.SCIENTIST) {
            // 为了在有限屏幕高度内更快看到效果，降低触发阈值
            if (this.risingHeight > 200) {
                // 不装备任何装备：冻成冰块
                if (!this.equipped.oxygen && !this.equipped.coat) {
                    this.deathCause = 'cold'; // 无任何装备：冻成冰块
                } else if (!this.equipped.oxygen) {
                    this.deathCause = 'suffocation'; // 无氧气罐但有棉衣：窒息死亡
                } else if (!this.equipped.coat) {
                    this.deathCause = 'cold'; // 有氧气罐无棉衣：低温致死
                } else if (this.risingHeight > 450) {
                    this.deathCause = 'fluid_boil'; // 装备齐全，高度>450：体液沸腾
                } else if (this.risingHeight > 320) {
                    this.deathCause = 'radiation'; // 装备齐全，高度>320：辐射病
                } else if (this.risingHeight > 260) {
                    this.deathCause = 'decompression_sickness'; // 装备齐全，高度>260：减压病
                }
            }
        }
    }

    // 绘制角色
    draw() {
        push(); // 保存绘图状态
        translate(this.x, this.y); // 移至角色坐标中心

        if (this.type === CHAR_TYPES.CHILD) {
            // 如果有小孩图片(XH)则使用，否则使用代码绘制
            if (XH && XH.width > 0) {
                imageMode(CENTER);
                image(XH, 0, 30, 150, 120);
            } else {
                // 绘制小孩简笔画
                stroke(0);
                strokeWeight(2);
                fill(255);
                circle(0, 0, 20); // 头
                line(0, 10, 0, 40); // 身体
                line(0, 20, -15, 30); // 左手
                line(0, 20, 15, 30); // 右手
                line(0, 40, -15, 60); // 左脚
                line(0, 40, 15, 60); // 右脚
            }

            // 绘制星球特效（触发后）
            if (this.deathCause === 'reach_star') {
                fill(255, 255, 0);
                noStroke();
                circle(20, -20, 40); // 光晕
                fill(255, 200, 0);
                circle(20, -20, 30);
                fill(200, 150, 0);
                circle(20, -20, 20);
            }
        } else if (this.type === CHAR_TYPES.BIRD) {
            // 如果有小鸡图片则使用，否则使用代码绘制
            // 扇动翅膀时使用 XJF 图片，否则使用 XJ 图片
            if (this.wings && XJF && XJF.width > 0) {
                // 扇动翅膀时使用 XJF 图片（缩放后大小与XJ图片一致）
                push();
                let wingAngle = Math.sin(this.wingFlap) * 0.2;
                scale(1 + wingAngle); // 翅膀扇动时轻微缩放（0.8-1.2倍）
                imageMode(CENTER);
                image(XJF, 0, 50, 50, 40); // 基础尺寸与XJ一致，缩放后平均大小也是50x40
                pop();
            } else if (!this.wings && XJ && XJ.width > 0) {
                // 未扇动翅膀时使用 XJ 图片
                push();
                imageMode(CENTER);
                image(XJ, 0, 50, 50, 40); // 不扇动时：50x40
                pop();
            } else {
                // 绘制小鸡简笔画（带翅膀扇动缩放）
                push();
                if (this.wings) {
                    let wingAngle = Math.sin(this.wingFlap) * 0.2;
                    scale(1 + wingAngle); // 翅膀扇动时轻微缩放
                }
                fill(255, 200, 0);
                noStroke();
                ellipse(0, 0, 30, 20); // 身体
                circle(10, 0, 16); // 头
                stroke(0);
                strokeWeight(2);
                line(15, 0, 20, -5); // 嘴

                // 绘制翅膀动画（展开后）
                if (this.wings) {
                    let wingAngle = Math.sin(this.wingFlap) * 20;
                    noStroke();
                    arc(-30 - wingAngle, -20, 40, 30, Math.PI, 0); // 左翅膀
                    arc(-wingAngle, -20, 40, 30, Math.PI, 0); // 右翅膀
                }
                pop();
            }
        } else if (this.type === CHAR_TYPES.SCIENTIST) {
            // 绘制冰冻特效（低温致死时）- 如果有BD图片则使用，否则使用代码绘制
            if (this.deathCause === 'cold') {
                if (BD && BD.width > 0) {
                    push();
                    scale(this.iceScale); // 冰块逐渐放大
                    imageMode(CENTER);
                    image(BD, 0, 20, 150, 120);
                    pop();
                } else {
                    push();
                    scale(this.iceScale); // 冰块逐渐放大
                    fill(173, 216, 230, 100);
                    stroke(100, 149, 237);
                    strokeWeight(3);
                    rect(-40, -30, 80, 120, 10); // 冰块矩形（圆角）
                    // 绘制冰晶纹理
                    stroke(240, 248, 255);
                    strokeWeight(1);
                    line(-30, -20, 30, 80);
                    line(-20, -20, 20, 80);
                    line(0, -20, 0, 80);
                    line(20, -20, -20, 80);
                    line(30, -20, -30, 80);
                    pop();
                }
            }

            // 绘制科学家（如果有KXJ图片则使用，否则使用代码绘制）
            if (KXJ && KXJ.width > 0) {
                imageMode(CENTER);
                if (this.deathCause === 'cold') {
                    tint(173, 216, 230, 200); // 冰冻效果：蓝色调
                } else if (this.equipFlash > 0) {
                    tint(255, 255, 0, 255); // 装备闪光：黄色
                } else {
                    noTint();
                }
                image(KXJ, 0, 30, 60, 80);
                noTint();
            } else {
                // 绘制科学家简笔画（低温致死时变蓝）
                let headColor = this.equipFlash > 0 ? color(255, 255, 0) : (this.deathCause === 'cold' ? color(173, 216, 230) : color(0));
                let bodyStroke = this.deathCause === 'cold' ? color(100, 149, 237) : color(0);
                fill(headColor);
                stroke(bodyStroke);
                strokeWeight(2);
                circle(0, 0, 20); // 头
                line(0, 10, 0, 40); // 身体
                line(0, 20, -15, 30); // 左手
                line(0, 20, 15, 30); // 右手
                line(0, 40, -15, 60); // 左脚
                line(0, 40, 15, 60); // 右脚
            }

            // 绘制棉衣装备（如果有MY图片则使用，否则使用代码绘制）
            if (this.equipped.coat) {
                if (MY && MY.width > 0) {
                    imageMode(CENTER);
                    let flashAlpha = this.equipFlash > 0 ? 255 : 200;
                    tint(255, flashAlpha);
                    image(MY, 6, 40, 40, 40);
                    noTint();
                } else {
                    let coatColor = this.equipFlash > 0 ? color(255) : color(0, 150, 255);
                    fill(coatColor);
                    stroke(0);
                    rect(-15, 10, 30, 30);
                }
            }

            // 绘制氧气罐装备（如果有YQG图片则使用，否则使用代码绘制）
            if (this.equipped.oxygen) {
                if (YQG && YQG.width > 0) {
                    imageMode(CENTER);
                    let flashAlpha = this.equipFlash > 0 ? 255 : 200;
                    tint(255, flashAlpha);
                    image(YQG, -12, 70, 15, 20);
                    noTint();
                    strokeWeight(1);
                    stroke(0);
                    line(-12, 40, 0, 10); // 氧气管
                } else {
                    let oxyColor = this.equipFlash > 0 ? color(255) : color(0, 200, 0);
                    fill(oxyColor);
                    stroke(0);
                    rect(-20, 40, 15, 20);
                    strokeWeight(1);
                    line(-12, 40, 0, 10); // 氧气管
                }
            }

            // 额外死亡效果叠加（在装备之后画）
            if (this.deathCause === 'decompression_sickness') {
                // 关节和身体周围不断冒出的气泡
                noStroke();
                for (let i = 0; i < 18; i++) {
                    let ang = random(TWO_PI);
                    let r = random(10, 28);
                    let bx = cos(ang) * r;
                    let by = 20 + sin(ang) * r;
                    fill(173, 216, 230, random(140, 210));
                    circle(bx, by, random(4, 8));
                }
            } else if (this.deathCause === 'radiation') {
                // 绿色辐射光圈 + 射线
                noFill();
                stroke(34, 197, 94, 120);
                strokeWeight(3);
                circle(0, 20, 120);
                stroke(190, 242, 100, 160);
                for (let i = 0; i < 12; i++) {
                    let a = (TWO_PI / 12) * i + frameCount * 0.03;
                    let x1 = cos(a) * 20;
                    let y1 = 20 + sin(a) * 20;
                    let x2 = cos(a) * 55;
                    let y2 = 20 + sin(a) * 55;
                    line(x1, y1, x2, y2);
                }
            } else if (this.deathCause === 'fluid_boil') {
                // 体表沸腾：头和身体外侧布满上浮小气泡
                noStroke();
                for (let i = 0; i < 25; i++) {
                    let ang = random(TWO_PI);
                    let r = random(10, 30);
                    let px = cos(ang) * (r + 5);
                    let py = sin(ang) * (r + 5);
                    fill(248, 250, 252, random(120, 200));
                    circle(px, py, random(3, 7));
                }
            }
        }

        // 绘制死亡提示文字
        if (this.deathCause) {
            let deathMsg = DEATH_TEXT[this.deathCause] || '未知死亡';
            let textColor = this.deathCause === 'reach_star' ? color(0, 255, 0) : color(255, 0, 0);
            noStroke(); // 重置描边状态，避免影响文字
            // 文字阴影
            fill(100);
            textSize(20);
            textAlign(CENTER, CENTER);
            text(deathMsg, 2, -32);
            // 主文字
            fill(textColor);
            text(deathMsg, 0, -30);
        }

        pop(); // 恢复绘图状态
    }
}

// 窗口尺寸变化时，自动让画布充满屏幕
function windowResized() {
    resizeCanvas(windowWidth, windowHeight);
}
