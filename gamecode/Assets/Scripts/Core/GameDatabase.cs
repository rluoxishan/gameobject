using System.Collections.Generic;
using GlucoseWar.Data;
using UnityEngine;

namespace GlucoseWar.Core
{
    /// <summary>
    /// 运行时数据库：用《敌人属性表/Boss配置表/难度系数表/关卡波次配置表》的数值
    /// 在内存中构建所有 ScriptableObject 实例，使游戏无需手动制作 .asset 即可运行。
    /// 策划仍可在 Project 中创建同类 SO 资产并通过 CSV 导入工具覆盖。
    /// </summary>
    public static class GameDatabase
    {
        private static bool built;

        public static readonly Dictionary<string, EnemyData> Enemies = new Dictionary<string, EnemyData>();
        public static readonly Dictionary<string, BossData> Bosses = new Dictionary<string, BossData>();
        public static readonly Dictionary<Difficulty, DifficultyData> Difficulties = new Dictionary<Difficulty, DifficultyData>();
        public static readonly Dictionary<ItemType, ItemData> Items = new Dictionary<ItemType, ItemData>();
        public static readonly Dictionary<string, KnowledgeCard> Knowledge = new Dictionary<string, KnowledgeCard>();
        public static readonly List<LevelTimeline> Levels = new List<LevelTimeline>();

        public static void EnsureBuilt()
        {
            if (built) return;
            built = true;
            BuildItems();
            BuildEnemies();
            BuildBosses();
            BuildDifficulties();
            BuildKnowledge();
            BuildLevels();
        }

        private static readonly Color PoisonPurple = new Color(0.65f, 0.2f, 0.85f);
        private static readonly Color BloodRed = new Color(0.85f, 0.12f, 0.18f);

        private static BulletPatternData Pattern(PatternType type, int count, float spread, float speed, float interval, int burst, Color color)
        {
            var p = ScriptableObject.CreateInstance<BulletPatternData>();
            p.type = type; p.bulletCount = count; p.spreadAngle = spread; p.bulletSpeed = speed;
            p.burstCount = burst; p.burstInterval = interval; p.color = color;
            p.startAngle = 270f; p.damage = 1;
            return p;
        }

        private static void BuildItems()
        {
            Items[ItemType.Insulin] = MakeItem(ItemType.Insulin, "胰岛素结晶", 1, 0,
                new Color(0.17f, 0.66f, 1f), ProceduralShapeKind.Hexagon);
            Items[ItemType.Fiber] = MakeItem(ItemType.Fiber, "膳食纤维核心", 30, 0,
                new Color(0.22f, 1f, 0.08f), ProceduralShapeKind.Circle);
            Items[ItemType.Energy] = MakeItem(ItemType.Energy, "运动能量", 1.5f, 5f,
                new Color(1f, 0.55f, 0.1f), ProceduralShapeKind.Diamond);
            Items[ItemType.SugarTrap] = MakeItem(ItemType.SugarTrap, "高糖陷阱", 1, 4f,
                new Color(0.9f, 0.15f, 0.2f), ProceduralShapeKind.Ring);
        }

        private static ItemData MakeItem(ItemType t, string name, float val, float dur, Color c, ProceduralShapeKind shape)
        {
            var item = ScriptableObject.CreateInstance<ItemData>();
            item.type = t; item.displayName = name; item.effectValue = val; item.duration = dur;
            item.color = c; item.shape = shape;
            return item;
        }

        private static EnemyData MakeEnemy(string id, string name, string level, int hp, float spd, int score,
            MoveMode move, BulletPatternData pattern, float fire, float radius, string kid, string path,
            Color color, ProceduralShapeKind shape, float size)
        {
            var e = ScriptableObject.CreateInstance<EnemyData>();
            e.enemyId = id; e.displayName = name; e.level = level; e.maxHealth = hp; e.moveSpeed = spd;
            e.score = score; e.moveMode = move; e.bulletPattern = pattern; e.fireInterval = fire;
            e.hitRadius = radius; e.knowledgeId = kid; e.pathology = path;
            e.bodyColor = color; e.shape = shape; e.visualSize = size;
            Enemies[id] = e;
            return e;
        }

