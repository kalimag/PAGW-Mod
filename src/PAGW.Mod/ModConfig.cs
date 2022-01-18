using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using IO = System.IO;

namespace PAGW.Mod
{
	internal class ModConfig
	{


		private static readonly HashSet<string> ImportantSettings = new HashSet<string>
		{
			nameof(Flying),
			nameof(Teleport),
			nameof(LevelMenu),
			nameof(Visualizations),
			nameof(InstantRestart),
			nameof(NoDelayLoadingScreen),
			nameof(Mission7Fix),
			nameof(Mission8Goal),
		};

		public float NotificationDisplayTime { get; private set; } = 7.5f;
		public bool LockMouse { get; private set; }
		public bool InstantRestart { get; private set; }
		public bool NoDelayLoadingScreen { get; private set; }
		public bool LevelMenu { get; private set; }
		public bool Visualizations { get; private set; }
		public bool Flying { get; private set; }
		public bool Teleport { get; private set; }
		public bool Mission7Fix { get; private set; }
		public int Mission8Goal { get; private set; } = Patches.Mission8GoalPatches.DefaultScore;
		public bool FramerateIndependentPlatforms { get; private set; }

		public string ActiveSettings { get; }



		private const string ConfigName = "mod.config";
		private static readonly Regex ConfigArgRegex = new Regex(@"^(.*\s)?-?-modconfig\s+(?<modconfig>[\w\-_\.]+)(\s.*)?$", RegexOptions.IgnoreCase);
		private static readonly Regex KeyValueRegex = new Regex(@"^\s*(?<key>[^=#\[][^=]+?)\s*=\s*(?<value>.*?)\s*$");

		private ModConfig(string path)
		{
			Debug.Log($"Config path: \"{path}\"");
			var changedProps = new Dictionary<PropertyInfo, object>();

			using (var reader = new IO.StreamReader(path, Encoding.UTF8))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					var match = KeyValueRegex.Match(line);
					if (!match.Success)
						continue;

					string key = match.Groups["key"].Value;
					string value = match.Groups["value"].Value;

					var prop = typeof(ModConfig).GetProperty(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
					if (prop != null && prop.CanWrite)
					{
						try
						{
							var convertedValue = Convert.ChangeType(value, prop.PropertyType, CultureInfo.InvariantCulture);
							if (!Equals(prop.GetValue(this, index: null), convertedValue))
								changedProps[prop] = convertedValue;
							prop.SetValue(this, Convert.ChangeType(value, prop.PropertyType, CultureInfo.InvariantCulture), index: null);
						}
						catch (Exception ex)
						{
							Debug.Log($"[ModConfig] Invalid value \"{value}\" for \"{key}\": {ex.Message}");
						}
					}
					else
					{
						Debug.Log($"[ModConfig] Unknown config key \"{key}\"");
					}
				}
			}

			ActiveSettings = String.Join(" ", changedProps
				.Where(kvp => ImportantSettings.Contains(kvp.Key.Name))
				.OrderBy(kvp => kvp.Key.Name)
				.Select(kvp => true.Equals(kvp.Value) ? kvp.Key.Name : $"{kvp.Key.Name}={kvp.Value?.ToString()}")
				.ToArray()
			);
			var prefix = GetConfigPrefix();
			if (!String.IsNullOrEmpty(prefix))
				ActiveSettings = prefix + ": " + ActiveSettings;
		}

		public static ModConfig LoadConfig()
		{
			var prefix = GetConfigPrefix();
			var configName = String.IsNullOrEmpty(prefix) ? ConfigName : prefix + "." + ConfigName;
			var path = IO.Path.Combine(IO.Path.GetDirectoryName(Environment.GetEnvironmentVariable("DOORSTOP_PROCESS_PATH")), configName);
			return new ModConfig(path);
		}

		private static string GetConfigPrefix()
		{
			var argMatch = ConfigArgRegex.Match(Environment.CommandLine);
			return argMatch.Groups["modconfig"].Value;
		}

	}
}
