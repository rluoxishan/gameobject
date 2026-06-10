using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlucoseWar.Data
{
    /// <summary>单条波次（对应《关卡波次配置表.csv》的一行）。</summary>
    [Serializable]
    public class WaveEntry
    {
        public string waveId;        // L1-W01
        public float triggerTime;    // 触发时间s
        public string segment;       // 段落
        public string enemyId;       // 关联敌人/Boss
        public int count = 1;
        public string spawnPosition; // 顶部均匀/两侧/随机/全屏/屏幕中上
        public float spawnInterval = 0.4f;
        public ItemType dropItem = ItemType.Insulin;
        public bool hasDrop = false;
        public float dropRate = 0f;
        public string knowledgeId = "";
        public bool isBoss = false;
        public bool isMixed = false; // 混合波
        public string note = "";
    }

    /// <summary>关卡时间轴：波次列表 + Boss + 时长 + 背景 + 通关科普。</summary>
    [CreateAssetMenu(menuName = "糖战/关卡时间轴", fileName = "LevelTimeline")]
    public class LevelTimeline : ScriptableObject
    {
        public string levelId = "L1";
        public string levelName = "血潮";
        public List<WaveEntry> waves = new List<WaveEntry>();
        public string bossId = "B01";
        public float duration = 90f;
        public string clearKnowledgeId = "K02";

        [Header("背景视差（程序生成色带）")]
        public Color farColor = new Color(0.18f, 0.02f, 0.04f);
        public Color midColor = new Color(0.35f, 0.05f, 0.08f);
        public Color nearColor = new Color(0.55f, 0.08f, 0.12f);
    }
}