        private static void BuildEnemies()
        {
            MakeEnemy("E01", "糖晶虫", "关一", 10, 4.0f, 100, MoveMode.StraightDown, null, -1f, 0.30f,
                "K01", "血糖颗粒", new Color(0.95f, 0.85f, 0.35f), ProceduralShapeKind.Diamond, 0.55f);

            MakeEnemy("E02", "变异白细胞", "关一", 22, 2.0f, 200, MoveMode.SineHorizontal,
                Pattern(PatternType.Fan3, 3, 40f, 3f, 0f, 1, PoisonPurple), 2.2f, 0.40f,
                "K03", "免疫紊乱", new Color(0.85f, 0.85f, 0.95f), ProceduralShapeKind.Circle, 0.75f);

            MakeEnemy("E03", "出血点", "关二", 18, 1.0f, 180, MoveMode.StaticBurst,
                Pattern(PatternType.DeathSplash, 10, 360f, 4f, 0f, 1, BloodRed), 99f, 0.45f,
                "K04", "微血管出血", new Color(0.7f, 0.05f, 0.1f), ProceduralShapeKind.Circle, 0.6f);

            MakeEnemy("E04", "异常新生血管", "关二", 30, 1.5f, 220, MoveMode.WrapLimit,
                Pattern(PatternType.DoubleStraight, 2, 8f, 3.2f, 0f, 1, BloodRed), 2.5f, 0.50f,
                "K04", "新生血管病变", new Color(0.6f, 0.1f, 0.25f), ProceduralShapeKind.Triangle, 0.8f);

            var spore = MakeEnemy("E05", "高糖孢子", "关三", 15, 1.2f, 150, MoveMode.FloatSpread,
                Pattern(PatternType.Single, 1, 0f, 3f, 0f, 1, PoisonPurple), 3.0f, 0.40f,
                "K07", "病情拖延扩散", new Color(0.55f, 0.85f, 0.35f), ProceduralShapeKind.Circle, 0.6f);
            spore.isSpore = true;

            MakeEnemy("E06", "吞噬细胞", "关三", 25, 2.5f, 200, MoveMode.SwarmRush, null, -1f, 0.45f,
                "K05", "组织被侵蚀", new Color(0.4f, 0.25f, 0.45f), ProceduralShapeKind.Hexagon, 0.75f);

            var el1 = MakeEnemy("ELITE1", "血栓核", "关一", 120, 0f, 800, MoveMode.StaticSummon,
                Pattern(PatternType.Ring8, 5, 360f, 2.8f, 0f, 1, BloodRed), 3.5f, 0.70f,
                "K02", "血栓阻塞", new Color(0.5f, 0.05f, 0.1f), ProceduralShapeKind.Hexagon, 1.4f);
            el1.isElite = true; el1.summonEnemyId = "E01"; el1.summonInterval = 2.5f;

            var el2 = MakeEnemy("ELITE2", "出血团", "关二", 140, 0.5f, 900, MoveMode.Drift,
                Pattern(PatternType.Ring12, 7, 360f, 2.8f, 0f, 1, BloodRed), 3.0f, 0.75f,
                "K04", "大面积出血", new Color(0.55f, 0.04f, 0.12f), ProceduralShapeKind.Circle, 1.5f);
            el2.isElite = true;

            var el3 = MakeEnemy("ELITE3", "孢子母巢", "关三", 160, 0f, 1000, MoveMode.StaticSummon,
                Pattern(PatternType.Ring16, 9, 360f, 2.8f, 0f, 1, PoisonPurple), 3.2f, 0.80f,
                "K07", "病灶增殖中心", new Color(0.35f, 0.55f, 0.2f), ProceduralShapeKind.Hexagon, 1.6f);
            el3.isElite = true; el3.summonEnemyId = "E05"; el3.summonInterval = 2.2f;
        }

        private static BossPhase Phase(string name, float hpEnter, PatternType pat, float fire, string mech, string kid)
        {
            return new BossPhase { phaseName = name, hpFractionEnter = hpEnter, pattern = pat, fireInterval = fire, mechanic = mech, knowledgeId = kid };
        }

