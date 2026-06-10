using GlucoseWar.Core;
using GlucoseWar.Data;

namespace GlucoseWar.Diff
{
    /// <summary>全局难度参数提供者，被各战斗模块查询/注入。</summary>
    public static class DifficultyService
    {
        public static DifficultyData Current { get; private set; }

        public static void Set(GlucoseWar.Data.Difficulty difficulty)
        {
            GameDatabase.EnsureBuilt();
            Current = GameDatabase.Difficulties[difficulty];
            ddaReduction = 0f;
        }

        public static void EnsureDefault()
        {
            if (Current == null) Set(GlucoseWar.Data.Difficulty.Normal);
        }

        // 动态难度：连死降密度（普通可选）
        private static float ddaReduction;
        public static void OnPlayerDeath()
        {
            if (Current != null && Current.enableDDA)
                ddaReduction = UnityEngine.Mathf.Min(0.3f, ddaReduction + 0.1f);
        }
        public static void ResetDDA() => ddaReduction = 0f;

        public static float BulletDensity => (Current?.bulletDensityFactor ?? 1f) * (1f - ddaReduction);
        public static float BulletSpeed => Current?.bulletSpeedFactor ?? 1f;
        public static float EnemyHealth => Current?.enemyHealthFactor ?? 1f;
        public static float DropRate => Current?.dropRateFactor ?? 1f;
        public static float UltimateCharge => Current?.ultimateChargeFactor ?? 1f;
        public static float Hitbox => Current?.hitboxFactor ?? 1f;
        public static float ScoreFactor => Current?.scoreFactor ?? 1.5f;
        public static int StartLives => Current?.playerStartLives ?? 3;
    }
}
