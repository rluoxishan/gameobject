#if UNITY_EDITOR
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GlucoseWar.Data;
using UnityEditor;
using UnityEngine;

namespace GlucoseWar.EditorTools
{
    /// <summary>
    /// CSV → ScriptableObject 导入工具（程序任务 T-32）。
    /// 读取《敌人属性表/难度系数表/Boss配置表/关卡波次配置表》生成/更新 SO 资产。
    /// 菜单：糖战/导入配置表 (CSV → SO)
    /// </summary>
    public static class CsvImporter
    {
        private const string PathKey = "gw_csv_folder";
        private const string SoRoot = "Assets/ScriptableObjects";

        [MenuItem("糖战/导入配置表 (CSV → SO)")]
        public static void Import()
        {
            string defaultPath = EditorPrefs.GetString(PathKey,
                Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "gameobject", "配置表")));
            string folder = EditorUtility.OpenFolderPanel("选择 配置表 文件夹（含 4 个 .csv）", defaultPath, "");
            if (string.IsNullOrEmpty(folder)) return;
            EditorPrefs.SetString(PathKey, folder);

            EnsureFolders();
            int enemies = ImportEnemies(Path.Combine(folder, "敌人属性表.csv"));
            int diffs = ImportDifficulties(Path.Combine(folder, "难度系数表.csv"));
            int bosses = ImportBosses(Path.Combine(folder, "Boss配置表.csv"));
            int levels = ImportWaves(Path.Combine(folder, "关卡波次配置表.csv"));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("糖战 · 配置导入完成",
                $"敌人 {enemies} 个\n难度 {diffs} 档\nBoss {bosses} 个\n关卡 {levels} 关\n\n资产位于 {SoRoot}", "好");
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder(SoRoot)) AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            foreach (var sub in new[] { "Enemies", "Difficulty", "Bosses", "Levels" })
                if (!AssetDatabase.IsValidFolder($"{SoRoot}/{sub}"))
                    AssetDatabase.CreateFolder(SoRoot, sub);
        }

        private static List<string[]> ReadCsv(string path, out bool ok)
        {
            ok = false;
            var rows = new List<string[]>();
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[CsvImporter] 找不到文件：{path}");
                return rows;
            }
            foreach (var raw in File.ReadAllLines(path))
            {
                string line = raw.TrimEnd('\r');
                if (string.IsNullOrWhiteSpace(line)) continue;
                rows.Add(line.Split(','));
            }
            ok = rows.Count > 1;
            return rows;
        }

        private static T LoadOrCreate<T>(string assetPath) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existing != null) return existing;
            var so = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(so, assetPath);
            return so;
        }

        private static float F(string s, float def = 0f)
            => float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : def;
        private static int I(string s, int def = 0)
            => int.TryParse(s, out var v) ? v : def;

        // ---------------- 敌人 ----------------
        private static int ImportEnemies(string path)
        {
            var rows = ReadCsv(path, out bool ok);
            if (!ok) return 0;
            int n = 0;
            for (int r = 1; r < rows.Count; r++)
            {
                var c = rows[r];
                if (c.Length < 12) { Debug.LogWarning($"[敌人表] 第{r + 1}行列数不足，跳过"); continue; }
                string id = c[0].Trim();
                if (string.IsNullOrEmpty(id)) continue;
                var e = LoadOrCreate<EnemyData>($"{SoRoot}/Enemies/{id}.asset");
                e.enemyId = id;
                e.displayName = c[1].Trim();
                e.level = c[2].Trim();
                e.maxHealth = I(c[3]);
                e.moveSpeed = F(c[4]);
                e.score = I(c[5]);
                e.moveMode = ParseMove(c[6].Trim());
                e.fireInterval = c[8].Contains("-") ? -1f : (c[8].Contains("死亡") ? 99f : F(c[8], -1f));
                e.hitRadius = F(c[9], 0.4f);
                e.knowledgeId = c[10].Trim();
                e.pathology = c[11].Trim();
                e.isElite = id.StartsWith("ELITE");
                e.isSpore = c[6].Contains("增殖");
                EditorUtility.SetDirty(e);
                n++;
            }
            return n;
        }

        // ---------------- 难度 ----------------
        private static int ImportDifficulties(string path)
        {
            var rows = ReadCsv(path, out bool ok);
            if (!ok) return 0;
            // 行=参数，列：参数,简单,普通,困难,说明
            var map = new Dictionary<string, string[]>();
            for (int r = 1; r < rows.Count; r++)
                if (rows[r].Length >= 4) map[rows[r][0].Trim()] = rows[r];

            int n = 0;
            var defs = new[] { (Difficulty.Easy, 1), (Difficulty.Normal, 2), (Difficulty.Hard, 3) };
            foreach (var (diff, col) in defs)
            {
                var d = LoadOrCreate<DifficultyData>($"{SoRoot}/Difficulty/{diff}.asset");
                d.difficulty = diff;
                if (map.TryGetValue("玩家初始生命", out var lives)) d.playerStartLives = I(lives[col]);
                if (map.TryGetValue("弹幕密度系数", out var bd)) d.bulletDensityFactor = F(bd[col], 1f);
                if (map.TryGetValue("子弹速度系数", out var bs)) d.bulletSpeedFactor = F(bs[col], 1f);
                if (map.TryGetValue("敌人血量系数", out var eh)) d.enemyHealthFactor = F(eh[col], 1f);
                if (map.TryGetValue("道具掉落率系数", out var dr)) d.dropRateFactor = F(dr[col], 1f);
                if (map.TryGetValue("大招冷却系数", out var uc)) d.ultimateChargeFactor = F(uc[col], 1f);
                if (map.TryGetValue("命中盒系数", out var hb)) d.hitboxFactor = F(hb[col], 1f);
                if (map.TryGetValue("计分难度系数", out var sf)) d.scoreFactor = F(sf[col], 1.5f);
                EditorUtility.SetDirty(d);
                n++;
            }
            return n;
        }

        // ---------------- Boss ----------------
        private static int ImportBosses(string path)
        {
            var rows = ReadCsv(path, out bool ok);
            if (!ok) return 0;
            var bossMap = new Dictionary<string, BossData>();
            for (int r = 1; r < rows.Count; r++)
            {
                var c = rows[r];
                if (c.Length < 10) continue;
                string id = c[0].Trim();
                if (!bossMap.TryGetValue(id, out var boss))
                {
                    boss = LoadOrCreate<BossData>($"{SoRoot}/Bosses/{id}.asset");
                    boss.bossId = id; boss.displayName = c[1].Trim(); boss.level = c[2].Trim();
                    boss.totalHealth = I(c[3]); boss.knowledgeId = c[9].Trim();
                    boss.phases = new List<BossPhase>();
                    bossMap[id] = boss;
                }
                boss.phases.Add(new BossPhase
                {
                    phaseName = c[4].Trim(),
                    hpFractionEnter = boss.phases.Count == 0 ? 1f : 0.5f,
                    pattern = PatternType.BossSpread,
                    fireInterval = 1.0f,
                    mechanic = c[7].Trim(),
                    knowledgeId = c[9].Trim()
                });
                EditorUtility.SetDirty(boss);
            }
            return bossMap.Count;
        }

        // ---------------- 关卡波次 ----------------
        private static int ImportWaves(string path)
        {
            var rows = ReadCsv(path, out bool ok);
            if (!ok) return 0;
            var levelMap = new Dictionary<string, LevelTimeline>();
            var idMap = new Dictionary<string, string> { { "关一血潮", "L1" }, { "关二盲视", "L2" }, { "关三核心", "L3" } };

            for (int r = 1; r < rows.Count; r++)
            {
                var c = rows[r];
                if (c.Length < 14) continue;
                string levelCn = c[0].Trim();
                string levelId = idMap.TryGetValue(levelCn, out var lid) ? lid : levelCn;
                if (!levelMap.TryGetValue(levelId, out var level))
                {
                    level = LoadOrCreate<LevelTimeline>($"{SoRoot}/Levels/{levelId}.asset");
                    level.levelId = levelId;
                    level.levelName = levelCn;
                    level.waves = new List<WaveEntry>();
                    levelMap[levelId] = level;
                }

                bool isBoss = c[3].Contains("Boss段");
                if (isBoss) { level.bossId = c[4].Trim(); level.clearKnowledgeId = c[13].Trim(); }

                level.waves.Add(new WaveEntry
                {
                    waveId = c[1].Trim(),
                    triggerTime = F(c[2]),
                    segment = c[3].Trim(),
                    enemyId = c[4].Trim(),
                    count = I(c[6], 1),
                    spawnPosition = c[7].Trim(),
                    spawnInterval = F(c[8], 0.4f),
                    dropItem = ParseItem(c[11].Trim()),
                    hasDrop = !string.IsNullOrEmpty(c[11].Trim()) && c[11].Trim() != "无" && c[11].Trim() != "-",
                    dropRate = F(c[12]),
                    knowledgeId = c[13].Trim(),
                    isBoss = isBoss,
                    isMixed = c[4].Trim().StartsWith("MIX")
                });
                EditorUtility.SetDirty(level);
            }
            return levelMap.Count;
        }

        private static MoveMode ParseMove(string s)
        {
            if (s.Contains("直线下冲")) return MoveMode.StraightDown;
            if (s.Contains("正弦")) return MoveMode.SineHorizontal;
            if (s.Contains("静止爆裂")) return MoveMode.StaticBurst;
            if (s.Contains("缠绕")) return MoveMode.WrapLimit;
            if (s.Contains("漂浮")) return MoveMode.FloatSpread;
            if (s.Contains("扑咬")) return MoveMode.SwarmRush;
            if (s.Contains("召唤")) return MoveMode.StaticSummon;
            if (s.Contains("缓动")) return MoveMode.Drift;
            return MoveMode.Tracking;
        }

        private static ItemType ParseItem(string s)
        {
            if (s.Contains("胰岛素")) return ItemType.Insulin;
            if (s.Contains("纤维")) return ItemType.Fiber;
            if (s.Contains("运动")) return ItemType.Energy;
            if (s.Contains("高糖")) return ItemType.SugarTrap;
            return ItemType.Insulin;
        }
    }
}
#endif
