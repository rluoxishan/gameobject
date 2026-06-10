using System.Collections.Generic;
using GlucoseWar.Core;
using GlucoseWar.Data;
using GlucoseWar.Items;
using UnityEngine;
using UnityEngine.UI;

namespace GlucoseWar.UI
{
    /// <summary>界面栈管理：菜单/HUD/暂停/结算/科普卡/档案库/设置 + 全屏覆盖效果。</summary>
    public class UIManager : Singleton<UIManager>
    {
        private Canvas canvas;
        private Transform root;
        private readonly List<GameObject> screens = new List<GameObject>();

        private HUDView hud;
        private GameObject hudPanel;
        private GameObject pausePanel;
        private Image flashOverlay;
        private Image blindOverlay;
        private float flashTimer, blindTimer, blindDuration;

        private int selectedLevel;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            BuildCanvas();
        }

        private void BuildCanvas()
        {
            var canvasGo = new GameObject("UICanvas");
            canvasGo.transform.SetParent(transform);
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();
            root = canvasGo.transform;

            // 致盲覆盖（Boss2）
            blindOverlay = UIFactory.Panel(root, "BlindOverlay", new Color(0.1f, 0f, 0f, 0f)).GetComponent<Image>();
            blindOverlay.raycastTarget = false;
            // 受击/陷阱泛红
            flashOverlay = UIFactory.Panel(root, "FlashOverlay", new Color(0.8f, 0f, 0f, 0f)).GetComponent<Image>();
            flashOverlay.raycastTarget = false;
        }

