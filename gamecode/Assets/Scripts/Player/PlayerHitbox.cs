using GlucoseWar.Diff;
using UnityEngine;

namespace GlucoseWar.Player
{
    /// <summary>精准命中盒（圆形），判定大小由难度系数注入。</summary>
    public class PlayerHitbox : MonoBehaviour
    {
        public PlayerHealth Owner { get; private set; }
        private CircleCollider2D col;

        private void Awake()
        {
            col = GetComponent<CircleCollider2D>();
        }

        public void Setup(PlayerHealth owner, float baseRadius)
        {
            Owner = owner;
            if (col == null) col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = baseRadius * DifficultyService.Hitbox;
        }
    }
}
