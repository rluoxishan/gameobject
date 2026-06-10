using System.Collections.Generic;
using GlucoseWar.Core;
using UnityEngine;

namespace GlucoseWar
{
    /// <summary>存档：解锁记录 / 本地排行榜 / 设置（PlayerPrefs）。</summary>
    public class SaveSystem : Singleton<SaveSystem>
    {
        private const string ScoreKey = "gw_scores";
        private const string UnlockPrefix = "gw_unlock_";
        private const int MaxScores = 10;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public void SubmitScore(int score)
        {
            if (score <= 0) return;
            var scores = GetScores();
            scores.Add(score);
            scores.Sort((a, b) => b.CompareTo(a));
            if (scores.Count > MaxScores) scores.RemoveRange(MaxScores, scores.Count - MaxScores);
            PlayerPrefs.SetString(ScoreKey, string.Join(",", scores));
            PlayerPrefs.Save();
        }

        public List<int> GetScores()
        {
            var list = new List<int>();
            string raw = PlayerPrefs.GetString(ScoreKey, "");
            if (!string.IsNullOrEmpty(raw))
                foreach (var s in raw.Split(','))
                    if (int.TryParse(s, out int v)) list.Add(v);
            return list;
        }

        public int HighScore
        {
            get { var s = GetScores(); return s.Count > 0 ? s[0] : 0; }
        }

        public void UnlockKnowledge(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            PlayerPrefs.SetInt(UnlockPrefix + id, 1);
        }

        public bool IsUnlocked(string id) => PlayerPrefs.GetInt(UnlockPrefix + id, 0) == 1;

        public float GetFloat(string key, float def) => PlayerPrefs.GetFloat(key, def);
        public void SetFloat(string key, float v) { PlayerPrefs.SetFloat(key, v); PlayerPrefs.Save(); }
    }
}
