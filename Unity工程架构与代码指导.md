# 《糖战·胰岛防线》Unity 工程架构与代码指导文件

> **说明**：本文件只提供**工程结构、架构设计、模块职责、数据流与开发规范的指导**，**不包含具体代码实现**。开发者据此即可独立搭建工程并编码。
> **技术栈**：Unity 2022 LTS · URP(2D Renderer) · Input System · UGUI · ScriptableObject 数据驱动 · 对象池。

---

## 一、技术选型与工程设置

### 1.1 版本与管线

| 项 | 选择 | 说明 |
| --- | --- | --- |
| Unity | 2022 LTS | 稳定，长期支持 |
| 渲染管线 | URP（2D Renderer） | 便于 2D 后处理（泛光/暗角/噪点）营造恐怖氛围 |
| 输入 | Input System（新版） | 统一键盘/手柄/触屏 |
| UI | UGUI（Canvas） | 上手快，够用；如团队熟悉可用 UI Toolkit |
| 物理 | 2D（Trigger 为主） | STG 用简化命中判定，少用刚体动力学 |
| 序列化/配置 | ScriptableObject + CSV 导入 | 数据驱动，策划可调 |
| 存档 | PlayerPrefs / JSON | 进度、解锁、排行榜 |
| 版本管理 | Git + Git LFS | LFS 管理美术资产 |

### 1.2 关键工程配置建议

- **PPU（Pixels Per Unit）**：全项目统一为 100，所有美术按此规范导入。
- **分辨率**：基准 1080×1920（纵版），Canvas 用 `CanvasScaler = Scale With Screen Size`。
- **物理层（Layer）**：`Player`、`PlayerBullet`、`Enemy`、`EnemyBullet`、`Item`、`Boss`、`Environment`。在 Physics2D 碰撞矩阵中只勾选必要的层间碰撞（如 PlayerBullet×Enemy、EnemyBullet×Player），减少无效检测。
- **Tag**：`Player`、`Enemy`、`Boss`、`Item`。
- **Time**：用 `Time.timeScale` 实现暂停；游戏逻辑用 `Time.deltaTime` 保证帧率无关。
- **后处理**：URP 全局 Volume 统一加 暗角(Vignette) + 泛光(Bloom) + 胶片噪点(Film Grain) + 色调，所有美术不必逐张处理。

---

## 二、工程目录结构（建议）

```
Assets/
├── Art/                      # 美术资产
│   ├── Player/  Enemies/  Boss/  Items/  Bullets/
│   ├── BG/L1/  BG/L2/  BG/L3/
│   ├── VFX/  UI/  Story/
├── Audio/                    # BGM/  SFX/  UI/
├── Prefabs/                  # 预制体
│   ├── Player/  Enemies/  Boss/  Bullets/  Items/  VFX/  UI/
├── ScriptableObjects/        # 所有数据配置（核心）
│   ├── Enemies/              # EnemyData 资产
│   ├── BulletPatterns/       # 弹幕参数资产
│   ├── Levels/               # 关卡时间轴/波次资产
│   ├── Difficulty/           # 难度配置资产
│   ├── Items/                # 道具配置
│   └── Knowledge/            # 科普卡片资产
├── Scripts/
│   ├── Core/                 # GameManager/事件/对象池/单例基类
│   ├── Player/               # 玩家相关
│   ├── Enemies/              # 敌人与AI
│   ├── Bullets/              # 子弹与弹幕
│   ├── Boss/                 # Boss状态机
│   ├── Level/                # 关卡/生成/背景
│   ├── Items/                # 道具与升级
│   ├── Difficulty/           # 难度系统
│   ├── Knowledge/            # 科普系统
│   ├── UI/                   # UI
│   ├── Audio/                # 音频
│   ├── Save/                 # 存档
│   ├── Data/                 # ScriptableObject 数据类定义
│   └── Editor/               # 编辑器工具（CSV导入等）
├── Scenes/                   # Boot/ MainMenu/ Level1..3/
├── Settings/                 # URP/Input/AudioMixer 配置
└── Resources/ (按需)
```

---

## 三、整体架构总览

### 3.1 分层结构

