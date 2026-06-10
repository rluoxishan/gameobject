using UnityEngine;

namespace GlucoseWar.Core
{
    /// <summary>池化特效：扩散并淡出的圆环（爆炸/拾取/升级/大招冲击波）。</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Vfx : MonoBehaviour, IPoolable
    {
        private SpriteRenderer sr;
        private float t, dur, startScale, endScale;
        private Color color;

        private void Awake() => sr = GetComponent<SpriteRenderer>();

        public void Play(Color c, float startSize, float endSize, float duration)
        {
            color = c; startScale = startSize; endScale = endSize; dur = Mathf.Max(0.05f, duration); t = 0f;
            sr.sprite = ProceduralSprites.Get(ProceduralSprites.Shape.Ring, Color.white, 64);
            sr.sortingOrder = 8;
        }

        public void OnSpawned() { }
        public void OnDespawned() { }

        private void Update()
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / dur);
            float s = Mathf.Lerp(startScale, endScale, n);
            transform.localScale = new Vector3(s, s, 1f);
            sr.color = new Color(color.r, color.g, color.b, 1f - n);
            if (n >= 1f)
            {
                if (ObjectPool.Instance != null) ObjectPool.Instance.Despawn(gameObject);
                else gameObject.SetActive(false);
            }
        }
    }
}
