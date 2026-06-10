using GlucoseWar.Core;
using GlucoseWar.Player;
using UnityEngine;
using UnityEngine.UI;

namespace GlucoseWar.UI
{
    /// <summary>HUD：生命/护盾/大招/得分/关卡进度/Boss 血条。订阅事件刷新。</summary>
    public class HUDView : MonoBehaviour
    {
        private Text livesText, scoreText, firepowerText, ultimateText, bossNameText, hintText;
        private Image shieldFill, ultimateFill, progressFill, bossFill;
        private RectTransform bossGroup;
        private float hintTimer;

        public void Build(Transform parent)
        {
            var panel = new GameObject("HUD");
            panel.transform.SetParent(parent, false);
            var rt = UIFactory.Rect(panel);
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            transform.SetParent(panel.transform, false);

            // 左上：命数 + 护盾
            livesText = UIFactory.Label(panel.transform, "♥ x3", 22, UIFactory.Green,
                new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20), new Vector2(200, 30), TextAnchor.UpperLeft);
            UIFactory.Label(panel.transform, "护盾", 16, UIFactory.Cyan,
                new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -54), new Vector2(60, 24), TextAnchor.UpperLeft);
            shieldFill = UIFactory.Bar(panel.transform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(80, -56),
                new Vector2(200, 18), new Color(0.1f, 0.1f, 0.15f, 0.8f), UIFactory.Cyan, out _);

            // 左下：大招能量
            UIFactory.Label(panel.transform, "胰岛素脉冲", 16, UIFactory.Cyan,
                new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 46), new Vector2(140, 24), TextAnchor.LowerLeft);
            ultimateFill = UIFactory.Bar(panel.transform, new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 22),
                new Vector2(220, 18), new Color(0.1f, 0.1f, 0.15f, 0.8f), new Color(0.3f, 0.8f, 1f), out _);
            ultimateText = UIFactory.Label(panel.transform, "", 14, Color.white,
                new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(250, 22), new Vector2(120, 24), TextAnchor.LowerLeft);

            // 右上：得分 + 火力
            scoreText = UIFactory.Label(panel.transform, "得分 0", 22, Color.white,
                new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -20), new Vector2(240, 30), TextAnchor.UpperRight);
            firepowerText = UIFactory.Label(panel.transform, "火力 Lv1", 18, UIFactory.Orange,
                new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-20, -52), new Vector2(240, 26), TextAnchor.UpperRight);

            // 顶部中：关卡进度（心电图）
            progressFill = UIFactory.Bar(panel.transform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(-150, -20),
                new Vector2(300, 10), new Color(0.1f, 0.1f, 0.15f, 0.6f), new Color(0.6f, 1f, 0.6f), out _);

            // Boss 血条（默认隐藏）
            var bossGo = new GameObject("BossBar");
            bossGo.transform.SetParent(panel.transform, false);
            bossGroup = UIFactory.Rect(bossGo);
            bossGroup.anchorMin = new Vector2(0.5f, 1); bossGroup.anchorMax = new Vector2(0.5f, 1);
            bossGroup.pivot = new Vector2(0.5f, 1); bossGroup.anchoredPosition = new Vector2(0, -40);
            bossGroup.sizeDelta = new Vector2(560, 50);
            bossNameText = UIFactory.Label(bossGo.transform, "", 18, UIFactory.Red,
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, 0), new Vector2(0, 24), TextAnchor.MiddleCenter);
            bossFill = UIFactory.Bar(bossGo.transform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 4),
                new Vector2(0, 16), new Color(0.15f, 0.02f, 0.05f, 0.9f), UIFactory.Red, out var bc);
            bc.offsetMin = new Vector2(10, 4); bc.offsetMax = new Vector2(-10, 20);
            bossGo.SetActive(false);

            // 底部中：科普角标提示
            hintText = UIFactory.Label(panel.transform, "", 18, new Color(1f, 0.9f, 0.4f),
                new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 80), new Vector2(600, 30), TextAnchor.MiddleCenter);

            Subscribe();
        }

        private void Subscribe()
        {
            EventBus.OnPlayerHealthChanged += OnLives;
            EventBus.OnPlayerShieldChanged += OnShield;
            EventBus.OnFirepowerChanged += OnFirepower;
            EventBus.OnUltimateChargeChanged += OnUltimate;
            EventBus.OnScoreChanged += OnScore;
            EventBus.OnLevelProgress += OnProgress;
            EventBus.OnBossAppeared += OnBossAppeared;
            EventBus.OnBossHealth += OnBossHealth;
            EventBus.OnBossDefeated += OnBossDefeated;
            EventBus.OnKnowledgeUnlocked += OnKnowledge;
        }

        private void OnDestroy()
        {
            EventBus.OnPlayerHealthChanged -= OnLives;
            EventBus.OnPlayerShieldChanged -= OnShield;
            EventBus.OnFirepowerChanged -= OnFirepower;
            EventBus.OnUltimateChargeChanged -= OnUltimate;
            EventBus.OnScoreChanged -= OnScore;
            EventBus.OnLevelProgress -= OnProgress;
            EventBus.OnBossAppeared -= OnBossAppeared;
            EventBus.OnBossHealth -= OnBossHealth;
            EventBus.OnBossDefeated -= OnBossDefeated;
            EventBus.OnKnowledgeUnlocked -= OnKnowledge;
        }

        private void Update()
        {
            if (hintTimer > 0f)
            {
                hintTimer -= Time.unscaledDeltaTime;
                if (hintTimer <= 0f && hintText != null) hintText.text = "";
            }
        }

        private void OnLives(int hp, int max) { if (livesText != null) livesText.text = $"♥ x{hp}"; }
        private void OnShield(float v, float max) { if (shieldFill != null) shieldFill.fillAmount = max > 0 ? v / max : 0; }
        private void OnFirepower(int lv) { if (firepowerText != null) firepowerText.text = $"火力 Lv{lv}"; }
        private void OnUltimate(float n)
        {
            if (ultimateFill != null) ultimateFill.fillAmount = n;
            if (ultimateText != null) ultimateText.text = n >= 1f ? "就绪! [J/空格]" : $"{Mathf.RoundToInt(n * 100)}%";
        }
        private void OnScore(int s) { if (scoreText != null) scoreText.text = $"得分 {s}"; }
        private void OnProgress(float e, float t) { if (progressFill != null) progressFill.fillAmount = t > 0 ? e / t : 0; }
        private void OnBossAppeared(string n)
        {
            if (bossGroup != null) bossGroup.gameObject.SetActive(true);
            if (bossNameText != null) bossNameText.text = $"⚠ {n}";
        }
        private void OnBossHealth(float c, float m, string n) { if (bossFill != null) bossFill.fillAmount = m > 0 ? c / m : 0; }
        private void OnBossDefeated() { if (bossGroup != null) bossGroup.gameObject.SetActive(false); }
        private void OnKnowledge(string id)
        {
            var card = KnowledgeManager.GetCard(id);
            if (card != null && hintText != null)
            {
                hintText.text = $"📖 解锁科普：{card.title}";
                hintTimer = 2.5f;
            }
        }
    }
}
