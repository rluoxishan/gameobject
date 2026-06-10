using GlucoseWar.Core;
using GlucoseWar.Data;
using GlucoseWar.Player;
using UnityEngine;

namespace GlucoseWar.Items
{
    /// <summary>道具拾取与效果（含负面高糖陷阱）。池化、缓慢下落。</summary>
    public class PowerUp : MonoBehaviour, IPoolable
    {
        private SpriteRenderer sr;
        private ItemData data;
        private float fallSpeed = 2.2f;
        private float life;

        private void Awake() => sr = GetComponent<SpriteRenderer>();

        public void Init(ItemData itemData)
        {
            data = itemData;
            life = 12f;
            sr.sprite = ProceduralSprites.Get(ToShape(data.shape), data.color, 48);
            sr.sortingOrder = 5;
            transform.localScale = Vector3.one * 0.55f;
        }

        private static ProceduralSprites.Shape ToShape(ProceduralShapeKind k)
        {
            switch (k)
            {
                case ProceduralShapeKind.Circle: return ProceduralSprites.Shape.Circle;
                case ProceduralShapeKind.Diamond: return ProceduralSprites.Shape.Diamond;
                case ProceduralShapeKind.Ring: return ProceduralSprites.Shape.Ring;
                default: return ProceduralSprites.Shape.Hexagon;
            }
        }

        public void OnSpawned() { }
        public void OnDespawned() { }

        private void Update()
        {
            life -= Time.deltaTime;
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            transform.Rotate(0, 0, 90f * Time.deltaTime);
            float bottom = Camera.main != null ? -Camera.main.orthographicSize - 2f : -12f;
            if (life <= 0f || transform.position.y < bottom) Recycle();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var hitbox = other.GetComponent<PlayerHitbox>();
            if (hitbox == null) return;
            Apply(hitbox.Owner.gameObject);
            Recycle();
        }

        private void Apply(GameObject playerGo)
        {
            ScoreManager.Instance?.AddCollect();
            AudioManager.Instance?.PlaySfx(data.type == ItemType.SugarTrap ? SfxId.Hit : SfxId.Pickup);
            PrefabFactory.Instance?.SpawnVfx(transform.position, data.color, 0.2f, 1.2f, 0.3f);

            switch (data.type)
            {
                case ItemType.Insulin:
                    playerGo.GetComponent<PlayerWeapon>()?.AddFirepower((int)data.effectValue);
                    playerGo.GetComponent<UltimateSkill>()?.AddFromItem();
                    break;
                case ItemType.Fiber:
                    playerGo.GetComponent<PlayerHealth>()?.AddShield(data.effectValue);
                    break;
                case ItemType.Energy:
                    playerGo.GetComponent<PlayerController>()?.ApplySpeedBoost(data.effectValue, data.duration);
                    playerGo.GetComponent<PlayerHealth>()?.GrantInvulnerability(data.duration);
                    break;
                case ItemType.SugarTrap:
                    playerGo.GetComponent<PlayerWeapon>()?.ApplyFirepowerDebuff(data.duration);
                    GlucoseWar.UI.UIManager.Instance?.FlashRed();
                    break;
            }
        }

        private void Recycle()
        {
            if (ObjectPool.Instance != null) ObjectPool.Instance.Despawn(gameObject);
            else gameObject.SetActive(false);
        }

        public static void Drop(ItemData data, Vector3 pos)
        {
            if (data == null || ObjectPool.Instance == null || PrefabFactory.Instance == null) return;
            var go = ObjectPool.Instance.Spawn(PrefabFactory.Instance.ItemTemplate, pos, Quaternion.identity);
            go.GetComponent<PowerUp>().Init(data);
        }
    }
}
