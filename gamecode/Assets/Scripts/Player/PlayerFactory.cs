using GlucoseWar.Core;
using UnityEngine;

namespace GlucoseWar.Player
{
    /// <summary>程序化构建玩家战机（无外部资产）。</summary>
    public static class PlayerFactory
    {
        public static GameObject Create(Vector3 position)
        {
            var go = new GameObject("Player");
            go.transform.position = position;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = ProceduralSprites.Get(ProceduralSprites.Shape.Triangle, new Color(0.55f, 0.85f, 1f), 64);
            sr.sortingOrder = 6;
            go.transform.localScale = new Vector3(0.8f, 0.95f, 1f);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
            rb.useFullKinematicContacts = true;

            var health = go.AddComponent<PlayerHealth>();
            go.AddComponent<PlayerController>();
            go.AddComponent<PlayerWeapon>();
            go.AddComponent<UltimateSkill>();

            // 引擎尾焰（装饰）
            var flame = new GameObject("Flame");
            flame.transform.SetParent(go.transform);
            flame.transform.localPosition = new Vector3(0, -0.55f, 0);
            flame.transform.localScale = new Vector3(0.45f, 0.6f, 1f);
            var fsr = flame.AddComponent<SpriteRenderer>();
            fsr.sprite = ProceduralSprites.Get(ProceduralSprites.Shape.Triangle, new Color(1f, 0.55f, 0.1f, 0.8f), 32);
            fsr.transform.localRotation = Quaternion.Euler(0, 0, 180f);
            fsr.sortingOrder = 5;

            // 精准命中盒
            var hitboxGo = new GameObject("Hitbox");
            hitboxGo.transform.SetParent(go.transform);
            hitboxGo.transform.localPosition = Vector3.zero;
            var col = hitboxGo.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            var hitbox = hitboxGo.AddComponent<PlayerHitbox>();
            hitbox.Setup(health, 0.14f);

            return go;
        }
    }
}