```
┌───────────────────────────────────────────────┐
│  表现层  UI / VFX / Audio / 背景滚动            │
├───────────────────────────────────────────────┤
│  玩法层  Player / Enemy / Boss / Bullet / Item  │
├───────────────────────────────────────────────┤
│  流程层  GameManager / LevelManager / Spawn     │
├───────────────────────────────────────────────┤
│  数据层  ScriptableObject(敌人/弹幕/关卡/难度/科普) │
├───────────────────────────────────────────────┤
│  基础设施  ObjectPool / EventBus / Save / Input  │
└───────────────────────────────────────────────┘
```

### 3.2 设计原则

1. **数据驱动**：敌人、弹幕、关卡波次、难度、科普全部用 ScriptableObject 配置；改内容不改代码。
2. **事件解耦**：玩法层通过事件总线通知 UI/音频，避免互相直接引用。
3. **对象池优先**：子弹、敌人、特效一律池化，杜绝运行时频繁 `Instantiate/Destroy`。
4. **单一职责**：每个类只做一件事，便于 1-3 人并行开发。
5. **帧率无关**：所有移动/计时用 `Time.deltaTime`。

---

## 四、核心模块职责说明（不含代码）

### 4.1 基础设施（Scripts/Core）

| 类（建议名） | 职责 | 关键点 |
| --- | --- | --- |
| `Singleton<T>` | 通用单例基类 | 供 Manager 继承 |
| `GameManager` | 全局状态机（Boot/Menu/Playing/Paused/Result）、难度传递、场景切换 | 跨场景常驻 |
| `EventBus` | 全局事件分发（发布/订阅） | 解耦 UI 与玩法；事件如 `OnPlayerDamaged`、`OnEnemyKilled`、`OnLevelCleared` |
| `ObjectPool` | 通用对象池（按预制体分桶，取/还） | 子弹/敌人/特效共用；预热常用对象 |

### 4.2 玩家（Scripts/Player）

| 类 | 职责 |
| --- | --- |
| `PlayerController` | 读取输入→移动（含屏幕边界限制与平滑） |
| `PlayerWeapon` | 自动开火、火力等级 Lv1-5 弹道切换、从池取子弹 |
| `PlayerHealth` | 生命/护盾/受伤/无敌帧/死亡，发受伤与死亡事件 |
| `UltimateSkill` | 胰岛素脉冲：能量积累、全屏清弹、范围伤害、冷却 |
| `PlayerHitbox` | 精准命中盒（判定盒大小由难度注入） |

### 4.3 敌人与弹幕（Scripts/Enemies, Scripts/Bullets）

| 类 | 职责 |
| --- | --- |
| `EnemyBase` | 敌人基类：读取 `EnemyData`，管理血量/移动/开火/死亡/掉落/科普解锁 |
| `IMovePattern`（接口）+ 各实现 | 移动模式：直线/跟踪/正弦/缠绕/静止，可组合 |
| `BulletSpawner` | 按 `BulletPatternData` 发射弹幕（角度/数量/速度/扩散/连射） |
| `Bullet` | 子弹生命周期、移动、命中检测、出屏回收（敌我分层） |
| `SporeEnemy`（特例） | 高糖孢子增殖逻辑（关三） |

### 4.4 Boss（Scripts/Boss）

| 类 | 职责 |
| --- | --- |
| `BossController` | 多阶段状态机（按血量/时间切阶段），统一框架供 3 个 Boss 复用 |
| `BossPhase`（数据/状态） | 每阶段的弹幕、行为、可受伤部位、转换条件 |
| 各 Boss 行为脚本 | Boss1 分裂 / Boss2 致盲+护盾 / Boss3 三阶段+保护点+限时 |
| `BossHealthUI` 联动 | 通过事件更新 Boss 血条与阶段演出 |

### 4.5 关卡与生成（Scripts/Level）

| 类 | 职责 |
| --- | --- |
| `LevelManager` | 关卡流程：导入→推进→Boss→通关→结算；维护关卡时间 |
| `SpawnSystem` | 读取关卡波次数据，按"触发时间/事件"从池生成敌人 |
| `WaveData / LevelTimeline`（SO） | 波次配置数据（与 CSV 配置表一一对应） |
| `BackgroundScroller` | 多层视差背景滚动；支持无缝拼接 |
| `EnvironmentHazard` | 环境危害：收缩血管/血雾遮挡/腐蚀伤害区 |

### 4.6 道具与数值（Scripts/Items）

