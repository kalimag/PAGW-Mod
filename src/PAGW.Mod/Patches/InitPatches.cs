extern alias GameScripts;
using HarmonyLib;

namespace PAGW.Mod.Patches
{
	[HarmonyPatch]
	internal class InitPatches
	{

		[HarmonyPrefix, HarmonyPatch(typeof(GameScripts.OperatingDB), MethodType.Constructor), HarmonyWrapSafe]
		private static void OperatingDB_ctor()
		{
			ModController.TryGameInit();
		}

	}
}
