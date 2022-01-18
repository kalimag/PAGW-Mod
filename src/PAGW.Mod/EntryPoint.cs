using System;
using System.Threading;
using HarmonyLib;

namespace PAGW.Mod
{
	internal class EntryPoint
	{

		public static void Main()
		{
			var harmonyThread = new Thread(Init) { IsBackground = true };
			harmonyThread.Start();
		}

		private static void Init()
		{
			Thread.Sleep(1000);

			ModConfig config;
			try
			{
				config = ModConfig.LoadConfig();
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError($"[Mod] Could not read config: {ex.Message}");
				return;
			}
			ModController.Config = config;
			UnityEngine.Debug.Log($"[Mod] Mod loaded");

#if DEBUG
            HarmonyLib.Tools.Logger.ChannelFilter = HarmonyLib.Tools.Logger.LogChannel.All;
            HarmonyLib.Tools.HarmonyFileLog.Enabled = true;
#endif
			var harmony = new Harmony("net.kalimag.modding.pagw");

			ApplyPatches<Patches.InitPatches>();

			if (config.LockMouse)
				ApplyPatches<Patches.LockCursorPatches>();
			if (config.InstantRestart)
				ApplyPatches<Patches.InstantRestartPatches>();
			if (config.NoDelayLoadingScreen)
				ApplyPatches<Patches.NoDelayLoadingScreenPatches>();
			if (config.Flying)
				ApplyPatches<Patches.FlyingPatches>();
			if (config.Mission7Fix)
				ApplyPatches<Patches.Mission7CounterPatches>();
			if (config.Mission8Goal != Patches.Mission8GoalPatches.DefaultScore && config.Mission8Goal > 0)
				ApplyPatches<Patches.Mission8GoalPatches>();
			if (config.FramerateIndependentPlatforms)
				ApplyPatches<Patches.FramerateIndependentPlatformsPatches>();
			if (config.Visualizations)
				ApplyPatches<Patches.VisualizationUpdatePatches>();

#if DEBUG
			ApplyPatches<Patches.LogSpamPatches>();
			ApplyPatches<Patches.FruitDebugPatches>();
#endif

			void ApplyPatches<T>() => harmony.CreateClassProcessor(typeof(T)).Patch();
		}

	}
}
