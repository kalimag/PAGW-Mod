extern alias GameScripts;
using HarmonyLib;
using UnityEngine;

namespace PAGW.Mod.Patches
{
	[HarmonyPatch]
	internal class FruitDebugPatches
	{

		[HarmonyPrefix, HarmonyPatch(typeof(GameScripts.selfKill), "OnTriggerEnter"), HarmonyWrapSafe]
		private static void selfKill_OnTriggerEnter(GameScripts.selfKill __instance, Collider collider, int ___n, GameScripts.UILabel ___MoneyLabel)
		{
			if (collider.tag == "Player")
			{
				Debug.Log($"[FruitDebugPatches selfKill.OnTriggerEnter {__instance.GetInstanceID()} {__instance.gameObject.GetInstanceID()}]" +
					$" n={___n} MoneyLabel={___MoneyLabel.text} " +
					$" collider={{{collider.GetType().Name} of {collider.gameObject.name} #{collider.gameObject.GetInstanceID()} trigger={collider.isTrigger}}}");
			}
		}

	}
}
