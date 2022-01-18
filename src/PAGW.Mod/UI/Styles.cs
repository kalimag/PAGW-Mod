using UnityEngine;

namespace PAGW.Mod.UI
{
	partial class Styles
    {

        static Styles()
		{
            InitDefaultSkin();
        }

        public static void InitDefaultSkin()
        {
            const int FontSize = 15;
            GUI.skin.button.fontSize = FontSize;
            GUI.skin.label.fontSize = FontSize;
            GUI.skin.toggle.fontSize = FontSize;
            GUI.skin.window.fontSize = FontSize;
        }

        public static readonly GUILayoutOption ExpandHeight = GUILayout.ExpandHeight(true);
        public static readonly GUILayoutOption ExpandWidth = GUILayout.ExpandWidth(true);
        public static readonly GUILayoutOption[] ExpandBoth = new[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };

        public static readonly GUILayoutOption DontExpandHeight = GUILayout.ExpandHeight(false);
        public static readonly GUILayoutOption DontExpandWidth = GUILayout.ExpandWidth(false);
        public static readonly GUILayoutOption[] DontExpandBoth = new[] { GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false) };

        public static readonly GUILayoutOption Width100 = GUILayout.Width(100);

    }
}
