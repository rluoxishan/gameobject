using UnityEngine;

namespace GlucoseWar.Data
{
    /// <summary>敌人配置：对应《敌人属性表.csv》。</summary>
    [CreateAssetMenu(menuName = "糖战/敌人配置", fileName = "EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public string enemyId = "E01";
        public string displayName = "糖晶虫";
        public string level = "关一";
        public int maxHealth = 20;
        public float moveSpeed = 4f;
        public int score = 100;
        public MoveMode moveMode = MoveMode.StraightDown;
        public BulletPatternData bulletPattern;
        public float fireInterval = -1f;     // <=0 表示不开火
        public float hitRadius = 0.3f;
        public string knowledgeId = "";
        public string pathology = "";        // 病理隐喻

        [Header("表现")]
        public Color bodyColor = new Color(0.9f, 0.85f, 0.4f);
        public ProceduralShapeKind shape = ProceduralShapeKind.Hexagon;
        public float visualSize = 0.7f;

        [Header("特殊")]
        public bool isElite = false;
        public bool isSpore = false;         // 高糖孢子增殖
        public string summonEnemyId = "";    // 精英召唤的敌人ID
        public float summonInterval = 0f;
    }

    public enum ProceduralShapeKind { Circle, Triangle, Hexagon, Diamond, Square, Ring }
}
