extern alias GameScripts;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PAGW.Mod.Patches
{
	[HarmonyPatch()]
	internal class NoDelayLoadingScreenPatches
	{

		[HarmonyPrefix, HarmonyPatch(typeof(GameScripts.ChangeToScence), "WaitAndPrint")]
		private static bool ChangeToScence_WaitAndPrint(ref IEnumerator __result)
		{
			__result = LoadSceneCoroutine();
			return false;
		}

		private static IEnumerator LoadSceneCoroutine()
		{
			SceneManager.LoadSceneAsync(PlayerPrefs.GetString("ToScenceName"));
			yield break;
		}

	}
}