        private void Update()
        {
            if (flashTimer > 0f)
            {
                flashTimer -= Time.unscaledDeltaTime;
                var c = flashOverlay.color; c.a = Mathf.Max(0f, flashTimer) * 0.5f; flashOverlay.color = c;
            }
            if (blindTimer > 0f)
            {
                blindTimer -= Time.unscaledDeltaTime;
                float n = blindTimer / blindDuration;
                var c = blindOverlay.color; c.a = Mathf.Sin(n * Mathf.PI) * 0.85f; blindOverlay.color = c;
            }

            if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) && GameManager.Instance != null)
            {
                var st = GameManager.Instance.State;
                if (st == GameState.Playing || st == GameState.Paused)
                    GameManager.Instance.TogglePause();
            }
        }

        private GameObject NewScreen(string name)
        {
            var go = UIFactory.Panel(root, name, UIFactory.PanelBg);
            screens.Add(go);
            return go;
        }

        private void HideAllScreens()
        {
            foreach (var s in screens) if (s != null) Destroy(s);
            screens.Clear();
            if (hudPanel != null) hudPanel.SetActive(false);
        }

        // ---------------- 主菜单 ----------------
        public void ShowMainMenu()
        {
            HideAllScreens();
            var panel = NewScreen("MainMenu");
            UIFactory.Label(panel.transform, "糖战 · 胰岛防线", 56, UIFactory.Red,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -180), new Vector2(900, 80));
            UIFactory.Label(panel.transform, "Glucose War: The Last Islet", 24, UIFactory.Cyan,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -250), new Vector2(900, 40));
            UIFactory.Label(panel.transform, "纵版弹幕射击 · 糖尿病科普", 18, new Color(0.8f, 0.8f, 0.8f),
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -290), new Vector2(900, 30));

            UIFactory.Button(panel.transform, "开始游戏", new Vector2(0, 60), new Vector2(360, 70), UIFactory.Cyan, ShowDifficultySelect);
            UIFactory.Button(panel.transform, "医学档案库", new Vector2(0, -30), new Vector2(360, 70), new Color(0.2f, 0.3f, 0.4f), ShowArchive);
            UIFactory.Button(panel.transform, "设置", new Vector2(0, -120), new Vector2(360, 70), new Color(0.25f, 0.25f, 0.3f), ShowSettings);
            UIFactory.Button(panel.transform, "退出", new Vector2(0, -210), new Vector2(360, 70), new Color(0.35f, 0.1f, 0.12f), QuitGame);

            int high = SaveSystem.Instance != null ? SaveSystem.Instance.HighScore : 0;
            UIFactory.Label(panel.transform, $"最高分：{high}", 18, new Color(1f, 0.9f, 0.4f),
                new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 60), new Vector2(400, 30));
            UIFactory.Label(panel.transform, "移动 WASD/方向键 · 大招 J/空格/Shift · 暂停 Esc · 自动开火",
                14, new Color(0.7f, 0.7f, 0.7f),
                new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 24), new Vector2(900, 24));
        }

        // ---------------- 难度 / 关卡选择 ----------------
        public void ShowDifficultySelect()
        {
            HideAllScreens();
            var panel = NewScreen("DifficultySelect");
            UIFactory.Label(panel.transform, "选择关卡与难度", 40, Color.white,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -160), new Vector2(900, 60));

            string[] levelNames = { "关一 · 血潮", "关二 · 盲视", "关三 · 核心" };
            for (int i = 0; i < levelNames.Length && i < GameDatabase.Levels.Count; i++)
            {
                int idx = i;
                var c = idx == selectedLevel ? UIFactory.Cyan : new Color(0.2f, 0.3f, 0.4f);
                UIFactory.Button(panel.transform, levelNames[i], new Vector2(-280 + i * 280, 120), new Vector2(250, 70), c,
                    () => { selectedLevel = idx; ShowDifficultySelect(); });
            }

            UIFactory.Label(panel.transform, "难度：", 24, Color.white,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 20), new Vector2(400, 30));
            UIFactory.Button(panel.transform, "简单·科普", new Vector2(-280, -60), new Vector2(250, 70), UIFactory.Green,
                () => StartGame(Difficulty.Easy));
            UIFactory.Button(panel.transform, "普通·标准", new Vector2(0, -60), new Vector2(250, 70), UIFactory.Orange,
                () => StartGame(Difficulty.Normal));
            UIFactory.Button(panel.transform, "困难·生存", new Vector2(280, -60), new Vector2(250, 70), UIFactory.Red,
                () => StartGame(Difficulty.Hard));

            UIFactory.Button(panel.transform, "返回", new Vector2(0, -200), new Vector2(200, 60), new Color(0.25f, 0.25f, 0.3f), ShowMainMenu);
        }

        private void StartGame(Difficulty difficulty)
        {
            GameManager.Instance.StartNewGame(difficulty, selectedLevel);
        }

        // ---------------- HUD ----------------
        public void ShowHUD()
        {
            HideAllScreens();
            if (hudPanel == null)
            {
                var hudGo = new GameObject("HUDView");
                hudGo.transform.SetParent(root, false);
                hud = hudGo.AddComponent<HUDView>();
                hud.Build(root);
                hudPanel = root.Find("HUD")?.gameObject;
            }
            if (hudPanel != null) { hudPanel.SetActive(true); hudPanel.transform.SetAsLastSibling(); }
            flashOverlay.transform.SetAsLastSibling();
            blindOverlay.transform.SetAsLastSibling();
        }

        // ---------------- 暂停 ----------------
        public void ShowPause()
        {
            if (pausePanel == null)
            {
                pausePanel = UIFactory.Panel(root, "Pause", new Color(0, 0, 0, 0.7f));
                UIFactory.Label(pausePanel.transform, "已暂停", 48, Color.white,
                    new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 160), new Vector2(400, 60));
                UIFactory.Button(pausePanel.transform, "继续", new Vector2(0, 60), new Vector2(300, 64), UIFactory.Cyan,
                    () => GameManager.Instance.Resume());
                UIFactory.Button(pausePanel.transform, "重新开始本关", new Vector2(0, -20), new Vector2(300, 64), UIFactory.Orange,
                    () => { GameManager.Instance.Resume(); GameManager.Instance.RestartLevel(); });
                UIFactory.Button(pausePanel.transform, "返回主菜单", new Vector2(0, -100), new Vector2(300, 64), new Color(0.35f, 0.1f, 0.12f),
                    () => GameManager.Instance.GoToMenu());
            }
            pausePanel.SetActive(true);
            pausePanel.transform.SetAsLastSibling();
        }

        public void HidePause()
        {
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        // ---------------- 结算 / 科普卡 ----------------
        public void ShowResult(bool cleared, bool lastLevel, string knowledgeId, System.Action onContinue)
        {
            HideAllScreens();
            var panel = NewScreen("Result");
            string title = cleared ? (lastLevel ? "胜利 · 守住胰岛防线!" : "关卡通过!") : "战机损毁 · 病情恶化";
            Color titleColor = cleared ? UIFactory.Green : UIFactory.Red;
            UIFactory.Label(panel.transform, title, 44, titleColor,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -150), new Vector2(900, 70));

            int score = ScoreManager.Instance != null ? ScoreManager.Instance.Total : 0;
            int high = SaveSystem.Instance != null ? SaveSystem.Instance.HighScore : 0;
            UIFactory.Label(panel.transform, $"本局得分：{score}\n历史最高：{high}", 26, Color.white,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -260), new Vector2(700, 80));

            var card = KnowledgeManager.GetCard(knowledgeId);
            if (card != null)
            {
                var box = UIFactory.Panel(panel.transform, "Card", new Color(0.1f, 0.05f, 0.08f, 0.95f), false);
                var brt = UIFactory.Rect(box);
                brt.anchorMin = new Vector2(0.5f, 0.5f); brt.anchorMax = new Vector2(0.5f, 0.5f);
                brt.pivot = new Vector2(0.5f, 0.5f); brt.anchoredPosition = new Vector2(0, 30); brt.sizeDelta = new Vector2(760, 260);
                UIFactory.Label(box.transform, $"📖 病理档案 {card.id} · {card.title}", 24, new Color(1f, 0.9f, 0.4f),
                    new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -20), new Vector2(-40, 40));
                UIFactory.Label(box.transform, card.body, 20, Color.white,
                    new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, -10), new Vector2(-60, -80));
                UIFactory.Label(box.transform, $"— {card.source}", 14, new Color(0.7f, 0.7f, 0.7f),
                    new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, 14), new Vector2(-40, 24), TextAnchor.LowerRight);
            }

            string btnLabel = cleared ? (lastLevel ? "返回主菜单" : "进入下一关") : "返回主菜单";
            UIFactory.Button(panel.transform, btnLabel, new Vector2(0, -200), new Vector2(320, 70), UIFactory.Cyan,
                () => onContinue?.Invoke());
        }

        // ---------------- 医学档案库 ----------------
        public void ShowArchive()
        {
            HideAllScreens();
            var panel = NewScreen("Archive");
            UIFactory.Label(panel.transform, "医学档案库 · 科普图鉴", 40, UIFactory.Cyan,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -120), new Vector2(900, 60));

            var cards = KnowledgeManager.AllCards();
            float y = -200;
            foreach (var card in cards)
            {
                bool unlocked = SaveSystem.Instance == null || SaveSystem.Instance.IsUnlocked(card.id) || card.trigger == TriggerType.Archive;
                string text = unlocked ? $"{card.id}  {card.title} — {card.body}" : $"{card.id}  ??? （未解锁）";
                Color col = unlocked ? Color.white : new Color(0.45f, 0.45f, 0.45f);
                UIFactory.Label(panel.transform, text, 17, col,
                    new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, y), new Vector2(880, 40), TextAnchor.UpperLeft);
                y -= 110;
            }

            UIFactory.Button(panel.transform, "返回", new Vector2(0, -780), new Vector2(200, 60), new Color(0.25f, 0.25f, 0.3f), ShowMainMenu);
        }

        // ---------------- 设置 ----------------
        public void ShowSettings()
        {
            HideAllScreens();
            var panel = NewScreen("Settings");
            UIFactory.Label(panel.transform, "设置", 40, Color.white,
                new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -160), new Vector2(400, 60));

            float vol = SaveSystem.Instance != null ? SaveSystem.Instance.GetFloat("master", 0.6f) : 0.6f;
            var volLabel = UIFactory.Label(panel.transform, $"主音量：{Mathf.RoundToInt(vol * 100)}%", 24, Color.white,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 80), new Vector2(500, 30));
            UIFactory.Button(panel.transform, "-", new Vector2(-160, 20), new Vector2(80, 60), new Color(0.3f, 0.3f, 0.35f),
                () => ChangeVolume(-0.1f, volLabel));
            UIFactory.Button(panel.transform, "+", new Vector2(160, 20), new Vector2(80, 60), new Color(0.3f, 0.3f, 0.35f),
                () => ChangeVolume(0.1f, volLabel));
            UIFactory.Label(panel.transform, "（弹幕'安全帧'与动态难度遵循所选难度，详见策划案）", 16, new Color(0.7f, 0.7f, 0.7f),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -60), new Vector2(800, 30));

            UIFactory.Button(panel.transform, "返回", new Vector2(0, -200), new Vector2(200, 60), new Color(0.25f, 0.25f, 0.3f), ShowMainMenu);
        }

        private void ChangeVolume(float delta, Text label)
        {
            float v = Mathf.Clamp01((SaveSystem.Instance != null ? SaveSystem.Instance.GetFloat("master", 0.6f) : 0.6f) + delta);
            SaveSystem.Instance?.SetFloat("master", v);
            AudioManager.Instance?.SetVolumes(v, 0.5f, 0.3f);
            if (label != null) label.text = $"主音量：{Mathf.RoundToInt(v * 100)}%";
        }

        // ---------------- 覆盖效果 ----------------
        public void FlashRed() { flashTimer = 1f; }
        public void PlayBlind(float duration) { blindDuration = Mathf.Max(0.1f, duration); blindTimer = duration; }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
