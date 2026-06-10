using UnityEngine;

namespace GlucoseWar.Data
{
    /// <summary>科普卡片：K01-K10。标题/正文/触发条件。</summary>
    [CreateAssetMenu(menuName = "糖战/科普卡片", fileName = "KnowledgeCard")]
    public class KnowledgeCard : ScriptableObject
    {
        public string id = "K01";
        public string title = "";
        [TextArea(2, 6)] public string body = "";
        public string source = "";
        public TriggerType trigger = TriggerType.Archive;
        public string relatedId = ""; // 关联关卡或敌人ID
    }

    public enum TriggerType { LevelClear, Death, Archive }

    /// <summary>Boss 阶段数据（对应《Boss配置表.csv》一行）。</summary>
    [System.Serializable]
    public class BossPhase
    {
        public string phaseName;      // 完整态/分裂态/护盾态...
        public float hpFractionEnter; // 进入该阶段时的血量比例（1.0,0.5,0.66...）
        public PatternType pattern;
        public float fireInterval = 1.2f;
        public string mechanic;       // 特殊机制说明
        public string knowledgeId = "";
    }
}
