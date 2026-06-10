using System.Collections.Generic;
using UnityEngine;

namespace GlucoseWar.Core
{
    /// <summary>
    /// Generates simple colored placeholder sprites at runtime so the game is
    /// fully playable without any external PNG art. Sprites are cached by key.
    /// </summary>
    public static class ProceduralSprites
    {
        public enum Shape { Circle, Triangle, Hexagon, Ring, Square, Diamond, Bullet }

        private const float PPU = 100f;
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static Sprite Get(Shape shape, Color color, int size = 64)
        {
            string key = $"{shape}_{ColorUtility.ToHtmlStringRGBA(color)}_{size}";
            if (Cache.TryGetValue(key, out Sprite cached) && cached != null)
                return cached;

            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            Color clear = new Color(0, 0, 0, 0);
            float r = size * 0.5f;
            Vector2 c = new Vector2(r, r);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                    bool inside = ShapeContains(shape, p, c, r);
                    tex.SetPixel(x, y, inside ? color : clear);
                }
            }

            tex.Apply();
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), PPU);
            sprite.name = key;
            Cache[key] = sprite;
            return sprite;
        }

        private static bool ShapeContains(Shape shape, Vector2 p, Vector2 c, float r)
        {
            Vector2 d = p - c;
            float dist = d.magnitude;
            switch (shape)
            {
                case Shape.Circle:
                    return dist <= r - 0.5f;
                case Shape.Ring:
                    return dist <= r - 0.5f && dist >= r * 0.62f;
                case Shape.Square:
                    return Mathf.Abs(d.x) <= r - 1f && Mathf.Abs(d.y) <= r - 1f;
                case Shape.Diamond:
                    return Mathf.Abs(d.x) + Mathf.Abs(d.y) <= r - 1f;
                case Shape.Bullet:
                    // capsule-ish: tall rounded
                    return Mathf.Abs(d.x) <= r * 0.45f && dist <= r - 0.5f;
                case Shape.Triangle:
                    return InTriangle(p, c, r, true);
                case Shape.Hexagon:
                    return InHexagon(d, r - 1f);
                default:
                    return dist <= r;
            }
        }

        private static bool InTriangle(Vector2 p, Vector2 c, float r, bool pointingUp)
        {
            float top = pointingUp ? c.y + r - 1f : c.y - r + 1f;
            float baseY = pointingUp ? c.y - r + 1f : c.y + r - 1f;
            float t = pointingUp ? (top - p.y) / (top - baseY) : (p.y - top) / (baseY - top);
            if (t < 0f || t > 1f) return false;
            float halfWidth = (r - 1f) * t;
            return Mathf.Abs(p.x - c.x) <= halfWidth;
        }

        private static bool InHexagon(Vector2 d, float r)
        {
            float x = Mathf.Abs(d.x);
            float y = Mathf.Abs(d.y);
            if (x > r || y > r * 0.866f) return false;
            return r * 0.866f * x + 0.5f * r * y <= r * r * 0.866f;
        }

        public static Sprite SolidBar(Color color)
        {
            return Get(Shape.Square, color, 8);
        }
    }
}
