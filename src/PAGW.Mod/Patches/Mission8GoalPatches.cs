extern alias GameScripts;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace PAGW.Mod.Patches
{
	[HarmonyPatch]
	internal class Mission8GoalPatches
	{

		public const int DefaultScore = 100;

		[HarmonyTranspiler, HarmonyPatch(typeof(GameScripts.ep08Dialogue), "OnTriggerEnter")]
		private static IEnumerable<CodeInstruction> Transpile_ep08Dialogue_OnTriggerEnter(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				if (instruction.LoadsConstant(DefaultScore))
					yield return new CodeInstruction(OpCodes.Ldc_I4, (int)ModController.Config.Mission8Goal);
				else
					yield return instruction;
			}
		}

		[HarmonyPostfix, HarmonyPatch(typeof(GameScripts.ep08Dialogue), "OnTriggerEnter"), HarmonyWrapSafe]
		private static void Postfix_ep08Dialogue_OnTriggerEnter(bool ___dialogue01, bool ___dialogue02)
		{
			if (!___dialogue01 && ___dialogue02)
			{
				var dialogues = Traverse.Create<GameScripts.Dialogue>().Field<List<GameScripts.DialogueEntity>>("dialogues").Value;
				var entity = dialogues.FirstOrDefault(e => e.sceneName == "ep08" && e.ID == 105);
				if (entity != null)
				{
					entity.dialogueChinese = entity.dialogueChinese.Replace(DefaultScore.ToString(), ModController.Config.Mission8Goal.ToString());
					entity.dialogueEnghlish = entity.dialogueEnghlish.Replace(DefaultScore.ToString(), ModController.Config.Mission8Goal.ToString());
				}
			}
		}

	}
}
