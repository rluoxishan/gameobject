using GlucoseWar.Core;
using GlucoseWar.Diff;
using UnityEngine;

namespace GlucoseWar.Items
{
    /// <summary>计分：击杀/收集/时间/难度系数/受击扣分。</summary>
    public class ScoreManager : Singleton<ScoreManager>
    {
        private int killScore;
        private int collectCount;
        private int hitCount;

        public int Total { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            EventBus.OnEnemyKilled += OnEnemyKilled;
            EventBus.OnPlayerDamaged += OnPlayerDamaged;
        }

        private void OnDisable()
        {
            EventBus.OnEnemyKilled -= OnEnemyKilled;
            EventBus.OnPlayerDamaged -= OnPlayerDamaged;
        }

        public void ResetScore()
        {
            killScore = 0; collectCount = 0; hitCount = 0; Total = 0;
            EventBus.ScoreChanged(Total);
        }

        private void OnEnemyKilled(Vector3 pos, int score)
        {
            killScore += score;
            Recalculate();
        }

        public void AddCollect()
        {
            collectCount++;
            Recalculate();
        }

        private void OnPlayerDamaged()
        {
            hitCount++;
            Recalculate();
        }

        private void Recalculate()
        {
            int collectScore = collectCount * 50;
            float baseSum = killScore + collectScore;
            float diffBonus = baseSum * (DifficultyService.ScoreFactor - 1f);
            int penalty = hitCount * 30;
            Total = Mathf.Max(0, Mathf.RoundToInt(baseSum + diffBonus) - penalty);
            EventBus.ScoreChanged(Total);
        }
    }
}
