extern alias GameScripts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PAGW.Mod.UI
{
	internal class LevelsWindow : Window
	{


		protected override WindowId Id => WindowId.LevelsWindow;
		private static Rect _windowPosition;
		protected override Rect WindowPosition { get => _windowPosition; set => _windowPosition = value; }
		protected override Rect InitialPosition => new Rect(Screen.width - 220, 200, 200, Screen.height / 2 - 80);
		protected override string Title => "Levels";
		protected override bool StayOpen => true;



		protected override void Awake()
		{
			base.Awake();
		}


		protected override void DrawWindow()
		{
			for (var id = MinLevelId; id <= MaxLevelId; id++)
			{
				if (GUILayout.Button($"{id}. {LevelNames[id - 1]}"))
					LoadLevel(id);
			}

			GUILayout.Space(10);

			if (GUILayout.Button("Main Menu"))
				SceneManager.LoadScene("GameStart");

			if (GUILayout.Button("Close"))
				Close();
		}

		private void LoadLevel(int id)
		{
			GetDB().UpdateCurrentMissionID(id);
			if (Input.GetKey(KeyCode.LeftControl))
			{
				PlayerPrefs.SetString("ToScenceName", $"ep{id:00}");
				SceneManager.LoadScene("LoadScence");
			}
			else
			{
				SceneManager.LoadScene($"ep{id:00}");
			}

			Close();
		}

		private GameScripts.OperatingDB GetDB()
		{
			var db = FindObjectOfType<GameScripts.OperatingDB>();
			if (!db)
				db = gameObject.AddComponent<GameScripts.OperatingDB>();
			return db;
		}


		private const int MinLevelId = 1;
		private const int MaxLevelId = 20;
		private static readonly string[] LevelNames = new[]
		{
			"Hello world",
			"Making progress",
			"Well Learn",
			"So complicated",
			"Happy from high",
			"Left or Right",
			"Jump for joy",
			"Undivided attention",
			"Difficult to find",
			"Grasp the rhythm",
			"Haste makes waste",
			"Meet by chance",
			"Infinite loop",
			"Three and Out",
			"Well protection",
			"In a dilemma",
			"Very dizzy",
			"Good in pairs",
			"Narrow escape",
			"Last Mission",
		};
	}
}
