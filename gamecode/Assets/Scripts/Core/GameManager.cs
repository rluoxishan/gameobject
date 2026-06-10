using GlucoseWar.Data;
using GlucoseWar.Diff;
using GlucoseWar.Items;
using GlucoseWar.Level;
using GlucoseWar.UI;
using UnityEngine;

namespace GlucoseWar.Core
{
    /// <summary>全局状态机：Boot/Menu/Playing/Paused/Result，难度传递与关卡推进。</summary>
    public class GameManager : Singleton<GameManager>
    {
        public GameState State { get; private set; } = GameState.Boot;
        public int CurrentLevelIndex { get; private set; }
        public int Lives { get; private set; }

        private LevelManager levelManager;

        protected override void Awake()
        {
            base.Awake();
            GameDatabase.EnsureBuilt();
            DontDestroyOnLoad(gameObject);
        }

        public void GoToMenu()
        {
            TeardownGameplay();
            State = GameState.Menu;
            Time.timeScale = 1f;
            UIManager.Instance?.ShowMainMenu();
            AudioManager.Instance?.PlayMenuBgm();
        }

        public void StartNewGame(GlucoseWar.Data.Difficulty difficulty, int levelIndex)
        {
            DifficultyService.Set(difficulty);
            Lives = DifficultyService.StartLives;
            CurrentLevelIndex = Mathf.Clamp(levelIndex, 0, GameDatabase.Levels.Count - 1);
            ScoreManager.Instance?.ResetScore();
            BeginCurrentLevel();
        }

        private void BeginCurrentLevel()
        {
            TeardownGameplay();
            State = GameState.Playing;
            Time.timeScale = 1f;

            LevelTimeline level = GameDatabase.Levels[CurrentLevelIndex];
            var go = new GameObject($"LevelManager_{level.levelId}");
            levelManager = go.AddComponent<LevelManager>();
            levelManager.BeginLevel(level, Lives);

            UIManager.Instance?.ShowHUD();
            AudioManager.Instance?.PlayLevelBgm(CurrentLevelIndex);
        }

        public void OnLevelCleared()
        {
            State = GameState.Result;
            string kid = GameDatabase.Levels[CurrentLevelIndex].clearKnowledgeId;
            if (!string.IsNullOrEmpty(kid)) EventBus.KnowledgeUnlocked(kid);
            bool lastLevel = CurrentLevelIndex >= GameDatabase.Levels.Count - 1;
            UIManager.Instance?.ShowResult(true, lastLevel, kid, () =>
            {
                if (lastLevel) GoToMenu();
                else { CurrentLevelIndex++; BeginCurrentLevel(); }
            });
            SaveSystem.Instance?.SubmitScore(ScoreManager.Instance != null ? ScoreManager.Instance.Total : 0);
        }

        public void OnPlayerOutOfLives()
        {
            State = GameState.Result;
            UIManager.Instance?.ShowResult(false, false, KnowledgeManager.RandomDeathCardId(), GoToMenu);
            SaveSystem.Instance?.SubmitScore(ScoreManager.Instance != null ? ScoreManager.Instance.Total : 0);
        }

        public void NotifyLifeLost()
        {
            Lives--;
            DifficultyService.OnPlayerDeath();
            if (Lives < 0) Lives = 0;
        }

        public bool HasLivesLeft => Lives > 0;

        public void TogglePause()
        {
            if (State == GameState.Playing)
            {
                State = GameState.Paused;
                Time.timeScale = 0f;
                UIManager.Instance?.ShowPause();
            }
            else if (State == GameState.Paused)
            {
                Resume();
            }
        }

        public void Resume()
        {
            if (State != GameState.Paused) return;
            State = GameState.Playing;
            Time.timeScale = 1f;
            UIManager.Instance?.HidePause();
        }

        public void RestartLevel()
        {
            Lives = DifficultyService.StartLives;
            ScoreManager.Instance?.ResetScore();
            BeginCurrentLevel();
        }

        private void TeardownGameplay()
        {
            if (levelManager != null)
            {
                Destroy(levelManager.gameObject);
                levelManager = null;
            }
            GlucoseWar.Enemies.EnemyBase.ClearAll();
            GlucoseWar.Bullets.BulletSpawner.ClearAllBullets();
            foreach (var item in FindObjectsOfType<GlucoseWar.Items.PowerUp>())
                if (item.gameObject.activeInHierarchy && ObjectPool.Instance != null)
                    ObjectPool.Instance.Despawn(item.gameObject);
        }
    }
}
