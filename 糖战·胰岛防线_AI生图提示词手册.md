# 《糖战·胰岛防线》AI 生图提示词手册

> **配套文档**：本手册基于《糖战·胰岛防线》游戏策划案编写，为所有角色、敌人、Boss、道具、关卡背景、UI 提供可直接使用的 AI 生图提示词。
> **核心风格**：生物科幻 + 暗黑恐怖 + 人体微观世界（参考：异形、死亡空间、微观人体纪录片）。
> **适用工具**：Midjourney / Stable Diffusion / DALL·E / 即梦 / 文心一格等（英文提示词通用性最佳，附中文说明便于理解与微调）。

---

## 使用说明

1. **英文提示词为主**：多数生图模型对英文响应更精准，每条均提供英文主提示词 + 中文释义。
2. **风格锚点统一**：所有素材建议拼接[全局风格后缀](#零全局风格设定建议固定拼接)，保证整套美术风格一致。
3. **负面提示词**：见[末尾通用负面词](#十二通用负面提示词negative-prompt)，Stable Diffusion 系建议填入；Midjourney 用 `--no` 参数。
4. **参数建议**：纵版游戏背景用竖图（如 `--ar 9:16`），图标/立绘按需调整，见各节标注。
5. **2D 资产建议**：本作为 2D 卷轴射击，提示词中强调 `2D game asset`、`sprite`、`transparent background`、`game UI`，避免出真实照片或 3D 渲染过重。

---

## 零、全局风格设定（建议固定拼接）

把下面这段作为"风格锚"追加到几乎所有提示词后面，保持整套美术统一：

**英文（风格后缀）：**
```
biological sci-fi horror style, dark microscopic human body world, low-key lighting, eerie atmosphere, deep crimson and ink black palette, sickly yellow-green and toxic neon purple accents, wet slimy organic textures, high contrast, cinematic, volumetric haze, film grain, vignette, detailed digital painting, game art
```

**中文释义：** 生物科幻恐怖风、暗黑人体微观世界、低调压抑布光、诡异氛围、暗红与墨黑主色、病态黄绿与毒糖紫点缀、湿润黏腻有机质感、高对比、电影感、体积雾、噪点、暗角、精细数字绘画、游戏美术。

> **配色速查**：主色 `暗红 #4A0E0E / 墨黑 #0A0A0F`；危险高糖色 `毒糖紫 #B026FF / 荧光绿 #39FF14`；金属冷光 `冷蓝 #2CA8FF`。

---

## 一、玩家战机 INS-09

> 纳米战机，象征"外源胰岛素/治疗"。金属冷光与有机环境形成强烈反差——它是这个腐坏世界里唯一干净、锋利的存在。

### 1.1 战机本体（基础形态）

**英文：**
```
2D game sprite of a sleek nano-fighter spaceship called "INS-09", top-down view facing up, syringe-and-crystal inspired design, polished chrome and medical white hull with cold blue glowing energy lines, sharp aerodynamic shape, tiny insulin-crystal core glowing cyan at center, clean futuristic medical-tech machine, sci-fi nanobot, contrasting sharply against organic environment, transparent background, centered, game asset, high detail
```
**中文：** 纵版俯视、机头朝上的精致纳米战机"INS-09"，灵感来自注射器与结晶体，镀铬+医疗白机身配冷蓝发光能量线，锋利气动外形，中心一颗发青色光的胰岛素结晶核心，干净的未来医疗科技机械，与有机环境形成反差，透明背景，居中，游戏素材。
**参数：** `--ar 1:1`，建议透明底。

### 1.2 火力升级形态（Lv1–Lv5 特效）

**英文：**
```
power-up evolution sheet of the nano-fighter INS-09, 5 stages from left to right, increasing energy intensity, more glowing cyan thrusters and weapon emitters added each stage, level 5 surrounded by orbiting insulin-crystal drones and bright energy aura, top-down view, transparent background, game asset sprite sheet
```
**中文：** 战机 5 级火力进化图（从左到右能量递增），每级增加发光推进器与武器发射口，5 级环绕胰岛素结晶僚机与明亮能量光环，俯视，透明背景，素材表。

### 1.3 大招"胰岛素脉冲"特效

**英文：**
```
2D VFX sprite of an "insulin pulse" shockwave, expanding ring of bright cyan and white crystalline energy radiating outward, crystalline shards and glowing particles, screen-clearing nova effect, top-down, transparent background, game effect asset, glowing, high contrast
```
**中文：** "胰岛素脉冲"冲击波特效，向外扩散的青白色结晶能量环，结晶碎片与发光粒子，全屏清场新星效果，俯视，透明背景，游戏特效素材。

---

## 二、剧情角色

### 2.1 AI 向导"瑞吉娜 REGINA"

**英文：**
```
character portrait of "REGINA", a holographic medical AI assistant, semi-transparent glowing female face made of cyan data and light particles, calm and clinical expression, futuristic HUD ring elements around her, dark background, sci-fi medical interface aesthetic, cold blue glow, eerie but elegant, digital painting, game UI character art
```
**中文：** 向导 AI"瑞吉娜"立绘，半透明发光的全息医疗 AI 女性面孔，由青色数据与光粒子构成，冷静临床的表情，周围环绕未来 HUD 环形元素，暗背景，科幻医疗界面美学，冷蓝辉光，诡异而优雅，数字绘画，游戏 UI 角色美术。
**用途：** 对话框头像、简报界面。`--ar 1:1` 或 `3:4`。

### 2.2 患者"林先生"（剧情演出/结局用）

**英文：**
```
cinematic illustration of a 47-year-old overweight man named Mr. Lin lying unconscious in a dark ICU, pale sickly skin, ECG monitor glowing faintly, dramatic low-key lighting, ominous shadows, medical horror atmosphere, muted desaturated colors, sense of crisis, digital painting
```
**中文：** 47 岁超重男性"林先生"昏迷躺在黑暗 ICU 的电影感插画，苍白病态皮肤，微弱发光的心电监护仪，戏剧化低调布光，不祥阴影，医疗恐怖氛围，低饱和，危机感，数字绘画。
**用途：** 序章/结局过场。`--ar 16:9`。

### 2.3 结局——苏醒的病房（好结局）

**英文：**
```
cinematic illustration of Mr. Lin waking up in a hospital bed at dawn, faint warm light through window breaking the darkness, ECG showing steady heartbeat, relief and hope mixed with lingering unease, muted warm tones replacing the previous dark palette, emotional, digital painting
```
**中文：** 林先生黎明在病床上苏醒的电影感插画，窗外微弱暖光打破黑暗，心电图显示平稳心跳，释然与希望中带着隐忧，暖色调取代之前的暗色，情绪化，数字绘画。`--ar 16:9`。

---

## 三、敌人杂兵（共 6 种）

> 全部由真实糖尿病病理转译，造型诡异、危险，使用高饱和"高糖色"以在暗色背景中凸显。

### 3.1 糖晶虫（关一 · 直线冲锋）

**英文：**
```
2D enemy sprite, "sugar crystal worm", a small aggressive creature made of jagged toxic-purple sugar crystals fused with red blood cell flesh, glowing crystalline spikes, menacing, top-down view facing down, transparent background, game asset, biological horror
```
**中文：** 杂兵"糖晶虫"，由毒糖紫尖锐糖结晶与红细胞血肉融合的小型攻击性生物，发光结晶尖刺，凶恶，俯视机头朝下，透明背景。

### 3.2 变异白细胞（关一 · 吐酸弹）

**英文：**
```
2D enemy sprite, "mutated white blood cell", a bloated pale-green amoeba-like cell with too many twitching pseudopods, acidic glowing green pustules, corrupted immune cell, grotesque, top-down, transparent background, game asset, body horror
```
**中文：** 杂兵"变异白细胞"，臃肿苍绿、长着过多抽搐伪足的变形虫状细胞，酸性发光绿脓疱，腐化的免疫细胞，恶心怪诞，俯视，透明背景。

### 3.3 出血点（关二 · 爆裂溅射）

**英文：**
```
2D enemy sprite, "hemorrhage node", a swollen dark-red blood blister about to burst, pulsing veins on surface, leaking droplets, ominous glow, retinal bleeding theme, top-down, transparent background, game asset, horror
```
**中文：** 杂兵"出血点"，即将爆裂的肿胀暗红血疱，表面搏动的血管，渗漏血滴，不祥微光，视网膜出血主题，俯视，透明背景。

### 3.4 异常新生血管（关二 · 缠绕限位）

**英文：**
```
2D enemy sprite, "abnormal neovessel", a writhing tangle of thin twisted blood vessels growing wildly like dark tendrils, glowing red tips, invasive and creepy, top-down, transparent background, game asset, biological horror
```
**中文：** 杂兵"异常新生血管"，疯狂生长、扭曲缠绕如暗色触须的细血管团，发红的尖端，侵略性、瘆人，俯视，透明背景。

### 3.5 高糖孢子（关三 · 自我增殖）

**英文：**
```
2D enemy sprite, "high-glucose spore", a glowing toxic-purple spore pod with sugary crystalline shell, spawning smaller spores around it, infectious and proliferating look, eerie neon glow, top-down, transparent background, game asset
```
**中文：** 杂兵"高糖孢子"，发毒糖紫光、带糖结晶外壳的孢子囊，周围孵化更小孢子，具传染增殖感，诡异霓虹光，俯视，透明背景。

### 3.6 吞噬细胞（关三 · 群体扑咬）

**英文：**
```
2D enemy sprite, "devourer cell", a dark fleshy cell with a gaping toothed maw, dripping necrotic ooze, aggressive predatory look, decayed gray-green flesh, top-down, transparent background, game asset, body horror
```
**中文：** 杂兵"吞噬细胞"，长着张开獠牙巨口的暗色肉质细胞，滴落坏死黏液，掠食性凶相，腐败灰绿血肉，俯视，透明背景。

---

## 四、Boss（共 3 个）

> 体型巨大、多阶段，是每关的视觉与玩法高潮。

### 4.1 Boss 1 — 黏稠之主（高血糖凝块）

**英文：**
```
2D game boss sprite, "Viscous Lord", a massive semi-transparent gelatinous blob of thickened blood and suspended sugar crystals, glowing toxic-purple crystal core inside, oozing slime tendrils, splitting cracks across its body, top-down view facing down, dark crimson palette, biological horror, transparent background, highly detailed, large scale, game boss asset
```
**中文：** Boss"黏稠之主"，巨大半透明的黏稠血液凝块、内悬糖结晶，内部毒糖紫发光晶核，渗出黏液触须，身体布满即将分裂的裂缝，俯视机头朝下，暗红配色，生物恐怖，透明背景，高细节，巨型，Boss 素材。

### 4.2 Boss 2 — 血网之眼（视网膜病变）

**英文：**
```
2D game boss sprite, "Eye of the Blood Web", a gigantic horrifying eyeball covered in burst bleeding micro-vessels forming a red web, dilated pupil as weak point glowing ominously, neovascular tendrils growing around it like a protective shield, retinal nerve background radiating outward, dark and terrifying, top-down view, transparent background, body horror, game boss asset
```
**中文：** Boss"血网之眼"，巨大恐怖眼球，布满破裂出血的微血管织成红网，散大的瞳孔作弱点不祥发光，周围生长如护盾的新生血管触须，放射状视网膜神经背景，黑暗骇人，俯视，透明背景，Boss 素材。

### 4.3 Boss 3 — 胰岛吞噬者（终极 Boss，三阶段）

**英文：**
```
2D game boss sprite sheet, "Islet Devourer", colossal final boss, a monstrous core of corrupted high-glucose matter coiled over a dying pancreatic islet factory, glowing toxic-purple and necrotic green, three phases shown: 1) erosion phase spawning spores, 2) frenzy phase devouring glowing islet cells, 3) collapse phase with exposed bright vulnerable core, biological mechanical decay, terrifying, top-down view, transparent background, epic scale, game boss asset
```
**中文：** 终极 Boss"胰岛吞噬者"素材表，盘踞在垂死胰岛工厂上的巨型高糖怪核，毒糖紫与坏死绿发光，呈现三阶段：①侵蚀期孵孢子 ②狂暴期吞噬发光胰岛细胞 ③崩解期暴露明亮脆弱核心，生物机械腐败，骇人，俯视，透明背景，史诗体量，Boss 素材。
**参数：** `--ar 9:16` 或分阶段分别出图。

---

## 五、道具图标（共 4 种）

> 统一为发光的圆形/六边形图标风格，便于 HUD 与拾取识别。负面道具"高糖陷阱"故意做得诱人。

### 5.1 胰岛素结晶（蓝 · 火力+1）

**英文：**
```
2D game item icon, glowing cyan insulin crystal shard, faceted gem-like, clean medical sci-fi glow, hexagonal frame, transparent background, game UI icon, top-down, high contrast
```
**中文：** 道具图标"胰岛素结晶"，发青色光的结晶碎片，宝石切面，干净的医疗科幻辉光，六边形外框，透明背景，游戏 UI 图标。

### 5.2 膳食纤维核心（绿 · 恢复护盾）

**英文：**
```
2D game item icon, "dietary fiber core", a woven green fibrous energy sphere with healthy natural glow, protective aura, hexagonal frame, transparent background, game UI icon
```
**中文：** 道具图标"膳食纤维核心"，编织状绿色纤维能量球，健康自然辉光，护盾光环，六边形框，透明背景。

### 5.3 运动能量（橙 · 加速/无敌）

**英文：**
```
2D game item icon, "exercise energy", a dynamic orange lightning-bolt energy orb radiating motion lines, vigorous and fast feeling, hexagonal frame, transparent background, game UI icon
```
**中文：** 道具图标"运动能量"，动感的橙色闪电能量球，放射动态线，充满活力速度感，六边形框，透明背景。

### 5.4 高糖陷阱（红 · 负面 · 伪装诱人）

**英文：**
```
2D game item icon, "sugar trap" disguised as a tempting glazed donut / bubble tea, sugary and appealing on the outside but with a subtle ominous red glow and tiny toxic-purple crystals hidden inside, deceptive, transparent background, game UI icon
```
**中文：** 道具图标"高糖陷阱"，伪装成诱人的糖霜甜甜圈/奶茶，外表香甜吸引人，却带隐隐不祥红光与藏在内部的毒糖紫小结晶，具欺骗性，透明背景。

---

## 六、关卡背景（共 3 套，纵版滚动）

> 竖图、可分层（远景/中景/前景视差）。带"心跳搏动"的有机压迫感。

### 6.1 第一关《血潮》— 血管/血液

**英文：**
```
vertical scrolling game background, inside a dark pulsing blood vessel tunnel, deep crimson blood plasma, floating mutated red blood cells, vessel walls coated with yellowish-white glycation scabs and sugar crystal growths, claustrophobic and slimy, eerie red glow, depth and parallax layers, biological horror, seamless tileable top-to-bottom, no characters, game environment art
```
**中文：** 纵版滚动背景，黑暗搏动的血管隧道内部，深红血浆，漂浮变异红细胞，血管壁挂满黄白色糖化结痂与糖结晶增生，幽闭黏腻，诡异红光，纵深视差分层，生物恐怖，上下可无缝拼接，无角色。
**参数：** `--ar 9:16`，强调 `seamless / tileable`。

### 6.2 第二关《盲视》— 眼底/视网膜

**英文：**
```
vertical scrolling game background, inside an eyeball, radiating retinal nerve fibers like a dark web, ruptured micro-vessels spraying red blood mist, floating exudates, ink black and blood red palette with faint nerve-blue glow, terrifying and oppressive, partial vision-obscuring blood haze, depth layers, biological horror, seamless top-to-bottom, no characters, game environment art
```
**中文：** 纵版滚动背景，眼球内部，放射状视网膜神经纤维如暗网，破裂微血管喷出红色血雾，漂浮渗出物，墨黑血红配淡淡神经蓝光，骇人压抑，局部遮挡视野的血雾，分层，生物恐怖，上下无缝，无角色。`--ar 9:16`。

### 6.3 第三关《核心》— 胰腺/胰岛

**英文：**
```
vertical scrolling game background, deep inside a decaying pancreas islet factory, giant half-necrotic cellular machinery, half the structures dead and stopped, faintly glowing surviving islet cells, corrosive mist clouds, sickly yellow-green and necrotic gray palette, collapsing organic debris, final-battle dread, depth layers, biological horror, seamless top-to-bottom, no characters, game environment art
```
**中文：** 纵版滚动背景，腐败的胰腺胰岛工厂深处，巨大的半坏死细胞机械结构，半数已死寂停转，微弱发光的残存胰岛细胞，腐蚀云雾，病态黄绿与坏死灰，崩塌的有机碎块，决战压迫感，分层，生物恐怖，上下无缝，无角色。`--ar 9:16`。

---

## 七、子弹与弹幕

### 7.1 玩家子弹（青蓝色系）

**英文：**
```
2D game bullet sprite set, player projectiles, cyan and white crystalline energy bolts, glowing, sharp, various sizes including homing missiles, transparent background, top-down, game asset sheet
```
**中文：** 玩家子弹素材组，青白色结晶能量弹，发光锋利，含不同大小及追踪导弹，透明背景，俯视，素材表。

### 7.2 敌人弹幕（毒糖紫/血红系）

**英文：**
```
2D game bullet sprite set, enemy bullets, toxic-purple and blood-red glowing orbs and shards, sugar-crystal and blood-droplet shapes, ominous, various sizes for danmaku patterns, transparent background, top-down, game asset sheet
```
**中文：** 敌人弹幕素材组，毒糖紫与血红发光弹与碎片，糖结晶与血滴造型，不祥，多种尺寸供弹幕组合，透明背景，俯视，素材表。

---

## 八、UI 套件

> 整体为"ICU 医疗监护仪"风：半透明深色面板 + 荧光描边 + 科技/等宽字体。

### 8.1 HUD（游戏内界面）

**英文：**
```
2D game HUD UI kit, ICU medical monitor aesthetic, dark semi-transparent panels with cyan neon outlines, health/shield bar styled as vital signs, bottom ECG heartbeat progress bar, score counter, ultimate energy gauge, sci-fi medical interface, futuristic monospace tech font, clean game UI overlay, transparent background
```
**中文：** 游戏 HUD UI 套件，ICU 监护仪美学，半透明深色面板配青色霓虹描边，生命/护盾条如生命体征，底部心电图心跳进度条，得分计数，大招能量槽，科幻医疗界面，未来等宽科技字体，干净的 UI 叠层，透明背景。

### 8.2 主菜单界面

**英文：**
```
game main menu screen, ICU monitor style UI floating over a dark microscopic human-body background with faint pulsing blood vessels, menu buttons: Start / Level Select / Difficulty / Medical Archive / Settings, cyan neon on dark glass panels, ominous sci-fi medical horror atmosphere, game UI design, vertical layout
```
**中文：** 游戏主菜单界面，ICU 监护仪风 UI 浮在暗黑人体微观背景（隐隐搏动的血管）之上，菜单项：开始/选关/难度/医学档案库/设置，暗色玻璃面板配青色霓虹，不祥的科幻医疗恐怖氛围，UI 设计，竖版布局。

### 8.3 难度选择界面

**英文：**
```
difficulty selection UI screen, three options styled as medical risk levels: "Easy / Normal / Hard" shown like rising blood-glucose warning levels (green-safe to red-danger), ICU monitor aesthetic, dark glass panels with neon, sci-fi medical UI, game design
```
**中文：** 难度选择界面，三个选项做成医疗风险等级"简单/普通/困难"，如血糖警示等级递增（绿色安全到红色危险），ICU 监护仪美学，暗色玻璃面板配霓虹，科幻医疗 UI。

### 8.4 科普"病理档案"卡片

**英文：**
```
game popup card UI, "Pathology Archive" knowledge card, dark glass panel with cyan neon medical-file frame, header with file number and title, area for a creepy biological illustration of the defeated boss, body text area for diabetes facts, a confirm button, ICU monitor aesthetic, clean readable sci-fi medical UI, game design
```
**中文：** 游戏弹出卡片 UI"病理档案"科普卡，暗色玻璃面板配青色霓虹的医疗档案外框，顶部档案编号与标题，中部放被击败 Boss 的诡异生物插画区，下部糖尿病知识正文区，确认按钮，ICU 监护仪美学，干净易读的科幻医疗 UI。

### 8.5 结算界面

**英文：**
```
level results screen UI, ICU monitor style, displays score / kills / collection rate / time bonus, with a vital-signs theme, recap of the level's knowledge card, dark glass panels with cyan neon, sci-fi medical horror aesthetic, game UI design
```
**中文：** 关卡结算界面，ICU 监护仪风，显示得分/击杀/收集率/时间奖励，生命体征主题，本关科普卡回顾，暗色玻璃面板配青色霓虹，科幻医疗恐怖美学，UI 设计。

### 8.6 通用 UI 按钮/图标组件

**英文：**
```
2D game UI component kit, buttons / toggles / sliders / icon set, ICU medical monitor style, dark glass with cyan neon edges, glowing hover states, sci-fi medical theme, transparent background, clean vector-like game UI assets
```
**中文：** 游戏 UI 组件套件，按钮/开关/滑块/图标组，ICU 监护仪风，暗玻璃配青色霓虹边，发光悬停态，科幻医疗主题，透明背景，干净的矢量感 UI 素材。

---

## 九、游戏 Logo / 标题

**英文：**
```
game logo design, title "Glucose War: The Last Islet" (with Chinese subtitle "糖战·胰岛防线"), metallic sci-fi medical typography fused with biohazard and sugar-crystal motifs, cyan and toxic-purple glow against deep black, dripping organic texture on letters, ominous and epic, game key art logo, transparent background
```
**中文：** 游戏 Logo 设计，标题"Glucose War: The Last Islet"（含中文副标题"糖战·胰岛防线"），金属科幻医疗字体融合生化危害与糖结晶元素，青色与毒糖紫辉光衬深黑底，字母带滴落有机质感，不祥而史诗，游戏主视觉 Logo，透明背景。

---

## 十、宣传主视觉 / 封面（可选）

**英文：**
```
game key art poster, the nano-fighter INS-09 glowing cyan, flying through a dark terrifying blood vessel tunnel, swarms of toxic-purple sugar monsters and a giant boss looming in the depths, dramatic cinematic lighting, deep crimson and ink black with neon accents, biological sci-fi horror, epic and ominous, vertical poster composition, highly detailed digital painting
```
**中文：** 游戏主视觉海报，发青光的纳米战机 INS-09 穿越黑暗骇人的血管隧道，成群毒糖紫高糖怪物与深处隐现的巨型 Boss，戏剧化电影布光，暗红墨黑配霓虹点缀，生物科幻恐怖，史诗不祥，竖版海报构图，高细节数字绘画。`--ar 2:3`。

---

## 十一、生成参数建议（Midjourney 示例）

| 用途 | 比例参数 | 备注 |
| --- | --- | --- |
| 关卡背景（纵版） | `--ar 9:16` | 强调 seamless/tileable，便于滚动拼接 |
| 角色立绘 | `--ar 3:4` 或 `1:1` | 头像用 1:1 |
| 战机/敌人/Boss 素材 | `--ar 1:1` | 透明底，俯视 |
| 道具/UI 图标 | `--ar 1:1` | 透明底 |
| 主视觉海报 | `--ar 2:3` | 竖版 |
| 横版过场插画 | `--ar 16:9` | 剧情演出 |

> Midjourney 可加 `--style raw` 提升写实可控性；统一加 `--s 250` 左右控制风格化程度。SD 系建议固定同一基础模型/LoRA + 同一随机种子区间，保证全套风格一致。

---

## 十二、通用负面提示词（Negative Prompt）

> SD 系填入负面框；Midjourney 用 `--no` 跟关键词。

**英文：**
```
text, watermark, signature, logo (unless logo asset), blurry, low quality, jpeg artifacts, deformed, extra limbs, bad anatomy, cartoonish cute, bright cheerful colors, pastel, daylight, photo of real people, 3D render plastic look, oversaturated, messy background (for sprites), opaque background (for assets needing transparency), realistic medical photograph, gore excessive blood splatter (keep stylized)
```
**中文释义：** 文字、水印、签名、（非 Logo 时的）标志、模糊、低质、JPEG 噪点、畸形、多肢、解剖错误、可爱卡通、明快欢乐配色、马卡龙色、日光、真人照片、塑料感 3D 渲染、过饱和、（素材图的）杂乱背景、（需透明素材的）不透明背景、写实医学照片、过度血腥（保持风格化）。

---

## 十三、风格一致性实操建议

1. **先定一张"风格基准图"**：先把[全局风格后缀](#零全局风格设定建议固定拼接) + 一个主体（如战机或第一关背景）反复出图，选定一张满意的作为基准。
2. **复用种子/参考图**：MJ 用 `--sref`（风格参考）或 `--cref`（角色参考）锁定；SD 用相同 checkpoint + LoRA + 固定 seed。
3. **统一后处理**：所有图最后过一遍统一的暗角、噪点、色调（可在 Unity URP 后处理里全局加，省去逐张处理）。
4. **分层导出**：背景尽量出"无角色"的纯环境图，角色/敌人单独出透明底素材，方便在 Unity 中分层组合与做视差。
5. **敌我配色纪律**：玩家方一律青蓝冷光，敌方/危险一律毒糖紫+血红，玩家可凭颜色瞬间判断敌我与威胁——这条务必在所有提示词中坚持。

---

*文档版本：v1.0 ｜ 配套《糖战·胰岛防线》游戏策划案 ｜ 用途：AI 生图提示词参考*
