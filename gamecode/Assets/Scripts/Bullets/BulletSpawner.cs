using GlucoseWar.Core;
using GlucoseWar.Data;
using GlucoseWar.Diff;
using UnityEngine;

namespace GlucoseWar.Bullets
{
    /// <summary>按 BulletPatternData 发射弹幕（角度/数量/速度/扩散）。数据驱动。</summary>
    public static class BulletSpawner
    {
        private const int MaxOnScreen = 600;
        private static int liveEnemyBullets;

        public static Bullet Fire(Vector2 origin, Team team, Vector2 dir, float speed, int damage, Color color, float radius, bool homing = false)
        {
            if (ObjectPool.Instance == null || PrefabFactory.Instance == null) return null;
            var go = ObjectPool.Instance.Spawn(PrefabFactory.Instance.BulletTemplate, origin, Quaternion.identity);
            var b = go.GetComponent<Bullet>();
            b.Init(team, dir, speed, damage, color, radius, homing);
            return b;
        }

        public static void Emit(Vector2 origin, Team team, BulletPatternData pattern, Vector2 aimDir)
        {
            if (pattern == null || pattern.type == PatternType.None) return;

            float speedFactor = team == Team.Enemy ? DifficultyService.BulletSpeed : 1f;
            float densityFactor = team == Team.Enemy ? DifficultyService.BulletDensity : 1f;
            float speed = pattern.bulletSpeed * speedFactor;
            float radius = team == Team.Enemy ? 0.14f : 0.12f;

            int count = Mathf.Max(1, Mathf.RoundToInt(pattern.bulletCount * (pattern.bulletCount > 1 ? densityFactor : 1f)));
            float baseAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

            bool ring = pattern.spreadAngle >= 359f;
            for (int i = 0; i < count; i++)
            {
                float angle;
                if (ring)
                {
                    angle = baseAngle + (360f / count) * i;
                }
                else if (count == 1)
                {
                    angle = baseAngle;
                }
                else
                {
                    float t = (float)i / (count - 1) - 0.5f;
                    angle = baseAngle + t * pattern.spreadAngle;
                }
                Vector2 d = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                Fire(origin, team, d, speed, pattern.damage, pattern.color, radius);
            }
        }

        public static void ClearAllEnemyBullets()
        {
            var bullets = Object.FindObjectsOfType<Bullet>();
            foreach (var b in bullets)
                if (b.Team == Team.Enemy && b.gameObject.activeInHierarchy)
                    b.Recycle();
        }

        public static void ClearAllBullets()
        {
            var bullets = Object.FindObjectsOfType<Bullet>();
            foreach (var b in bullets)
                if (b.gameObject.activeInHierarchy)
                    b.Recycle();
        }
    }
}
