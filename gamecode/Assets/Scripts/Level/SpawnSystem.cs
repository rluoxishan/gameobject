using System.Collections;
using System.Collections.Generic;
using GlucoseWar.Core;
using GlucoseWar.Data;
using GlucoseWar.Enemies;
using UnityEngine;

namespace GlucoseWar.Level
{
    /// <summary>读取关卡波次数据，按触发时间从对象池生成敌人；Boss 波次回调 LevelManager。</summary>
    public class SpawnSystem : MonoBehaviour
    {
        private LevelTimeline level;
        private System.Action<WaveEntry> onBossWave;
        private int nextWave;
        private float topY, halfW;

        public void Setup(LevelTimeline timeline, System.Action<WaveEntry> bossWaveCallback)
        {
            level = timeline;
            onBossWave = bossWaveCallback;
            nextWave = 0;
            var cam = Camera.main;
            float v = cam != null ? cam.orthographicSize : 8f;
            topY = v + 1.2f;
            halfW = v * (cam != null ? cam.aspect : 0.56f) * 0.85f;
        }

        public void Tick(float elapsed)
        {
            while (nextWave < level.waves.Count && elapsed >= level.waves[nextWave].triggerTime)
            {
                var wave = level.waves[nextWave];
                nextWave++;
                if (wave.isBoss) { onBossWave?.Invoke(wave); }
                else StartCoroutine(SpawnWave(wave));
            }
        }

        public bool AllWavesDispatched => nextWave >= level.waves.Count;

        private IEnumerator SpawnWave(WaveEntry wave)
        {
            for (int i = 0; i < wave.count; i++)
            {
                SpawnOne(wave, i);
                if (wave.spawnInterval > 0f) yield return new WaitForSeconds(wave.spawnInterval);
            }
        }

        private void SpawnOne(WaveEntry wave, int index)
        {
            EnemyData data;
            if (wave.isMixed)
            {
                var ids = GameDatabase.MixEnemiesFor(level.levelId);
                data = GameDatabase.GetEnemy(ids[Random.Range(0, ids.Count)]);
            }
            else
            {
                data = GameDatabase.GetEnemy(wave.enemyId);
            }
            if (data == null) return;

            Vector3 pos = ResolvePosition(wave.spawnPosition, index, wave.count);
            var enemy = EnemyBase.Spawn(data, pos);
            if (enemy != null) enemy.SetDrop(wave.hasDrop, wave.dropItem, wave.dropRate);
        }

        private Vector3 ResolvePosition(string spawnPos, int index, int count)
        {
            float x;
            switch (spawnPos)
            {
                case "顶部均匀":
                    x = count > 1 ? Mathf.Lerp(-halfW, halfW, (float)index / (count - 1)) : 0f;
                    break;
                case "左右两侧":
                case "两侧":
                    x = (index % 2 == 0 ? -1 : 1) * halfW;
                    break;
                case "两侧交替":
                    x = (index % 2 == 0 ? -1 : 1) * halfW * 0.8f;
                    break;
                case "顶部分散":
                    x = Mathf.Lerp(-halfW * 0.8f, halfW * 0.8f, count > 1 ? (float)index / (count - 1) : 0.5f);
                    break;
                case "屏幕中上":
                    x = 0f;
                    break;
                case "全屏":
                    x = Random.Range(-halfW, halfW);
                    break;
                default: // 顶部随机
                    x = Random.Range(-halfW, halfW);
                    break;
            }
            return new Vector3(x, topY, 0f);
        }
    }
}
