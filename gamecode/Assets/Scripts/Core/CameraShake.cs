using UnityEngine;

namespace GlucoseWar.Core
{
    /// <summary>挂在主摄像机上的轻量震屏（大招/Boss 转阶段）。</summary>
    public class CameraShake : MonoBehaviour
    {
        private static CameraShake instance;
        private Vector3 basePos;
        private float amplitude, timer, duration;

        private void Awake()
        {
            instance = this;
            basePos = transform.localPosition;
        }

        public static void Shake(float amplitude, float duration)
        {
            if (instance == null) return;
            instance.amplitude = amplitude;
            instance.duration = duration;
            instance.timer = duration;
        }

        private void LateUpdate()
        {
            if (timer > 0f)
            {
                timer -= Time.unscaledDeltaTime;
                float a = amplitude * (timer / duration);
                transform.localPosition = basePos + (Vector3)(Random.insideUnitCircle * a);
            }
            else
            {
                transform.localPosition = basePos;
            }
        }
    }
}