| 类 | 职责 |
| --- | --- |
| `PowerUp` + `ItemData`(SO) | 4 种道具拾取与效果；负面"高糖陷阱" |
| `ScoreManager` | 计分公式（击杀/收集/时间/难度系数/受击扣分） |
| `DropTable` | 按敌人/难度掉落道具 |

### 4.7 难度（Scripts/Difficulty）

| 类 | 职责 |
| --- | --- |
| `DifficultyData`(SO) | 简单/普通/困难全参数（生命/弹幕密度/弹速/血量/掉落/命中盒/大招冷却/存档点） |
| `DifficultyService` | 全局提供当前难度参数，被各战斗模块查询/注入 |
| `DynamicDifficulty`（可选） | 连死降密度的 DDA，带开关 |

### 4.8 科普（Scripts/Knowledge）

| 类 | 职责 |
| --- | --- |
| `KnowledgeCard`(SO) | 标题/正文/配图/触发条件/对应敌人ID |
| `KnowledgeManager` | 通关弹卡、死亡随机提示、图鉴解锁记录 |
| `ArchiveUI` | 医学档案库浏览界面 |

### 4.9 UI / 音频 / 存档

| 类 | 职责 |
| --- | --- |
| `UIManager` | 界面栈管理（菜单/HUD/暂停/结算/卡片/档案库） |
| `HUDView` | 订阅事件刷新 生命/护盾/大招/得分/心电图进度条 |
| `AudioManager` | BGM/SFX/UI 分组（AudioMixer），低血量警报 |
| `SaveSystem` | 进度/解锁/排行榜持久化 |

---

## 五、ScriptableObject 数据结构指导（字段建议，不写代码）

> 以下为各配置资产应包含的**字段清单**，开发者据此定义 SO 类。

### 5.1 EnemyData（敌人）

`敌人ID / 显示名 / 预制体引用 / 最大血量 / 移动速度 / 移动模式枚举 / 弹幕配置引用(BulletPatternData) / 开火间隔 / 分值 / 掉落表引用 / 关联科普ID / 击中半径`

### 5.2 BulletPatternData（弹幕）

`弹道类型(单发/扇形/环形/追踪) / 子弹预制体 / 子弹数量 / 起始角度 / 扩散角度 / 子弹速度 / 连射次数 / 连射间隔 / 伤害`

### 5.3 WaveData / LevelTimeline（关卡波次）

`关卡ID / 波次列表[ {波次ID, 触发时间, 段落, 敌人ID, 数量, 生成位置, 生成间隔, 移动覆盖, 弹幕覆盖} ] / Boss引用 / 关卡时长 / 背景引用 / 通关科普ID`
> 与《关卡波次配置表.csv》列一一对应，经 Editor 工具导入。

### 5.4 DifficultyData（难度）

`难度名 / 玩家初始生命 / 弹幕密度系数 / 子弹速度系数 / 敌人血量系数 / 掉落率系数 / 大招冷却系数 / 命中盒系数 / 存档点策略 / 科普卡片是否强制阅读 / 是否启用DDA`

### 5.5 ItemData（道具）

`道具ID / 类型(火力/护盾/加速/负面) / 图标 / 效果数值 / 持续时间 / 拾取音效`

### 5.6 KnowledgeCard（科普）

`卡片编号 / 标题 / 正文 / 插画 / 触发类型(通关/死亡/图鉴) / 关联关卡或敌人ID`

---

## 六、数据流与典型时序（说明）

### 6.1 关卡启动流程

```
GameManager(选难度→加载关卡场景)
  → LevelManager.Init(读取 LevelTimeline + 注入 DifficultyData)
  → BackgroundScroller 启动
  → SpawnSystem 按时间轴推进
      └ 到达波次触发时间 → 从 ObjectPool 取敌人 → 按 EnemyData 初始化(难度系数已注入)
  → 到达 Boss 段 → LevelManager 激活 BossController
  → Boss 死亡事件 → LevelManager 触发"通关"
  → KnowledgeManager 弹病理档案卡 → ScoreManager 结算 → UIManager 显示结算
```

### 6.2 一次击杀的事件链

