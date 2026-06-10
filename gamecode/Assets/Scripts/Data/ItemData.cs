using UnityEngine;

namespace GlucoseWar.Data
{
    /// <summary>道具配置：4 种道具（含负面高糖陷阱）。</summary>
    [CreateAssetMenu(menuName = "糖战/道具配置", fileName = "ItemData")]
    public class ItemData : ScriptableObject
    {
        public ItemType type = ItemType.Insulin;
        public string displayName = "胰岛素结晶";
        public float effectValue = 1f;   // 火力级数 / 护盾量 / 加速倍率
        public float duration = 0f;      // 持续型效果时长
        public Color color = new Color(0.17f, 0.66f, 1f);
        public ProceduralShapeKind shape = ProceduralShapeKind.Hexagon;
    }
}