        private static void BuildBosses()
        {
            var b1 = ScriptableObject.CreateInstance<BossData>();
            b1.bossId = "B01"; b1.displayName = "黏稠之主"; b1.level = "关一"; b1.totalHealth = 350; b1.knowledgeId = "K02";
            b1.bodyColor = new Color(0.5f, 0.08f, 0.12f); b1.visualSize = 2.6f;
            b1.phases.Add(Phase("完整态", 1.0f, PatternType.BossSpread, 1.1f, "黏液减速玩家", "K02"));
            b1.phases.Add(Phase("分裂态", 0.5f, PatternType.BossSpread, 0.8f, "半血分裂成多个小凝块", "K02"));
            Bosses["B01"] = b1;

            var b2 = ScriptableObject.CreateInstance<BossData>();
            b2.bossId = "B02"; b2.displayName = "血网之眼"; b2.level = "关二"; b2.totalHealth = 450; b2.knowledgeId = "K04";
            b2.bodyColor = new Color(0.45f, 0.04f, 0.1f); b2.visualSize = 3.0f;
            b2.phases.Add(Phase("护盾态", 1.0f, PatternType.BossRadial, 1.2f, "新生血管护盾需先打断", "K04"));
            b2.phases.Add(Phase("暴露态", 0.6f, PatternType.BossRadial, 0.7f, "周期性流血遮挡视野", "K04"));
            Bosses["B02"] = b2;

            var b3 = ScriptableObject.CreateInstance<BossData>();
            b3.bossId = "B03"; b3.displayName = "胰岛吞噬者"; b3.level = "关三"; b3.totalHealth = 650; b3.knowledgeId = "K08";
            b3.bodyColor = new Color(0.35f, 0.2f, 0.45f); b3.visualSize = 3.4f;
            b3.phases.Add(Phase("侵蚀期", 1.0f, PatternType.BossDense, 1.0f, "持续召唤高糖孢子", "K08"));
            b3.phases.Add(Phase("狂暴期", 0.66f, PatternType.BossDense, 0.7f, "高速密集弹幕", "K08"));
            b3.phases.Add(Phase("崩解期", 0.33f, PatternType.BossDense, 0.5f, "限时集火配合大招", "K08"));
            Bosses["B03"] = b3;
        }

        private static void BuildDifficulties()
        {
            var easy = ScriptableObject.CreateInstance<DifficultyData>();
            easy.difficulty = Difficulty.Easy; easy.displayName = "简单·科普模式"; easy.playerStartLives = 6;
            easy.bulletDensityFactor = 0.45f; easy.bulletSpeedFactor = 0.7f; easy.enemyHealthFactor = 0.65f;
            easy.dropRateFactor = 1.8f; easy.ultimateChargeFactor = 0.6f; easy.hitboxFactor = 0.55f;
            easy.scoreFactor = 1.0f; easy.forceReadKnowledge = true; easy.enableDDA = true;
            easy.checkpoint = CheckpointMode.EverySegment; easy.clearBulletsOnDeath = true;
            Difficulties[Difficulty.Easy] = easy;

            var normal = ScriptableObject.CreateInstance<DifficultyData>();
            normal.difficulty = Difficulty.Normal; normal.displayName = "普通·标准"; normal.playerStartLives = 4;
            normal.bulletDensityFactor = 0.75f; normal.bulletSpeedFactor = 0.85f; normal.enemyHealthFactor = 0.85f;
            normal.dropRateFactor = 1.2f; normal.ultimateChargeFactor = 0.9f; normal.hitboxFactor = 0.75f;
            normal.scoreFactor = 1.5f; normal.forceReadKnowledge = false; normal.enableDDA = false;
            normal.checkpoint = CheckpointMode.BossOnly; normal.clearBulletsOnDeath = true;
            Difficulties[Difficulty.Normal] = normal;

            var hard = ScriptableObject.CreateInstance<DifficultyData>();
            hard.difficulty = Difficulty.Hard; hard.displayName = "困难·生存"; hard.playerStartLives = 2;
            hard.bulletDensityFactor = 1.2f; hard.bulletSpeedFactor = 1.1f; hard.enemyHealthFactor = 1.15f;
            hard.dropRateFactor = 0.85f; hard.ultimateChargeFactor = 1.2f; hard.hitboxFactor = 0.85f;
            hard.scoreFactor = 2.5f; hard.forceReadKnowledge = false; hard.enableDDA = false;
            hard.checkpoint = CheckpointMode.None; hard.clearBulletsOnDeath = false;
            Difficulties[Difficulty.Hard] = hard;
        }

        private static void AddCard(string id, string title, string body, string source, TriggerType trig, string related)
        {
            var c = ScriptableObject.CreateInstance<KnowledgeCard>();
            c.id = id; c.title = title; c.body = body; c.source = source; c.trigger = trig; c.relatedId = related;
            Knowledge[id] = c;
        }

