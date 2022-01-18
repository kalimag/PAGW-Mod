extern alias GameScripts;
using HarmonyLib;
using UnityEngine;

namespace PAGW.Mod.Patches
{
	[HarmonyPatch]
	internal class LockCursorPatches
	{

		[HarmonyPostfix, HarmonyPatch(typeof(GameScripts.UIButtonEvents), "Start")]
		private static void UIButtonEvents_Start(MonoBehaviour __instance) => Components.CursorUserComponent.AddTo(__instance);

		[HarmonyPostfix, HarmonyPatch(typeof(GameScripts.QuitToMenuEvent), nameof(GameScripts.QuitToMenuEvent.ShowQuitMenu))]
		private static void QuitToMenuEvent_ShowQuitMenu(MonoBehaviour __instance) => Components.CursorUserComponent.AddTo(__instance);

		[HarmonyPostfix, HarmonyPatch(typeof(GameScripts.QuitToMenuEvent), nameof(GameScripts.QuitToMenuEvent.HideQuitMenu))]
		private static void QuitToMenuEvent_HideQuitMenu(MonoBehaviour __instance) => Components.CursorUserComponent.RemoveFrom(__instance);

		[HarmonyPrefix, HarmonyPatch(typeof(GameScripts.Winning), nameof(GameScripts.Winning.ShowWinMenu))]
		private static void Winning_ShowWinMenu(GameScripts.Winning __instance)
			=> Components.CursorUserComponent.AddTo(__instance.gameObject);

		[HarmonyPrefix, HarmonyPatch(typeof(GameScripts.Failing), nameof(GameScripts.Failing.ShowFailMenu))]
		private static void Failing_ShowFailMenu(GameScripts.Failing __instance)
			=> Components.CursorUserComponent.AddTo(__instance.gameObject);

	}
}