```
PlayerBullet 命中 Enemy
  → Enemy 扣血；死亡时：EventBus.Publish(OnEnemyKilled)
      ├ ScoreManager 加分
      ├ DropTable 概率生成道具(从池)
      ├ UltimateSkill 充能
      ├ VFX 播放爆炸(从池)
      └ KnowledgeManager 记录图鉴解锁
  → Enemy 归还对象池
```

### 6.3 难度注入路径

```
主菜单选难度 → GameManager.CurrentDifficulty = DifficultyData
  → 各模块启动时向 DifficultyService 查询系数：
      PlayerHealth(初始生命) / BulletSpawner(密度·弹速) / EnemyBase(血量) /
      DropTable(掉落率) / UltimateSkill(冷却) / PlayerHitbox(判定盒) / LevelManager(存档点)
```

---

## 七、CSV → ScriptableObject 导入工具指导（Scripts/Editor）

> 这是 2 周提效的关键工具，建议第一周优先做（对应程序任务 T-32）。

- **目的**：策划在《关卡波次配置表.csv》中填表，一键导入生成/更新 `LevelTimeline` SO，无需改代码。
- **思路**：
  1. 编辑器菜单项（如 `Tools/导入波次配置`）。
  2. 读取指定 CSV（UTF-8），按表头解析每行为一个波次条目。
  3. 将"敌人类型"列的字符串映射到对应 `EnemyData` 资产（按 ID 建字典查找）。
  4. 按"关卡"分组，写入/更新对应关卡的 `LevelTimeline` 资产并保存。
- **校验**：导入时检查敌人ID是否存在、时间是否递增、数值是否合法，报错定位到行号。
- **同理可做**：敌人属性表、Boss 配置表、难度系数表的导入。

---

## 八、命名与编码规范

- **类名**：PascalCase（`PlayerController`）；**字段/局部变量**：camelCase；**常量**：UPPER_CASE / PascalCase。
- **私有序列化字段**：`[SerializeField] private` + camelCase，便于 Inspector 调参又不暴露 public。
- **一个文件一个类**，文件名=类名。
- **避免 Update 滥用**：能用事件/协程的不轮询；大量对象考虑统一管理器集中 tick。
- **禁止运行时 Instantiate 子弹/敌人/特效**：一律走对象池。
- **MonoBehaviour 只做表现与生命周期**，纯逻辑/数据尽量放普通类或 SO，便于测试。
- **注释**：只解释"为什么"与非显而易见的约束，不写复述代码的废话注释。

---

## 九、场景组织建议

| 场景 | 内容 |
| --- | --- |
| `Boot` | 初始化全局 Manager（GameManager/Audio/Save），显示免责声明，跳主菜单 |
| `MainMenu` | 主菜单/选关/难度/档案库/设置 |
| `Level1/2/3` | 各关卡（或用一个通用关卡场景 + 关卡数据切换，更省事） |

> **省事方案**：只做 1 个 `Gameplay` 场景，通过加载不同 `LevelTimeline` 数据切换关卡内容，避免重复搭场景。推荐 2 周项目采用此方案。

---

## 十、性能与质量检查清单

- [ ] 子弹/敌人/特效全部对象池化，运行时无明显 GC 峰值。
- [ ] Physics2D 碰撞矩阵只勾必要层。
- [ ] Sprite 打成 Sprite Atlas，控制 Draw Call。
- [ ] 同屏子弹数设上限，超出不再生成。
- [ ] 背景用分层条带平铺，避免超大贴图。
- [ ] 目标设备稳定 60 FPS。
- [ ] 三档难度均经手测可通（困难档弹幕有解）。

---

## 十一、推荐开发顺序（与任务清单对应）

1. 工程基础 + 对象池 + 输入（T-01~T-06）。
2. 玩家移动/开火/受伤（T-10~T-12）→ 先有"能动能打"。
3. 敌人基类 + 弹幕器 + 子弹（T-20~T-23）→ 有靶子。
4. **CSV 导入工具（T-32）** → 打通数据流。
5. 生成系统 + 背景 + 第一关 Boss（T-31,T-33,T-40,T-41）→ 第一关闭环。
6. 复制扩展第二、三关 + Boss → 内容铺开。
7. 难度 + 科普 + UI + 音频 → 系统完整。
8. 平衡 + 修 Bug + 打包。

---

*文档版本：v1.0 ｜ Unity 工程架构与代码指导（仅架构指导，不含具体代码实现）*
