using System.Collections.Generic;
using GlucoseWar.Bullets;
using GlucoseWar.Core;
using GlucoseWar.Data;
using GlucoseWar.Diff;
using GlucoseWar.Items;
using GlucoseWar.Player;
using UnityEngine;

namespace GlucoseWar.Enemies
{
    /// <summary>敌人基类：读取 EnemyData 管理血量/移动/开火/死亡/掉落/科普解锁。</summary>
    public class EnemyBase : MonoBehaviour, IPoolable
    {
        public static readonly List<EnemyBase> Active = new List<EnemyBase>();
        private static int sporeCount;

        private EnemyData data;
        private SpriteRenderer sr;
        private CircleCollider2D col;
        private int health;
        private float fireTimer;
        private float summonTimer;
        private float sporeTimer;
        private float sineSeed;
        private float homeY;
        private bool reachedHome;
        private Transform player;

        public bool IsAlive { get; private set; }
        public EnemyData Data => data;

        private bool hasDrop;
        private ItemType dropItem;
        private float dropRate;

        public void SetDrop(bool has, ItemType item, float rate)
        {
            hasDrop = has; dropItem = item; dropRate = rate;
        }

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<CircleCollider2D>();
        }

        public void Init(EnemyData enemyData)
        {
            data = enemyData;
            health = Mathf.Max(1, Mathf.RoundToInt(data.maxHealth * DifficultyService.EnemyHealth));
            fireTimer = data.fireInterval > 0 ? Random.Range(0.2f, data.fireInterval) : 0f;
            summonTimer = data.summonInterval;
            sporeTimer = 3.5f;
            sineSeed = Random.Range(0f, Mathf.PI * 2f);
            reachedHome = false;
            IsAlive = true;
            hasDrop = false;

            sr.sprite = ProceduralSprites.Get(ToShape(data.shape), data.bodyColor, data.isElite ? 96 : 64);
            sr.sortingOrder = 4;
            transform.localScale = Vector3.one * data.visualSize;
            if (col != null) col.radius = 0.45f;

            // 进场停靠高度：精英与召唤型停在上方，其它一路下行
            homeY = Camera.main != null ? Camera.main.orthographicSize * (data.isElite ? 0.55f : 0.3f) : 4f;
            if (data.isSpore) sporeCount++;
        }

        private static ProceduralSprites.Shape ToShape(ProceduralShapeKind k)
        {
            switch (k)
            {
                case ProceduralShapeKind.Circle: return ProceduralSprites.Shape.Circle;
                case ProceduralShapeKind.Triangle: return ProceduralSprites.Shape.Triangle;
                case ProceduralShapeKind.Diamond: return ProceduralSprites.Shape.Diamond;
                case ProceduralShapeKind.Square: return ProceduralSprites.Shape.Square;
                case ProceduralShapeKind.Ring: return ProceduralSprites.Shape.Ring;
                default: return ProceduralSprites.Shape.Hexagon;
            }
        }

        public void OnSpawned()
        {
            Active.Add(this);
            if (sr != null) { var c = sr.color; c.a = 1f; sr.color = c; }
        }

        public void OnDespawned()
        {
            Active.Remove(this);
            if (data != null && data.isSpore) sporeCount = Mathf.Max(0, sporeCount - 1);
        }

        private void Update()
        {
            if (!IsAlive || data == null) return;
            float dt = Time.deltaTime;
            if (player == null)
            {
                var p = GameObject.Find("Player");
                if (p != null) player = p.transform;
            }

            Move(dt);
            HandleFire(dt);
            HandleSummon(dt);
            HandleSpore(dt);

            // 出屏回收（下方）
            float bottom = Camera.main != null ? -Camera.main.orthographicSize - 2f : -12f;
            if (transform.position.y < bottom)
                Recycle(false);
        }

