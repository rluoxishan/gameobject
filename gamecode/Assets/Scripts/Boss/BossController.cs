using GlucoseWar.Bullets;
using GlucoseWar.Core;
using GlucoseWar.Data;
using GlucoseWar.Diff;
using GlucoseWar.Enemies;
using UnityEngine;

namespace GlucoseWar.Boss
{
    /// <summary>多阶段状态机框架，供 3 个 Boss 复用；按血量切阶段并触发专属机制。</summary>
    public class BossController : MonoBehaviour
    {
        public static BossController Current { get; private set; }

        private BossData data;
        private SpriteRenderer sr;
        private int maxHealth, health;
        private int phaseIndex;
        private bool alive;
        private bool reachedHome;
        private float homeY;
        private float fireTimer;
        private float driftSeed;
        private float summonTimer;
        private float blindTimer;
        private System.Action onDefeated;
        private Transform player;

        public bool Damageable => alive && reachedHome;

        public void Init(BossData bossData, System.Action defeatedCallback)
        {
            data = bossData;
            onDefeated = defeatedCallback;
            maxHealth = Mathf.Max(1, Mathf.RoundToInt(data.totalHealth * DifficultyService.EnemyHealth));
            health = maxHealth;
            phaseIndex = 0;
            alive = true;
            reachedHome = false;
            driftSeed = Random.value * 10f;
            Current = this;

            sr = GetComponent<SpriteRenderer>();
            if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = ProceduralSprites.Get(ProceduralSprites.Shape.Hexagon, data.bodyColor, 128);
            sr.sortingOrder = 4;
            transform.localScale = Vector3.one * data.visualSize;

            var col = gameObject.GetComponent<CircleCollider2D>();
            if (col == null) col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
            var rb = gameObject.GetComponent<Rigidbody2D>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.useFullKinematicContacts = true;

            homeY = Camera.main != null ? Camera.main.orthographicSize * 0.55f : 4.5f;
            transform.position = new Vector3(0, Camera.main != null ? Camera.main.orthographicSize + 2f : 12f, 0);

            EventBus.BossAppeared(data.displayName);
            EventBus.BossHealth(health, maxHealth, data.displayName);
            AudioManager.Instance?.PlayBossBgm();
        }

