using GlucoseWar.Boss;
using GlucoseWar.Bullets;
using GlucoseWar.Core;
using GlucoseWar.Data;
using GlucoseWar.Enemies;
using GlucoseWar.Player;
using UnityEngine;

namespace GlucoseWar.Level
{
    /// <summary>关卡流程：构建场景→时间轴推进→Boss→通关→结算。</summary>
    public class LevelManager : MonoBehaviour
    {
        private LevelTimeline level;
        private SpawnSystem spawnSystem;
        private BackgroundScroller background;
        private GameObject player;
        private float elapsed;
        private bool bossSpawned;
        private bool bossDefeated;
        private bool ended;

        public void BeginLevel(LevelTimeline timeline, int lives)
        {
            level = timeline;
            elapsed = 0f;
            bossSpawned = bossDefeated = ended = false;

            ConfigurePlayField();
            EnemyBase.ClearAll();
            BulletSpawner.ClearAllEnemyBullets();

            // 背景
            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(transform);
            background = bgGo.AddComponent<BackgroundScroller>();
            background.Setup(level);

            // 玩家
            float bottom = Camera.main != null ? -Camera.main.orthographicSize * 0.7f : -5f;
            player = PlayerFactory.Create(new Vector3(0, bottom, 0));
            player.transform.SetParent(transform);

            // 生成系统
            var spawnGo = new GameObject("SpawnSystem");
            spawnGo.transform.SetParent(transform);
            spawnSystem = spawnGo.AddComponent<SpawnSystem>();
            spawnSystem.Setup(level, OnBossWave);

            EventBus.LevelProgress(0f, level.duration);
        }

        private void ConfigurePlayField()
        {
            var cam = Camera.main;
            float v = cam != null ? cam.orthographicSize : 8f;
            float h = v * (cam != null ? cam.aspect : 0.56f);
            Bullet.PlayField = new Bounds(Vector3.zero, new Vector3((h + 2f) * 2f, (v + 3f) * 2f, 1f));
        }

        private void Update()
        {
            if (ended || GameManager.Instance == null || GameManager.Instance.State != GlucoseWar.Data.GameState.Playing)
                return;

            elapsed += Time.deltaTime;
            spawnSystem.Tick(elapsed);
            EventBus.LevelProgress(Mathf.Min(elapsed, level.duration), level.duration);
        }

        private void OnBossWave(WaveEntry wave)
        {
            if (bossSpawned) return;
            bossSpawned = true;
            var bossData = GameDatabase.GetBoss(level.bossId);
            if (bossData == null) { CompleteLevel(); return; }

            var go = new GameObject($"Boss_{bossData.bossId}");
            go.transform.SetParent(transform);
            var boss = go.AddComponent<BossController>();
            boss.Init(bossData, OnBossDefeated);
        }

        private void OnBossDefeated()
        {
            if (bossDefeated) return;
            bossDefeated = true;
            Invoke(nameof(CompleteLevel), 1.2f);
        }

        private void CompleteLevel()
        {
            if (ended) return;
            ended = true;
            EnemyBase.ClearAll();
            BulletSpawner.ClearAllEnemyBullets();
            EventBus.LevelEnded(true);
            GameManager.Instance?.OnLevelCleared();
        }
    }
}
