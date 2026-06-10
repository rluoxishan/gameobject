using System.Collections.Generic;
using GlucoseWar.Core;
using GlucoseWar.Data;
using UnityEngine;

namespace GlucoseWar
{
    /// <summary>科普：通关弹卡、死亡随机提示、图鉴解锁记录。</summary>
    public class KnowledgeManager : Singleton<KnowledgeManager>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable() => EventBus.OnKnowledgeUnlocked += Unlock;
        private void OnDisable() => EventBus.OnKnowledgeUnlocked -= Unlock;

        private void Unlock(string id)
        {
            SaveSystem.Instance?.UnlockKnowledge(id);
        }

        public static KnowledgeCard GetCard(string id) => GameDatabase.GetCard(id);

        public static string RandomDeathCardId()
        {
            GameDatabase.EnsureBuilt();
            var deathCards = new List<string>();
            foreach (var kv in GameDatabase.Knowledge)
                if (kv.Value.trigger == TriggerType.Death) deathCards.Add(kv.Key);
            if (deathCards.Count == 0) return "K05";
            return deathCards[Random.Range(0, deathCards.Count)];
        }

        public static List<KnowledgeCard> AllCards()
        {
            GameDatabase.EnsureBuilt();
            var list = new List<KnowledgeCard>(GameDatabase.Knowledge.Values);
            list.Sort((a, b) => string.Compare(a.id, b.id, System.StringComparison.Ordinal));
            return list;
        }
    }
}
