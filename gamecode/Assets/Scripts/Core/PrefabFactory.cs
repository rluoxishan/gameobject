using GlucoseWar.Bullets;
using GlucoseWar.Enemies;
using GlucoseWar.Items;
using UnityEngine;

namespace GlucoseWar.Core
{
    /// <summary>
    /// 程序化构建并缓存"预制体模板"（无外部资产）。模板为禁用的 GameObject，
    /// 交给 ObjectPool 复制复用。命中判定走 Trigger + 组件查找，无需配置物理层。
    /// </summary>
    public class PrefabFactory : Singleton<PrefabFactory>
    {
        public GameObject BulletTemplate { get; private set; }
        public GameObject EnemyTemplate { get; private set; }
        public GameObject ItemTemplate { get; private set; }
        public GameObject VfxTemplate { get; private set; }

        private Transform hidden;

        protected override void Awake()
        {
            base.Awake();
            hidden = new GameObject("[Templates]").transform;
            hidden.SetParent(transform);
            hidden.gameObject.SetActive(false);

            BulletTemplate = BuildBullet();
            EnemyTemplate = BuildEnemy();
            ItemTemplate = BuildItem();
            VfxTemplate = BuildVfx();
        }

        private GameObject Base(string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(hidden);
            go.AddComponent<SpriteRenderer>();
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
            rb.useFullKinematicContacts = true; // 运动学物体之间也产生 Trigger 回调
            return go;
        }

        private GameObject BuildBullet()
        {
            var go = Base("Bullet");
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
            go.AddComponent<Bullet>();
            return go;
        }

        private GameObject BuildEnemy()
        {
            var go = Base("Enemy");
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            go.AddComponent<EnemyBase>();
            return go;
        }

        private GameObject BuildItem()
        {
            var go = Base("Item");
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
            go.AddComponent<PowerUp>();
            return go;
        }

        private GameObject BuildVfx()
        {
            var go = new GameObject("Vfx");
            go.transform.SetParent(hidden);
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<Vfx>();
            return go;
        }

        public void SpawnVfx(Vector3 pos, Color color, float startSize, float endSize, float duration)
        {
            if (ObjectPool.Instance == null) return;
            var go = ObjectPool.Instance.Spawn(VfxTemplate, pos, Quaternion.identity);
            go.GetComponent<Vfx>().Play(color, startSize, endSize, duration);
        }
    }
}