        private void Update()
        {
            if (!alive || data == null) return;
            float dt = Time.deltaTime;
            if (player == null) { var p = GameObject.Find("Player"); if (p != null) player = p.transform; }

            if (!reachedHome)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, homeY, 0), 4f * dt);
                if (Mathf.Abs(transform.position.y - homeY) < 0.05f) reachedHome = true;
                return;
            }

            // 横向游走
            Vector3 pos = transform.position;
            pos.x = Mathf.Sin(Time.time * 0.6f + driftSeed) * (Camera.main != null ? Camera.main.orthographicSize * 0.4f : 3f);
            transform.position = pos;

            Fire(dt);
            Mechanics(dt);
        }

        private void Fire(float dt)
        {
            BossPhase phase = data.phases[Mathf.Clamp(phaseIndex, 0, data.phases.Count - 1)];
            fireTimer -= dt;
            if (fireTimer > 0f) return;
            fireTimer = phase.fireInterval;

            Vector2 aim = player != null ? ((Vector2)player.position - (Vector2)transform.position).normalized : Vector2.down;
            float aimDeg = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
            float density = DifficultyService.BulletDensity;

            switch (phase.pattern)
            {
                case PatternType.BossSpread:
                    EmitFan(aimDeg, 70f, Mathf.RoundToInt(7 * density));
                    break;
                case PatternType.BossRadial:
                    EmitRing(Time.time * 40f, Mathf.RoundToInt(18 * density));
                    break;
                case PatternType.BossDense:
                    EmitRing(Time.time * 30f, Mathf.RoundToInt(24 * density));
                    EmitFan(aimDeg, 50f, Mathf.RoundToInt(5 * density));
                    break;
                default:
                    EmitFan(aimDeg, 40f, 3);
                    break;
            }
        }

        private void EmitFan(float centerDeg, float spread, int count)
        {
            count = Mathf.Max(1, count);
            float speed = 4.5f * DifficultyService.BulletSpeed;
            for (int i = 0; i < count; i++)
            {
                float t = count == 1 ? 0f : (float)i / (count - 1) - 0.5f;
                float a = (centerDeg + t * spread) * Mathf.Deg2Rad;
                BulletSpawner.Fire(transform.position, Team.Enemy, new Vector2(Mathf.Cos(a), Mathf.Sin(a)), speed, 1, data.bodyColor, 0.16f);
            }
        }

        private void EmitRing(float startDeg, int count)
        {
            count = Mathf.Max(3, count);
            float speed = 4.5f * DifficultyService.BulletSpeed;
            for (int i = 0; i < count; i++)
            {
                float a = (startDeg + 360f / count * i) * Mathf.Deg2Rad;
                BulletSpawner.Fire(transform.position, Team.Enemy, new Vector2(Mathf.Cos(a), Mathf.Sin(a)), speed, 1, data.bodyColor, 0.16f);
            }
        }

        private void Mechanics(float dt)
        {
            // B01 分裂态：召唤小凝块
            if (data.bossId == "B01" && phaseIndex >= 1)
            {
                summonTimer -= dt;
                if (summonTimer <= 0f)
                {
                    summonTimer = 3f;
                    var minion = GameDatabase.GetEnemy("E01");
                    if (minion != null)
                        for (int i = -1; i <= 1; i += 2)
                            EnemyBase.Spawn(minion, transform.position + new Vector3(i * 1.2f, -0.5f, 0));
                }
            }
            // B02 暴露态：周期性致盲遮挡
            if (data.bossId == "B02" && phaseIndex >= 1)
            {
                blindTimer -= dt;
                if (blindTimer <= 0f)
                {
                    blindTimer = 5f;
                    GlucoseWar.UI.UIManager.Instance?.PlayBlind(2.5f);
                }
            }
            // B03 侵蚀期：持续召唤高糖孢子
            if (data.bossId == "B03" && phaseIndex == 0)
            {
                summonTimer -= dt;
                if (summonTimer <= 0f)
                {
                    summonTimer = 4f;
                    var spore = GameDatabase.GetEnemy("E05");
                    if (spore != null) EnemyBase.Spawn(spore, transform.position + new Vector3(Random.Range(-2f, 2f), -1f, 0));
                }
            }
        }

        public void TakeDamage(int dmg)
        {
            if (!Damageable) return;
            health -= dmg;
            if (sr != null) sr.color = Color.Lerp(data.bodyColor, Color.white, 0.5f);
            EventBus.BossHealth(Mathf.Max(0, health), maxHealth, data.displayName);
            UpdatePhase();
            if (health <= 0) Die();
        }

        public void TakeUltimateHit(int dmg)
        {
            if (!alive) return;
            // 大招破盾/打断：即使未完全可受伤也生效
            reachedHome = true;
            TakeDamage(dmg);
        }

        private void LateUpdate()
        {
            if (alive && sr != null && data != null)
                sr.color = Color.Lerp(sr.color, data.bodyColor, Time.deltaTime * 10f);
        }

        private void UpdatePhase()
        {
            float frac = (float)health / maxHealth;
            int target = phaseIndex;
            for (int i = phaseIndex + 1; i < data.phases.Count; i++)
                if (frac <= data.phases[i].hpFractionEnter) target = i;

            if (target != phaseIndex)
            {
                phaseIndex = target;
                CameraShake.Shake(0.25f, 0.4f);
                PrefabFactory.Instance?.SpawnVfx(transform.position, data.bodyColor, 1f, data.visualSize * 3f, 0.5f);
                summonTimer = 0f;
            }
        }

        private void Die()
        {
            alive = false;
            EventBus.BossDefeated();
            PrefabFactory.Instance?.SpawnVfx(transform.position, Color.white, 1f, data.visualSize * 4f, 0.8f);
            CameraShake.Shake(0.5f, 0.8f);
            AudioManager.Instance?.PlaySfx(SfxId.Explosion);
            onDefeated?.Invoke();
            Current = null;
            Destroy(gameObject, 0.1f);
        }

        private void OnDestroy()
        {
            if (Current == this) Current = null;
        }
    }
}
