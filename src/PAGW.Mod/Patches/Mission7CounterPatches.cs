extern alias GameScripts;
using HarmonyLib;

namespace PAGW.Mod.Patches
{
	[HarmonyPatch]
	internal class Mission7CounterPatches
	{

		[HarmonyPostfix, HarmonyPatch(typeof(GameScripts.ep07), "Start"), HarmonyWrapSafe]
		private static void ep07_Start()
		{
			Traverse.Create<GameScripts.selfKill>().Field<int>("n").Value = 0;
		}

	}
}
