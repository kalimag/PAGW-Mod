using System.Collections.Generic;
using UnityEngine;

namespace PAGW.Mod.Components
{
	internal class HotkeyComponent : MonoBehaviour
	{

		private void Update()
		{
			if (!Input.anyKey)
				return;

			if (Input.GetKeyDown(KeyCode.F7))
				ModController.InitUnityExplorer();

			if (ModController.Config.LevelMenu)
			{
				if (Input.GetKeyDown(KeyCode.F2))
					UI.UIController.ToggleLevelsWindow();
				if (Input.GetKeyDown(KeyCode.R) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
					ModController.RestartLevel();
			}

			if (ModController.Config.Visualizations)
			{
				if (Input.GetKeyDown(KeyCode.F3))
					UI.UIController.ToggleVisualizationWindow();

				if (Input.GetKeyDown(KeyCode.F4))
					Kalimag.Modding.Unity.Visualization.VisualizationController.VisualizationsEnabled ^= true;
			}

			if (ModController.Config.Teleport)
			{
				for (int slot = 0; slot <= 9; slot++)
				{
					if (Input.GetKeyDown(KeyCode.Alpha0 + slot))
					{
						if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
							TeleportController.SetTeleport(slot);
						else
							TeleportController.TeleportTo(slot);
					}
				}
			}

#if DEBUG
			if (Input.GetKeyDown(KeyCode.F8))
				HideInaccurateMeshes();
#endif
		}

#if DEBUG
		private void HideInaccurateMeshes()
		{
			int playerLayer = LayerMask.NameToLayer("Default");
			//int uiLayer = LayerMask.NameToLayer("UI");
			var meshColliders = new List<MeshCollider>();
			foreach (var renderer in FindLoadedComponents<MeshRenderer>())
			{
				if (!Physics.GetIgnoreLayerCollision(renderer.gameObject.layer, playerLayer))
				{
					var meshFilter = renderer.GetComponent<MeshFilter>();
					if (meshFilter && meshFilter.sharedMesh)
					{
						bool hasMatchingCollider = false;
						renderer.GetComponents(meshColliders);
						foreach (var collider in meshColliders)
						{
							if (collider.enabled && collider.sharedMesh == meshFilter.sharedMesh)
							{
								hasMatchingCollider = true;
								break;
							}
						}
						if (hasMatchingCollider)
							continue;
					}
				}
				renderer.enabled = false;

			}
		}

		public static IEnumerable<T> FindLoadedComponents<T>() where T : Component
		{
			var components = Resources.FindObjectsOfTypeAll<T>();
			foreach (var component in components)
			{
				if (component)
				{
					var gameObject = component.gameObject;
					if (gameObject && gameObject.scene.isLoaded)
						yield return component;
				}
			}
		}
#endif
	}
}