        private void Move(float dt)
        {
            Vector3 pos = transform.position;
            float spd = data.moveSpeed;
            switch (data.moveMode)
            {
                case MoveMode.StraightDown:
                    pos.y -= spd * dt;
                    break;
                case MoveMode.SineHorizontal:
                    pos.y -= spd * 0.6f * dt;
                    pos.x += Mathf.Cos(Time.time * 2f + sineSeed) * 2.5f * dt;
                    break;
                case MoveMode.StaticBurst:
                    if (!reachedHome) { pos.y -= spd * dt; if (pos.y <= homeY) reachedHome = true; }
                    break;
                case MoveMode.WrapLimit:
                    if (!reachedHome) { pos.y -= spd * dt; if (pos.y <= homeY) reachedHome = true; }
                    else pos.x += Mathf.Sin(Time.time * 1.5f + sineSeed) * 1.5f * dt;
                    break;
                case MoveMode.FloatSpread:
                    pos.y -= spd * 0.5f * dt;
                    pos.x += Mathf.Sin(Time.time + sineSeed) * 0.8f * dt;
                    break;
                case MoveMode.SwarmRush:
                case MoveMode.Tracking:
                    if (player != null)
                    {
                        Vector2 dir = ((Vector2)player.position - (Vector2)pos).normalized;
                        pos += (Vector3)(dir * spd * dt);
                    }
                    else pos.y -= spd * dt;
                    break;
                case MoveMode.Drift:
                    if (!reachedHome) { pos.y -= 1.5f * dt; if (pos.y <= homeY) reachedHome = true; }
                    else pos.x += Mathf.Sin(Time.time * 0.8f + sineSeed) * spd * dt;
                    break;
                case MoveMode.StaticSummon:
                    if (!reachedHome) { pos.y -= 2f * dt; if (pos.y <= homeY) reachedHome = true; }
                    break;
            }
            transform.position = pos;
        }

        private void HandleFire(float dt)
        {
            if (data.bulletPattern == null || data.fireInterval <= 0f || data.fireInterval > 90f) return;
            fireTimer -= dt;
            if (fireTimer <= 0f)
            {
                fireTimer = data.fireInterval;
                Vector2 aim = player != null
                    ? ((Vector2)player.position - (Vector2)transform.position).normalized
                    : Vector2.down;
                BulletSpawner.Emit(transform.position, Team.Enemy, data.bulletPattern, aim);
            }
        }

        private void HandleSummon(float dt)
        {
            if (string.IsNullOrEmpty(data.summonEnemyId) || data.summonInterval <= 0f) return;
            summonTimer -= dt;
            if (summonTimer <= 0f)
            {
                summonTimer = data.summonInterval;
                var child = GameDatabase.GetEnemy(data.summonEnemyId);
                if (child != null) Spawn(child, transform.position + (Vector3)(Random.insideUnitCircle * 0.8f));
            }
        }

        private void HandleSpore(float dt)
        {
            if (!data.isSpore) return;
            sporeTimer -= dt;
            if (sporeTimer <= 0f && sporeCount < 18)
            {
                sporeTimer = 4.5f;
                Spawn(data, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f), 0));
            }
        }

        public static EnemyBase Spawn(EnemyData data, Vector3 pos)
        {
            if (data == null || ObjectPool.Instance == null || PrefabFactory.Instance == null) return null;
            var go = ObjectPool.Instance.Spawn(PrefabFactory.Instance.EnemyTemplate, pos, Quaternion.identity);
            var e = go.GetComponent<EnemyBase>();
            e.Init(data);
            return e;
        }

        public void TakeDamage(int dmg)
        {
            if (!IsAlive) return;
            health -= dmg;
            FlashHit();
            if (health <= 0) Die();
        }

        private void FlashHit()
        {
            if (sr != null) sr.color = Color.Lerp(data.bodyColor, Color.white, 0.6f);
        }

        private void LateUpdate()
        {
            if (IsAlive && sr != null && data != null)
                sr.color = Color.Lerp(sr.color, data.bodyColor, Time.deltaTime * 12f);
        }

        private void Die()
        {
            IsAlive = false;

            // 死亡溅射（出血点）
            if (data.bulletPattern != null && data.bulletPattern.type == PatternType.DeathSplash)
                BulletSpawner.Emit(transform.position, Team.Enemy, data.bulletPattern, Vector2.down);

            int score = Mathf.RoundToInt(data.score);
            EventBus.EnemyKilled(transform.position, score);
            if (!string.IsNullOrEmpty(data.knowledgeId))
                EventBus.KnowledgeUnlocked(data.knowledgeId);

            PrefabFactory.Instance?.SpawnVfx(transform.position, data.bodyColor, 0.3f, data.visualSize * 2.5f, 0.35f);
            AudioManager.Instance?.PlaySfx(SfxId.Explosion);

            if (hasDrop) DropTable.TryDrop(dropItem, dropRate, transform.position);
            Recycle(true);
        }

        private void Recycle(bool killed)
        {
            IsAlive = false;
            if (ObjectPool.Instance != null) ObjectPool.Instance.Despawn(gameObject);
            else gameObject.SetActive(false);
        }

        public static void ClearAll()
        {
            foreach (var e in Active.ToArray())
                if (e != null) e.Recycle(false);
            Active.Clear();
            sporeCount = 0;
        }
    }
}
