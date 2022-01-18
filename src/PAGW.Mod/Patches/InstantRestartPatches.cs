extern alias GameScripts;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace PAGW.Mod.Patches
{
	[HarmonyPatch()]
	internal class InstantRestartPatches
	{

		[HarmonyPrefix, HarmonyPatch(typeof(GameScripts.Failing), nameof(GameScripts.Failing.restart))]
		private static bool Failing_restart()
		{
			GameScripts.BasicBehaviour.playerCtrlBool = true;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			return false;
		}

	}
}