        private static void BuildKnowledge()
        {
            AddCard("K01", "什么是糖尿病", "糖尿病是一种以慢性高血糖为特征的代谢性疾病。", "WHO 定义", TriggerType.Archive, "序章");
            AddCard("K02", "正常空腹血糖", "正常空腹血糖 3.9–6.1 mmol/L，≥7.0 需警惕。高血糖使血液黏稠、循环受阻。", "中国2型糖尿病防治指南", TriggerType.LevelClear, "L1");
            AddCard("K03", "1型 vs 2型", "1 型为胰岛素缺乏，2 型为胰岛素抵抗，机制不同。", "WHO", TriggerType.Death, "E02");
            AddCard("K04", "微血管并发症", "长期高血糖损害微血管，导致视网膜病变、肾病、神经病变。糖尿病视网膜病变是成人失明主因之一。", "中国2型糖尿病防治指南", TriggerType.LevelClear, "L2");
            AddCard("K05", "急慢性并发症", "糖尿病足、酮症酸中毒等急慢性并发症危害严重。", "中国2型糖尿病防治指南", TriggerType.Death, "E06");
            AddCard("K06", "三多一少", "典型症状：多饮、多食、多尿、体重减少。", "内科学教材", TriggerType.Archive, "");
            AddCard("K07", "高危因素", "高危因素：肥胖、久坐、高糖高脂饮食、家族史、年龄。", "WHO / CDC", TriggerType.Archive, "E05");
            AddCard("K08", "预防胜于治疗", "预防：均衡饮食、规律运动、控制体重、定期监测血糖。胰岛功能严重衰竭难以逆转。", "WHO 糖尿病预防建议", TriggerType.LevelClear, "L3");
            AddCard("K09", "遵医嘱用药", "已确诊者应遵医嘱用药，不可擅自停药。", "中国2型糖尿病防治指南", TriggerType.Archive, "");
            AddCard("K10", "定期体检", "每年体检测血糖，早发现早干预。", "中国2型糖尿病防治指南", TriggerType.LevelClear, "结局");
        }

        private static WaveEntry Wave(string id, float t, string seg, string enemy, int count, string pos, float interval,
            ItemType drop, bool hasDrop, float rate, string kid, bool boss = false, bool mixed = false)
        {
            return new WaveEntry
            {
                waveId = id, triggerTime = t, segment = seg, enemyId = enemy, count = count, spawnPosition = pos,
                spawnInterval = interval, dropItem = drop, hasDrop = hasDrop, dropRate = rate, knowledgeId = kid,
                isBoss = boss, isMixed = mixed
            };
        }

