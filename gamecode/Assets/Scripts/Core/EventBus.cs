using System;
using UnityEngine;

namespace GlucoseWar.Core
{
    /// <summary>
    /// Lightweight global event bus. Gameplay publishes; UI/Audio subscribe.
    /// Keeps the presentation layer decoupled from gameplay (design rule #2).
    /// </summary>
    public static class EventBus
    {
        public static event Action<int, int> OnPlayerHealthChanged;      // currentHp, maxHp
        public static event Action<float, float> OnPlayerShieldChanged;  // current, max
        public static event Action<int> OnFirepowerChanged;              // level 1-5
        public static event Action<float> OnUltimateChargeChanged;       // 0..1
        public static event Action OnUltimateFired;
        public static event Action OnPlayerDamaged;
        public static event Action OnPlayerDied;

        public static event Action<Vector3, int> OnEnemyKilled;          // position, score
        public static event Action<string> OnKnowledgeUnlocked;          // knowledge id

        public static event Action<int> OnScoreChanged;                  // total score
        public static event Action<float, float> OnLevelProgress;        // elapsed, total
        public static event Action<float, float, string> OnBossHealth;   // current, max, name
        public static event Action<string> OnBossAppeared;               // boss name
        public static event Action OnBossDefeated;
        public static event Action<bool> OnLevelEnded;                   // cleared?

        public static void PlayerHealthChanged(int hp, int max) => OnPlayerHealthChanged?.Invoke(hp, max);
        public static void PlayerShieldChanged(float v, float max) => OnPlayerShieldChanged?.Invoke(v, max);
        public static void FirepowerChanged(int lv) => OnFirepowerChanged?.Invoke(lv);
        public static void UltimateChargeChanged(float n) => OnUltimateChargeChanged?.Invoke(n);
        public static void UltimateFired() => OnUltimateFired?.Invoke();
        public static void PlayerDamaged() => OnPlayerDamaged?.Invoke();
        public static void PlayerDied() => OnPlayerDied?.Invoke();

        public static void EnemyKilled(Vector3 pos, int score) => OnEnemyKilled?.Invoke(pos, score);
        public static void KnowledgeUnlocked(string id) => OnKnowledgeUnlocked?.Invoke(id);

        public static void ScoreChanged(int s) => OnScoreChanged?.Invoke(s);
        public static void LevelProgress(float e, float t) => OnLevelProgress?.Invoke(e, t);
        public static void BossHealth(float c, float m, string n) => OnBossHealth?.Invoke(c, m, n);
        public static void BossAppeared(string n) => OnBossAppeared?.Invoke(n);
        public static void BossDefeated() => OnBossDefeated?.Invoke();
        public static void LevelEnded(bool cleared) => OnLevelEnded?.Invoke(cleared);

        /// <summary>Clear all subscribers (call on hard scene/game reset).</summary>
        public static void Reset()
        {
            OnPlayerHealthChanged = null; OnPlayerShieldChanged = null; OnFirepowerChanged = null;
            OnUltimateChargeChanged = null; OnUltimateFired = null; OnPlayerDamaged = null; OnPlayerDied = null;
            OnEnemyKilled = null; OnKnowledgeUnlocked = null; OnScoreChanged = null; OnLevelProgress = null;
            OnBossHealth = null; OnBossAppeared = null; OnBossDefeated = null; OnLevelEnded = null;
        }
    }
}
