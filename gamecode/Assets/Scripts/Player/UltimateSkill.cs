using GlucoseWar.Boss;
using GlucoseWar.Bullets;
using GlucoseWar.Core;
using GlucoseWar.Diff;
using GlucoseWar.Enemies;
using UnityEngine;

namespace GlucoseWar.Player
{
    /// <summary>胰岛素脉冲：击杀充能→满格全屏清弹 + 范围伤害 + 破盾。</summary>
    public class UltimateSkill : MonoBehaviour
    {
        public const float Max = 100f;
        private float charge;

        private void OnEnable()
        {
            EventBus.OnEnemyKilled += OnEnemyKilled;
            EventBus.OnBossDefeated += OnBossDefeated;
            EventBus.UltimateChargeChanged(charge / Max);
        }

        private void OnDisable()
        {
            EventBus.OnEnemyKilled -= OnEnemyKilled;
            EventBus.OnBossDefeated -= OnBossDefeated;
        }

        private void Update()
        {
            if ((Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift))
                && charge >= Max)
            {
                Fire();
            }
        }

        private void OnEnemyKilled(Vector3 pos, int score) => Add(1f);
        private void OnBossDefeated() => Add(20f);
        public void AddFromItem() => Add(10f);

        private void Add(float baseAmount)
        {
            float gain = baseAmount / Mathf.Max(0.3f, DifficultyService.UltimateCharge);
            charge = Mathf.Clamp(charge + gain, 0f, Max);
            EventBus.UltimateChargeChanged(charge / Max);
        }

        private void Fire()
        {
            charge = 0f;
            EventBus.UltimateChargeChanged(0f);
            EventBus.UltimateFired();
            AudioManager.Instance?.PlaySfx(SfxId.Ultimate);

            BulletSpawner.ClearAllEnemyBullets();
            PrefabFactory.Instance?.SpawnVfx(transform.position, new Color(0.17f, 0.66f, 1f), 1f, 40f, 0.6f);
            CameraShake.Shake(0.3f, 0.5f);

            int aoe = 30;
            foreach (var e in EnemyBase.Active.ToArray())
                if (e != null && e.IsAlive) e.TakeDamage(aoe);

            if (BossController.Current != null)
                BossController.Current.TakeUltimateHit(aoe * 3);
        }
    }
}
