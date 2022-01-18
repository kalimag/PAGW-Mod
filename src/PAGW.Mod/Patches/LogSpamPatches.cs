extern alias GameScripts;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace PAGW.Mod.Patches
{
	[HarmonyPatch]
	internal class LogSpamPatches
	{

		[HarmonyTranspiler, HarmonyPatch(typeof(GameScripts.DbAccess), nameof(GameScripts.DbAccess.OpenDB))]
		private static IEnumerable<CodeInstruction> Transpile_DbAccess_OpenDB(IEnumerable<CodeInstruction> instructions)
			=> OmitLogCall(instructions, "Connected to db,连接数据库成功！");

		[HarmonyTranspiler, HarmonyPatch(typeof(GameScripts.DbAccess), nameof(GameScripts.DbAccess.CloseSqlConnection))]
		private static IEnumerable<CodeInstruction> Transpile_DbAccess_CloseSqlConnection(IEnumerable<CodeInstruction> instructions)
			=> OmitLogCall(instructions, "Disconnected from db.关闭数据库！");

		private static IEnumerable<CodeInstruction> OmitLogCall(IEnumerable<CodeInstruction> instructions, string logStr)
		{
			int skip = 0;
			foreach (var instruction in instructions)
			{
				if (skip > 0)
				{
					skip--;
					continue;
				}
				if (instruction.opcode == OpCodes.Ldstr && instruction.OperandIs(logStr))
					skip = 1;
				else
					yield return instruction;
			}
		}

		[HarmonyPrefix, HarmonyPatch(typeof(GameScripts.skill), "Update")]
		private static bool skill_Update()
		{
			return false;
		}

	}
}