        private static void BuildLevels()
        {
            // ---- 关一 血潮 ----
            var l1 = ScriptableObject.CreateInstance<LevelTimeline>();
            l1.levelId = "L1"; l1.levelName = "血潮"; l1.bossId = "B01"; l1.duration = 90f; l1.clearKnowledgeId = "K02";
            l1.farColor = new Color(0.14f, 0.02f, 0.04f); l1.midColor = new Color(0.30f, 0.04f, 0.07f); l1.nearColor = new Color(0.5f, 0.07f, 0.11f);
            l1.waves.Add(Wave("L1-W01", 2, "导入段", "E01", 4, "顶部均匀", 0.4f, ItemType.Insulin, true, 0.55f, "K02"));
            l1.waves.Add(Wave("L1-W02", 8, "导入段", "E01", 6, "左右两侧", 0.3f, ItemType.Insulin, true, 0.35f, ""));
            l1.waves.Add(Wave("L1-W03", 15, "推进段", "E02", 3, "顶部分散", 0.6f, ItemType.Fiber, true, 0.45f, ""));
            l1.waves.Add(Wave("L1-W04", 24, "推进段", "E01", 8, "顶部随机", 0.25f, ItemType.SugarTrap, true, 0.20f, ""));
            l1.waves.Add(Wave("L1-W05", 33, "推进段", "E02", 4, "两侧交替", 0.5f, ItemType.Insulin, true, 0.50f, ""));
            l1.waves.Add(Wave("L1-W06", 42, "推进段精英", "ELITE1", 1, "屏幕中上", 0f, ItemType.Energy, true, 1.0f, ""));
            l1.waves.Add(Wave("L1-W07", 55, "Boss前补给", "E01", 6, "顶部均匀", 0.3f, ItemType.Insulin, true, 0.60f, ""));
            l1.waves.Add(Wave("L1-B", 65, "Boss段", "B01", 1, "屏幕中上", 0f, ItemType.Insulin, false, 0f, "K02", true));
            Levels.Add(l1);

            // ---- 关二 盲视 ----
            var l2 = ScriptableObject.CreateInstance<LevelTimeline>();
            l2.levelId = "L2"; l2.levelName = "盲视"; l2.bossId = "B02"; l2.duration = 85f; l2.clearKnowledgeId = "K04";
            l2.farColor = new Color(0.02f, 0.02f, 0.05f); l2.midColor = new Color(0.06f, 0.05f, 0.1f); l2.nearColor = new Color(0.12f, 0.08f, 0.16f);
            l2.waves.Add(Wave("L2-W01", 2, "导入段", "E03", 4, "顶部分散", 0.5f, ItemType.Insulin, true, 0.50f, "K04"));
            l2.waves.Add(Wave("L2-W02", 10, "推进段", "E04", 3, "两侧", 0.6f, ItemType.Fiber, true, 0.45f, ""));
            l2.waves.Add(Wave("L2-W03", 19, "推进段", "E03", 6, "顶部随机", 0.4f, ItemType.Insulin, true, 0.35f, ""));
            l2.waves.Add(Wave("L2-W04", 28, "推进段", "MIX23", 5, "全屏", 0.4f, ItemType.Energy, true, 0.50f, "", false, true));
            l2.waves.Add(Wave("L2-W05", 38, "推进段精英", "ELITE2", 1, "屏幕中上", 0f, ItemType.Insulin, true, 1.0f, ""));
            l2.waves.Add(Wave("L2-W06", 50, "Boss前补给", "E04", 4, "两侧", 0.5f, ItemType.Fiber, true, 0.60f, ""));
            l2.waves.Add(Wave("L2-B", 60, "Boss段", "B02", 1, "屏幕中上", 0f, ItemType.Insulin, false, 0f, "K04", true));
            Levels.Add(l2);

            // ---- 关三 核心 ----
            var l3 = ScriptableObject.CreateInstance<LevelTimeline>();
            l3.levelId = "L3"; l3.levelName = "核心"; l3.bossId = "B03"; l3.duration = 95f; l3.clearKnowledgeId = "K08";
            l3.farColor = new Color(0.03f, 0.02f, 0.04f); l3.midColor = new Color(0.1f, 0.06f, 0.1f); l3.nearColor = new Color(0.18f, 0.1f, 0.16f);
            l3.waves.Add(Wave("L3-W01", 2, "导入段", "E05", 3, "顶部分散", 0.6f, ItemType.Insulin, true, 0.50f, "K07"));
            l3.waves.Add(Wave("L3-W02", 12, "推进段", "E06", 5, "顶部随机", 0.4f, ItemType.Fiber, true, 0.45f, ""));
            l3.waves.Add(Wave("L3-W03", 22, "推进段", "E05", 4, "全屏", 0.5f, ItemType.Insulin, true, 0.35f, ""));
            l3.waves.Add(Wave("L3-W04", 32, "推进段", "E06", 8, "两侧交替", 0.3f, ItemType.Energy, true, 0.50f, ""));
            l3.waves.Add(Wave("L3-W05", 44, "推进段精英", "ELITE3", 1, "屏幕中上", 0f, ItemType.Insulin, true, 1.0f, ""));
            l3.waves.Add(Wave("L3-W06", 58, "Boss前补给", "MIX56", 6, "全屏", 0.35f, ItemType.Fiber, true, 0.65f, "", false, true));
            l3.waves.Add(Wave("L3-B", 70, "Boss段", "B03", 1, "屏幕中上", 0f, ItemType.Insulin, false, 0f, "K08", true));
            Levels.Add(l3);
        }

        public static EnemyData GetEnemy(string id) => Enemies.TryGetValue(id, out var e) ? e : null;
        public static BossData GetBoss(string id) => Bosses.TryGetValue(id, out var b) ? b : null;
        public static KnowledgeCard GetCard(string id) => Knowledge.TryGetValue(id, out var c) ? c : null;

        public static List<string> MixEnemiesFor(string levelId)
        {
            switch (levelId)
            {
                case "L2": return new List<string> { "E03", "E04" };
                case "L3": return new List<string> { "E05", "E06" };
                default: return new List<string> { "E01", "E02" };
            }
        }
    }
}
