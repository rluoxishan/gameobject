#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GlucoseWar.EditorTools
{
    /// <summary>编辑器辅助：一键打开 Boot 场景；首次加载时若无场景自动打开。</summary>
    [InitializeOnLoad]
    public static class ProjectMenu
    {
        private const string BootScenePath = "Assets/Scenes/Boot.unity";

        static ProjectMenu()
        {
            EditorApplication.delayCall += TryAutoOpen;
        }

        private static void TryAutoOpen()
        {
            var active = EditorSceneManager.GetActiveScene();
            bool empty = string.IsNullOrEmpty(active.path) && active.rootCount == 0;
            if (empty && System.IO.File.Exists(BootScenePath))
                EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);
        }

        [MenuItem("糖战/打开 Boot 场景 _F5")]
        public static void OpenBoot()
        {
            EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);
        }
    }
}
#endif
