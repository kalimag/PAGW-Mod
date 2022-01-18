using UnityEngine;

namespace PAGW.Mod.UI
{
	internal static class UIController
	{
        private static VisualizationWindow _visualizationWindow;
        public static void ToggleVisualizationWindow() => Toggle(ref _visualizationWindow);

        private static LevelsWindow levelsWindow;
        public static void ToggleLevelsWindow() => Toggle(ref levelsWindow);


        private static void Toggle<T>(ref T component) where T : Component
        {
            if (!component)
                component = new GameObject().AddComponent<T>();
            else if (component is Window window)
                window.Close();
            else
                UnityEngine.Object.Destroy(component.gameObject);
        }

        private static void CreateIfNotExists<T>(ref T component) where T : Component
        {
            if (!component)
                component = new GameObject().AddComponent<T>();
        }

    }
}
