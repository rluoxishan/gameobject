using System.Collections.Generic;
using UnityEngine;

namespace GlucoseWar.Data
{
    /// <summary>Boss 配置：对应《Boss配置表.csv》，一个 Boss 含多个阶段。</summary>
    [CreateAssetMenu(menuName = "糖战/Boss配置", fileName = "BossData")]
    public class BossData : ScriptableObject
    {
        public string bossId = "B01";
        public string displayName = "黏稠之主";
        public string level = "关一";
        public int totalHealth = 800;
        public List<BossPhase> phases = new List<BossPhase>();
        public string knowledgeId = "K02";

        [Header("表现")]
        public Color bodyColor = new Color(0.5f, 0.1f, 0.15f);
        public float visualSize = 2.4f;
    }
}
