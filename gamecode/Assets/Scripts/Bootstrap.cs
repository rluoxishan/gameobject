using GlucoseWar.Core;
using GlucoseWar.Items;
using GlucoseWar.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GlucoseWar
{
    /// <summary>
    /// 全局入口：在 Boot 场景中构建摄像机/输入系统/全部 Manager 单例，
    /// 然后进入主菜单。整个游戏由一个场景 + 程序化构建驱动（省事方案）。
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        private static bool started;

        private void Awake()
        {
            if (started) { Destroy(gameObject); return; }
            started = true;
            DontDestroyOnLoad(gameObject);

            GameDatabase.EnsureBuilt();
            SetupCamera();
            SetupEventSystem();
            SetupManagers();
        }

        private void Start()
        {
            float saved = SaveSystem.Instance != null ? SaveSystem.Instance.GetFloat("master", 0.6f) : 0.6f;
            AudioManager.Instance?.SetVolumes(saved, 0.5f, 0.3f);
            GameManager.Instance.GoToMenu();
        }

        private void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<Camera>();
                camGo.AddComponent<AudioListener>();
            }
            cam.orthographic = true;
            cam.orthographicSize = 8f;
            cam.transform.position = new Vector3(0, 0, -10);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.01f, 0.03f);
            if (cam.GetComponent<CameraShake>() == null) cam.gameObject.AddComponent<CameraShake>();
            DontDestroyOnLoad(cam.gameObject);
        }

        private void SetupEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;
            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(go);
        }

        private void SetupManagers()
        {
            var managers = new GameObject("Managers");
            DontDestroyOnLoad(managers);
            managers.AddComponent<ObjectPool>();
            managers.AddComponent<PrefabFactory>();
            managers.AddComponent<SaveSystem>();
            managers.AddComponent<AudioManager>();
            managers.AddComponent<ScoreManager>();
            managers.AddComponent<KnowledgeManager>();
            managers.AddComponent<UIManager>();
            managers.AddComponent<GameManager>();

            // 预热子弹/敌人/特效对象池
            var pf = PrefabFactory.Instance;
            if (pf != null && ObjectPool.Instance != null)
            {
                ObjectPool.Instance.Prewarm(pf.BulletTemplate, 200);
                ObjectPool.Instance.Prewarm(pf.EnemyTemplate, 40);
                ObjectPool.Instance.Prewarm(pf.VfxTemplate, 20);
                ObjectPool.Instance.Prewarm(pf.ItemTemplate, 16);
            }
        }
    }
}
