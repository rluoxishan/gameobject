using GlucoseWar.Core;
using GlucoseWar.Data;
using GlucoseWar.Enemies;
using GlucoseWar.Boss;
using GlucoseWar.Player;
using UnityEngine;

namespace GlucoseWar.Bullets
{
    /// <summary>池化子弹：敌我分层（按 Team 过滤）、移动、命中、出屏回收。</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Bullet : MonoBehaviour, IPoolable
    {
        private SpriteRenderer sr;
        private CircleCollider2D col;
        private Team team;
        private Vector2 velocity;
        private int damage = 1;
        private float life;
        private bool homing;
        private Transform homingTarget;
        private float homingTurn = 200f;

        public static Bounds PlayField = new Bounds(Vector3.zero, new Vector3(20, 24, 1));

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            col = GetComponent<CircleCollider2D>();
        }

        public void Init(Team team, Vector2 dir, float speed, int damage, Color color, float radius, bool homing = false)
        {
            this.team = team;
            this.velocity = dir.normalized * speed;
            this.damage = damage;
            this.homing = homing;
            this.life = 8f;
            homingTarget = null;

            sr.sprite = ProceduralSprites.Get(ProceduralSprites.Shape.Bullet, color, 24);
            sr.sortingOrder = 5;
            float scale = radius * 2f;
            transform.localScale = new Vector3(scale, scale, 1f);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f);
            if (col != null) col.radius = 0.5f;
        }

        public void OnSpawned() { }
        public void OnDespawned() { homingTarget = null; }

        private void Update()
        {
            float dt = Time.deltaTime;
            life -= dt;

            if (homing)
            {
                if (homingTarget == null || !homingTarget.gameObject.activeInHierarchy)
                    homingTarget = FindNearestEnemy();
                if (homingTarget != null)
                {
                    Vector2 desired = ((Vector2)homingTarget.position - (Vector2)transform.position).normalized * velocity.magnitude;
                    velocity = Vector2.MoveTowards(velocity, desired, homingTurn * dt);
                }
            }

            transform.position += (Vector3)(velocity * dt);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f);

            if (life <= 0f || !PlayField.Contains(new Vector3(transform.position.x, transform.position.y, 0)))
                Recycle();
        }

        private Transform FindNearestEnemy()
        {
            Transform best = null;
            float bestSq = float.MaxValue;
            foreach (var e in EnemyBase.Active)
            {
                if (e == null) continue;
                float sq = ((Vector2)e.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (sq < bestSq) { bestSq = sq; best = e.transform; }
            }
            if (BossController.Current != null && BossController.Current.Damageable)
            {
                float sq = ((Vector2)BossController.Current.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (sq < bestSq) best = BossController.Current.transform;
            }
            return best;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (team == Team.Player)
            {
                var enemy = other.GetComponentInParent<EnemyBase>();
                if (enemy != null && enemy.IsAlive) { enemy.TakeDamage(damage); Recycle(); return; }
                var boss = other.GetComponentInParent<BossController>();
                if (boss != null && boss.Damageable) { boss.TakeDamage(damage); Recycle(); return; }
            }
            else
            {
                var hitbox = other.GetComponent<PlayerHitbox>();
                if (hitbox != null) { hitbox.Owner.TakeHit(damage); Recycle(); }
            }
        }

        public void Recycle()
        {
            if (ObjectPool.Instance != null) ObjectPool.Instance.Despawn(gameObject);
            else gameObject.SetActive(false);
        }

        public Team Team => team;
    }
}
