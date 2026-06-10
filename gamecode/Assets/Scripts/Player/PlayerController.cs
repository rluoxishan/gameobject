using UnityEngine;

namespace GlucoseWar.Player
{
    /// <summary>读取输入→移动（边界限制 + 平滑）。运动能量道具临时加速。</summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float baseSpeed = 8f;
        private float speedMultiplier = 1f;
        private float boostTimer;
        private Camera cam;
        private float halfW = 0.4f, halfH = 0.4f;

        private void Start()
        {
            cam = Camera.main;
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                halfW = sr.bounds.extents.x;
                halfH = sr.bounds.extents.y;
            }
        }

        private void Update()
        {
            if (boostTimer > 0f)
            {
                boostTimer -= Time.deltaTime;
                if (boostTimer <= 0f) speedMultiplier = 1f;
            }

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector2 input = new Vector2(h, v);
            if (input.sqrMagnitude > 1f) input.Normalize();

            Vector3 delta = (Vector3)(input * baseSpeed * speedMultiplier * Time.deltaTime);
            Vector3 pos = transform.position + delta;
            ClampToScreen(ref pos);
            transform.position = pos;
        }

        private void ClampToScreen(ref Vector3 pos)
        {
            if (cam == null) cam = Camera.main;
            if (cam == null) return;
            float vert = cam.orthographicSize;
            float horz = vert * cam.aspect;
            pos.x = Mathf.Clamp(pos.x, -horz + halfW, horz - halfW);
            pos.y = Mathf.Clamp(pos.y, -vert + halfH, vert - halfH);
        }

        public void ApplySpeedBoost(float multiplier, float duration)
        {
            speedMultiplier = multiplier;
            boostTimer = duration;
        }
    }
}
