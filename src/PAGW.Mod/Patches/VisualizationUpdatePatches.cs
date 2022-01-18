extern alias GameScripts;
using System.Collections;
using HarmonyLib;
using Kalimag.Modding.Unity.Visualization;
using UnityEngine;

namespace PAGW.Mod.Patches
{
	[HarmonyPatch]
	internal class VisualizationUpdatePatches
	{

		[HarmonyPostfix, HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Instantiate), typeof(UnityEngine.Object), typeof(Vector3), typeof(Quaternion)), HarmonyWrapSafe]
		private static void Object_Instantiate(ref UnityEngine.Object __result)
		{
#if DEBUG
			Debug.Log($"Instantiated {__result}");
#endif
			if (VisualizationController.VisualizationsEnabled && __result is GameObject obj)
				ModController.StartCoroutine(UpdateNextFrame(obj));
		}

		private static IEnumerator UpdateNextFrame(GameObject obj)
		{
			yield return null;
			VisualizationController.UpdateVisualizations(obj);
		}
	}
}
