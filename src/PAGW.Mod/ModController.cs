using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PAGW.Mod
{
	internal static class ModController
	{

		private static bool _initialized;
		private static bool _unityExplorerInitialized;

		private static int _cursorUsers;
		private static bool _forceCursorDisabled;



		public static ModConfig Config { get; set; }

		public static GameObject RootObject { get; private set; }
		private static Components.UIComponent UI { get; set; }
		private static Components.CoroutineComponent Coroutines { get; set; }



		public static void TryGameInit()
		{
			if (_initialized)
				return;
			_initialized = true;

			try
			{
				GameInit();
			}
			catch (Exception ex)
			{
				Debug.Log("[ModController] Could not initialize mod");
				Debug.LogException(ex);
			}
		}

		private static void GameInit()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;

			RootObject = new GameObject("Mod Root Object");
			RootObject.hideFlags |= HideFlags.HideAndDontSave;
			UnityEngine.Object.DontDestroyOnLoad(RootObject);

			Coroutines = RootObject.AddComponent<Components.CoroutineComponent>();
			UI = RootObject.AddComponent<Components.UIComponent>();
			RootObject.AddComponent<Components.HotkeyComponent>();

			UpdateCursor();
		}




		private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
#if DEBUG
			Debug.Log($"[Scene] Loaded {scene.name} (#{scene.buildIndex} {scene.path})");
#endif

			if (UI)
				UI.DisplayVersion = scene.name == "GameStart";
		}



		public static void IncrementCursorUsers()
		{
			_cursorUsers++;
			UpdateCursor();
		}

		public static void DecrementCursorUsers()
		{
			_cursorUsers--;
			if (_cursorUsers < 0)
			{
				_cursorUsers = 0;
				Debug.Log("[ModController.DecrementCursorUsers()] _cursorUsers unbalanced");
			}
			UpdateCursor();
		}

		public static void ForceCursorDisabled(bool forceDisabled)
		{
			if (forceDisabled != _forceCursorDisabled)
			{
				_forceCursorDisabled = forceDisabled;
				UpdateCursor();
			}
		}

		private static void UpdateCursor()
		{
			if (Config.LockMouse)
			{
				Cursor.visible = !_forceCursorDisabled && _cursorUsers > 0;
				Cursor.lockState = CursorLockMode.Confined;
			}
		}



		internal static void RestartLevel()
		{
			var scene = SceneManager.GetActiveScene().name;
			if (scene.StartsWith("ep"))
				SceneManager.LoadScene(scene);
		}

		public static void InitUnityExplorer()
		{
			Cursor.visible = true;
			if (_unityExplorerInitialized)
				return;
			_unityExplorerInitialized = true;

			try
			{
				Assembly.Load("UnityExplorer.STANDALONE.Mono")
					?.GetType("UnityExplorer.ExplorerStandalone")
					?.GetMethod("CreateInstance", new Type[] { })
					?.Invoke(null, null);
			}
			catch (Exception ex)
			{
				Debug.LogError($"[ModController] Failed to load UnityExplorer: {ex}");
			}
		}





		public static void AddNotification(string message)
		{
#if DEBUG
			Debug.Log($"[ModController] AddNotification({message})");
#endif
			if (UI)
				UI.AddNotification(message);
		}


		public static void StartCoroutine(System.Collections.IEnumerator routine)
		{
			Coroutines.StartCoroutine(routine);
		}
	}
}
