using UnityEngine;
using UnityEngine.UI;

namespace GlucoseWar.UI
{
    /// <summary>程序化 UGUI 构建助手（无外部美术，使用内置字体）。</summary>
    public static class UIFactory
    {
        private static Font cachedFont;

        public static Font Font
        {
            get
            {
                if (cachedFont == null)
                {
                    // 优先使用含中文字形的系统字体（Windows），否则回退到内置字体
                    string[] cjk = { "Microsoft YaHei", "微软雅黑", "SimHei", "黑体", "SimSun", "宋体", "PingFang SC", "Heiti SC" };
                    string[] available = Font.GetOSInstalledFontNames();
                    foreach (var name in cjk)
                    {
                        if (System.Array.IndexOf(available, name) >= 0)
                        {
                            cachedFont = Font.CreateDynamicFontFromOSFont(name, 28);
                            break;
                        }
                    }
                    if (cachedFont == null) cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    if (cachedFont == null) cachedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }
                return cachedFont;
            }
        }

        public static readonly Color Cyan = new Color(0.17f, 0.66f, 1f);
        public static readonly Color Green = new Color(0.22f, 1f, 0.08f);
        public static readonly Color Orange = new Color(1f, 0.55f, 0.1f);
        public static readonly Color Red = new Color(0.85f, 0.12f, 0.18f);
        public static readonly Color PanelBg = new Color(0.04f, 0.02f, 0.05f, 0.92f);

        public static RectTransform Rect(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            return rt;
        }

        private static Sprite White => GlucoseWar.Core.ProceduralSprites.Get(GlucoseWar.Core.ProceduralSprites.Shape.Square, Color.white, 8);

        public static GameObject Panel(Transform parent, string name, Color color, bool fullScreen = true)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.sprite = White;
            img.color = color;
            var rt = Rect(go);
            if (fullScreen)
            {
                rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            }
            return go;
        }

        public static Text Label(Transform parent, string text, int size, Color color,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta,
            TextAnchor align = TextAnchor.MiddleCenter)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>();
            t.font = Font;
            t.text = text;
            t.fontSize = size;
            t.color = color;
            t.alignment = align;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            var rt = Rect(go);
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax; rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos; rt.sizeDelta = sizeDelta;
            return t;
        }

        public static Button Button(Transform parent, string label, Vector2 anchoredPos, Vector2 size,
            Color bg, System.Action onClick)
        {
            var go = new GameObject("Button");
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.sprite = White;
            img.color = bg;
            var btn = go.AddComponent<Button>();
            var rt = Rect(go);
            rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos; rt.sizeDelta = size;

            var txt = Label(go.transform, label, 24, Color.white,
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            var colors = btn.colors;
            colors.highlightedColor = Color.Lerp(bg, Color.white, 0.3f);
            colors.pressedColor = Color.Lerp(bg, Color.black, 0.2f);
            colors.normalColor = bg;
            btn.colors = colors;
            btn.targetGraphic = img;

            if (onClick != null)
                btn.onClick.AddListener(() =>
                {
                    AudioManager.Instance?.PlaySfx(SfxId.UIClick);
                    onClick();
                });
            return btn;
        }

        /// <summary>水平进度条：返回填充 Image（修改 fillAmount 或 localScale.x）。</summary>
        public static Image Bar(Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos,
            Vector2 size, Color bgColor, Color fillColor, out RectTransform container)
        {
            var whiteSprite = GlucoseWar.Core.ProceduralSprites.Get(GlucoseWar.Core.ProceduralSprites.Shape.Square, Color.white, 8);
            var bg = new GameObject("BarBG");
            bg.transform.SetParent(parent, false);
            var bgImg = bg.AddComponent<Image>();
            bgImg.sprite = whiteSprite;
            bgImg.color = bgColor;
            container = Rect(bg);
            container.anchorMin = anchorMin; container.anchorMax = anchorMax;
            container.pivot = new Vector2(0f, 0.5f);
            container.anchoredPosition = anchoredPos; container.sizeDelta = size;

            var fill = new GameObject("Fill");
            fill.transform.SetParent(bg.transform, false);
            var fImg = fill.AddComponent<Image>();
            fImg.sprite = whiteSprite;
            fImg.color = fillColor;
            fImg.type = Image.Type.Filled;
            fImg.fillMethod = Image.FillMethod.Horizontal;
            fImg.fillOrigin = 0;
            fImg.fillAmount = 1f;
            var frt = Rect(fill);
            frt.anchorMin = Vector2.zero; frt.anchorMax = Vector2.one;
            frt.offsetMin = new Vector2(2, 2); frt.offsetMax = new Vector2(-2, -2);
            return fImg;
        }
    }
}
