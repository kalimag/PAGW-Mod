extern alias GameScripts;
using HarmonyLib;
using UnityEngine;

namespace PAGW.Mod.Patches
{
	[HarmonyPatch]
	internal class FlyingPatches
	{

		[HarmonyPostfix, HarmonyPatch(typeof(GameScripts.MoveBehaviour), "Start"), HarmonyWrapSafe]
		private static void MoveBehaviour_Start(MonoBehaviour __instance)
		{
			__instance.gameObject.AddComponent<GameScripts.FlyBehaviour>();
		}

	}
}
