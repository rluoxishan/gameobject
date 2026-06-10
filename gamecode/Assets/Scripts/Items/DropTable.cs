using GlucoseWar.Core;
using GlucoseWar.Data;
using GlucoseWar.Diff;
using UnityEngine;

namespace GlucoseWar.Items
{
    /// <summary>按波次/难度掉落道具。</summary>
    public static class DropTable
    {
        public static void TryDrop(ItemType type, float baseRate, Vector3 pos)
        {
            // 负面陷阱不受掉落率系数提升，正面道具受难度掉落率系数影响
            float rate = type == ItemType.SugarTrap ? baseRate : baseRate * DifficultyService.DropRate;
            if (Random.value > rate) return;
            GameDatabase.EnsureBuilt();
            if (GameDatabase.Items.TryGetValue(type, out var data))
                PowerUp.Drop(data, pos);
        }
    }
}
