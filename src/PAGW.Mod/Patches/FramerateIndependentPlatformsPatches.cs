extern alias GameScripts;
using HarmonyLib;
using UnityEngine;

namespace PAGW.Mod.Patches
{
	[HarmonyPatch]
	internal class FramerateIndependentPlatformsPatches
	{

		private const float BaseRotation = .5f;
		private const float BaseFramerate = 60;

		[HarmonyPrefix, HarmonyPatch(typeof(GameScripts.RotaSelf), "Update")]
		private static bool RotaSelf_Update(GameScripts.RotaSelf __instance)
		{
			__instance.transform.Rotate(Vector3.down * __instance.direction * BaseRotation * BaseFramerate * Time.deltaTime, Space.World);
			return false;
		}

	}
}
