using GlucoseWar.Bullets;
using GlucoseWar.Core;
using GlucoseWar.Data;
using UnityEngine;

namespace GlucoseWar.Player
{
    /// <summary>自动开火 + 火力等级 Lv1-5 弹道切换（Lv5 含追踪弹）。</summary>
    public class PlayerWeapon : MonoBehaviour
    {
        private static readonly Color BulletColor = new Color(0.17f, 0.66f, 1f);
        private static readonly float[] FireInterval = { 0.25f, 0.22f, 0.20f, 0.18f, 0.15f };

        private int level = 1;            // 1..5
        private int insulinCollected = 0; // 升级所需结晶进度
        private float cooldown;
        private float firepowerDebuff;    // 高糖陷阱：临时降火力计时

        public int Level => level;

        private void OnEnable() => EventBus.FirepowerChanged(level);

        private void Update()
        {
            if (firepowerDebuff > 0f) firepowerDebuff -= Time.deltaTime;

            cooldown -= Time.deltaTime;
            if (cooldown <= 0f)
            {
                Fire();
                int effLevel = EffectiveLevel();
                cooldown = FireInterval[effLevel - 1];
            }
        }

        private int EffectiveLevel() => firepowerDebuff > 0f ? Mathf.Max(1, level - 1) : level;

        private void Fire()
        {
            Vector2 origin = transform.position;
            Vector2 up = Vector2.up;
            int lv = EffectiveLevel();
            int dmg = 2 + lv;
            float spd = 16f;

            switch (lv)
            {
                case 1:
                    Shot(origin, up, spd, dmg);
                    break;
                case 2:
                    Shot(origin + new Vector2(-0.2f, 0), up, spd, dmg);
                    Shot(origin + new Vector2(0.2f, 0), up, spd, dmg);
                    break;
                case 3:
                    Shot(origin, up, spd, dmg);
                    Shot(origin, Rotate(up, 18f), spd, dmg);
                    Shot(origin, Rotate(up, -18f), spd, dmg);
                    break;
                case 4:
                    Shot(origin, up, spd, dmg);
                    Shot(origin, Rotate(up, 12f), spd, dmg);
                    Shot(origin, Rotate(up, -12f), spd, dmg);
                    Shot(origin, Rotate(up, 26f), spd, dmg);
                    Shot(origin, Rotate(up, -26f), spd, dmg);
                    break;
                default: // Lv5
                    Shot(origin, up, spd, dmg);
                    Shot(origin, Rotate(up, 12f), spd, dmg);
                    Shot(origin, Rotate(up, -12f), spd, dmg);
                    Shot(origin, Rotate(up, 26f), spd, dmg);
                    Shot(origin, Rotate(up, -26f), spd, dmg);
                    Homing(origin + new Vector2(-0.4f, 0), spd * 0.8f, dmg);
                    Homing(origin + new Vector2(0.4f, 0), spd * 0.8f, dmg);
                    break;
            }
            AudioManager.Instance?.PlaySfx(SfxId.Shoot);
        }

        private void Shot(Vector2 origin, Vector2 dir, float spd, int dmg)
            => BulletSpawner.Fire(origin, Team.Player, dir, spd, dmg, BulletColor, 0.12f);

        private void Homing(Vector2 origin, float spd, int dmg)
            => BulletSpawner.Fire(origin, Team.Player, Vector2.up, spd, dmg, new Color(0.6f, 0.9f, 1f), 0.14f, true);

        private static Vector2 Rotate(Vector2 v, float deg)
        {
            float r = deg * Mathf.Deg2Rad;
            float cos = Mathf.Cos(r), sin = Mathf.Sin(r);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        public void AddFirepower(int crystals)
        {
            for (int i = 0; i < crystals; i++)
            {
                if (level >= 5) { insulinCollected = 0; break; }
                insulinCollected++;
                if (insulinCollected >= 2) // 升级所需结晶数 = 当前等级
                {
                    level++;
                    insulinCollected = 0;
                    PrefabFactory.Instance?.SpawnVfx(transform.position, BulletColor, 0.5f, 2f, 0.4f);
                    AudioManager.Instance?.PlaySfx(SfxId.Upgrade);
                }
            }
            EventBus.FirepowerChanged(level);
        }

        public void ReduceFirepower(int levels)
        {
            level = Mathf.Max(1, level - levels);
            insulinCollected = 0;
            EventBus.FirepowerChanged(level);
        }

        public void ApplyFirepowerDebuff(float seconds) => firepowerDebuff = seconds;
    }
}
