using GlucoseWar.Bullets;
using GlucoseWar.Core;
using GlucoseWar.Diff;
using UnityEngine;

namespace GlucoseWar.Player
{
    /// <summary>生命(命数)/护盾/无敌帧/受伤/死亡。命数由 GameManager 维护。</summary>
    public class PlayerHealth : MonoBehaviour
    {
        public const float MaxShield = 100f;
        private const float ShieldPerHit = 50f;
        private const float InvulnTime = 1.2f;

        private float shield;
        private float invulnTimer;
        private SpriteRenderer sr;
        private PlayerWeapon weapon;

        public bool IsInvulnerable => invulnTimer > 0f;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            weapon = GetComponent<PlayerWeapon>();
        }

        private void Start()
        {
            PublishLives();
            EventBus.PlayerShieldChanged(shield, MaxShield);
        }

        private void Update()
        {
            if (invulnTimer > 0f)
            {
                invulnTimer -= Time.deltaTime;
                if (sr != null)
                {
                    float a = Mathf.PingPong(Time.unscaledTime * 10f, 1f) * 0.6f + 0.4f;
                    var c = sr.color; c.a = a; sr.color = c;
                }
                if (invulnTimer <= 0f && sr != null)
                {
                    var c = sr.color; c.a = 1f; sr.color = c;
                }
            }
        }

        public void GrantInvulnerability(float seconds)
        {
            invulnTimer = Mathf.Max(invulnTimer, seconds);
        }

        public void AddShield(float amount)
        {
            shield = Mathf.Clamp(shield + amount, 0f, MaxShield);
            EventBus.PlayerShieldChanged(shield, MaxShield);
        }

        public void TakeHit(int damage)
        {
            if (IsInvulnerable) return;

            if (shield > 0f)
            {
                shield = Mathf.Max(0f, shield - ShieldPerHit);
                EventBus.PlayerShieldChanged(shield, MaxShield);
                invulnTimer = InvulnTime;
                EventBus.PlayerDamaged();
                AudioManager.Instance?.PlaySfx(SfxId.Hit);
                return;
            }

            LoseLife();
        }

        private void LoseLife()
        {
            EventBus.PlayerDamaged();
            EventBus.PlayerDied();
            AudioManager.Instance?.PlaySfx(SfxId.Hit);

            var diff = DifficultyService.Current;
            if (diff != null && diff.clearBulletsOnDeath)
                BulletSpawner.ClearAllEnemyBullets();

            // 死亡火力回退（普通/困难回退1级，简单不回退）
            if (diff != null && diff.difficulty != GlucoseWar.Data.Difficulty.Easy)
                weapon?.ReduceFirepower(1);

            GameManager.Instance?.NotifyLifeLost();
            PublishLives();

            if (GameManager.Instance != null && !GameManager.Instance.HasLivesLeft)
            {
                GameManager.Instance.OnPlayerOutOfLives();
                gameObject.SetActive(false);
                return;
            }

            invulnTimer = InvulnTime + 0.6f;
            AddShield(MaxShield); // 复活给满护盾喘息
        }

        private void PublishLives()
        {
            int lives = GameManager.Instance != null ? GameManager.Instance.Lives : 3;
            EventBus.PlayerHealthChanged(lives, DifficultyService.StartLives);
        }
    }
}
