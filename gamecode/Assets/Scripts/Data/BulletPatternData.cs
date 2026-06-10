using UnityEngine;

namespace GlucoseWar.Data
{
    /// <summary>弹幕参数（角度/数量/速度/扩散/连射）。数据驱动，改 SO 不改代码。</summary>
    [CreateAssetMenu(menuName = "糖战/弹幕配置", fileName = "BulletPattern")]
    public class BulletPatternData : ScriptableObject
    {
        public PatternType type = PatternType.Single;
        public int bulletCount = 1;
        public float startAngle = 270f;   // 270 = 向下 (敌弹默认)
        public float spreadAngle = 0f;    // 扇形/散射总扩散
        public float bulletSpeed = 5f;
        public int burstCount = 1;        // 连射次数
        public float burstInterval = 0.1f;
        public int damage = 1;
        public Color color = new Color(0.65f, 0.2f, 0.85f); // 毒糖紫
    }
}
