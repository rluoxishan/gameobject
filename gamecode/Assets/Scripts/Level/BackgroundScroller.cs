using System.Collections.Generic;
using GlucoseWar.Core;
using GlucoseWar.Data;
using UnityEngine;

namespace GlucoseWar.Level
{
    /// <summary>多层视差背景：程序生成的色块向下漂移并循环，营造卷轴感。</summary>
    public class BackgroundScroller : MonoBehaviour
    {
        private struct Cell { public Transform t; public float speed; }
        private readonly List<Cell> cells = new List<Cell>();
        private float topY, bottomY, halfW;

        public void Setup(LevelTimeline level)
        {
            var cam = Camera.main;
            if (cam != null) cam.backgroundColor = level.farColor;

            float v = cam != null ? cam.orthographicSize : 8f;
            float h = v * (cam != null ? cam.aspect : 0.56f);
            topY = v + 2f; bottomY = -v - 2f; halfW = h;

            CreateLayer(18, level.midColor, 0.4f, 1.2f, -1, 2.5f);
            CreateLayer(14, level.nearColor, 0.8f, 2.0f, -2, 4.5f);
        }

        private void CreateLayer(int count, Color color, float minSize, float maxSize, int sorting, float baseSpeed)
        {
            var sprite = ProceduralSprites.Get(ProceduralSprites.Shape.Circle, color, 64);
            for (int i = 0; i < count; i++)
            {
                var go = new GameObject("BgCell");
                go.transform.SetParent(transform);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.color = new Color(color.r, color.g, color.b, 0.5f);
                sr.sortingOrder = sorting;
                float size = Random.Range(minSize, maxSize);
                go.transform.localScale = Vector3.one * size;
                go.transform.position = new Vector3(Random.Range(-halfW, halfW), Random.Range(bottomY, topY), 0);
                cells.Add(new Cell { t = go.transform, speed = baseSpeed * Random.Range(0.7f, 1.3f) });
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            for (int i = 0; i < cells.Count; i++)
            {
                var c = cells[i];
                Vector3 p = c.t.position;
                p.y -= c.speed * dt;
                if (p.y < bottomY)
                {
                    p.y = topY;
                    p.x = Random.Range(-halfW, halfW);
                }
                c.t.position = p;
            }
        }
    }
}
