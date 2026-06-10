using UnityEngine;

namespace GlucoseWar.Data
{
    /// <summary>难度配置：对应《难度系数表.csv》，由 DifficultyService 全局注入。</summary>
    [CreateAssetMenu(menuName = "糖战/难度配置", fileName = "DifficultyData")]
    public class DifficultyData : ScriptableObject
    {
        public Difficulty difficulty = Difficulty.Normal;
        public string displayName = "普通标准";
        public int playerStartLives = 3;
        public float bulletDensityFactor = 1f; // 弹幕密度系数
        public float bulletSpeedFactor = 1f;   // 子弹速度系数
        public float enemyHealthFactor = 1f;   // 敌人血量系数
        public float dropRateFactor = 1f;      // 道具掉落率系数
        public float ultimateChargeFactor = 1f;// 大招冷却系数（越大越慢）
        public float hitboxFactor = 1f;        // 命中盒系数（越小越容错）
        public float scoreFactor = 1.5f;       // 计分难度系数
        public bool forceReadKnowledge = false;// 科普卡片强制阅读
        public bool enableDDA = false;
        public CheckpointMode checkpoint = CheckpointMode.BossOnly;
        public bool clearBulletsOnDeath = true;
    }

    public enum CheckpointMode { EverySegment, BossOnly, None }
}
