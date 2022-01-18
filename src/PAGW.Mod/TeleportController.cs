extern alias GameScripts;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PAGW.Mod
{
	public static class TeleportController
	{
		private const int TeleportSlots = 10;

		private static readonly Dictionary<string, TeleportTarget[]> _storedTeleports = new Dictionary<string, TeleportTarget[]>();



		public static TeleportTarget TryGetTeleport(int slot)
		{
			if (_storedTeleports.TryGetValue(SceneManager.GetActiveScene().path, out var sceneTeleports) && sceneTeleports.Length > slot)
				return sceneTeleports[slot];
			else
				return null;
		}

		public static void SetTeleport(int slot, TeleportTarget target)
		{
			string key = SceneManager.GetActiveScene().path;
			if (!_storedTeleports.TryGetValue(key, out var sceneTeleports))
				_storedTeleports[key] = (sceneTeleports = new TeleportTarget[TeleportSlots]);
			sceneTeleports[slot] = target;
		}

		public static void SetTeleport(int slot)
		{
			var player = FindPlayer();
			if (player)
				SetTeleport(slot, new TeleportTarget(player.transform, FindCamera()));
		}

		public static void TeleportTo(int slot)
		{
			var teleport = TryGetTeleport(slot);
			if (teleport == null)
			{
				ModController.AddNotification("No teleport saved in slot " + slot);
				return;
			}

			var player = GameObject.FindGameObjectWithTag("Player");

			if (teleport != null && player)
				teleport.Apply(player.transform, FindCamera());
		}

		private static GameObject FindPlayer() => GameObject.FindGameObjectWithTag("Player");

		private static GameScripts.ThirdPersonOrbitCam FindCamera()
		{
			var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
			if (mainCam)
				return mainCam.GetComponent<GameScripts.ThirdPersonOrbitCam>();
			else
				return null;
		}



		public class TeleportTarget
		{
			public Vector3 Position { get; }
			public float Rotation { get; }
			public Vector2? Camera { get; }

			public TeleportTarget(Transform transform, GameScripts.ThirdPersonOrbitCam camera)
			{
				Position = transform.position;
				Rotation = transform.eulerAngles.y;
				if (camera)
				{
					Camera = new Vector2(
						AccessTools.FieldRefAccess<GameScripts.ThirdPersonOrbitCam, float>(camera, "angleH"),
						AccessTools.FieldRefAccess<GameScripts.ThirdPersonOrbitCam, float>(camera, "angleV")
					);
				}
			}

			public void Apply(Transform transform, GameScripts.ThirdPersonOrbitCam camera)
			{
				transform.SetPositionAndRotation(Position, Quaternion.Euler(0f, Rotation, 0f));
				var rigidbody = transform.GetComponent<Rigidbody>();
				if (rigidbody)
					rigidbody.velocity = Vector3.zero;
				if (Camera != null && camera != null)
				{
					AccessTools.FieldRefAccess<GameScripts.ThirdPersonOrbitCam, float>(camera, "angleH") = Camera.Value.x;
					AccessTools.FieldRefAccess<GameScripts.ThirdPersonOrbitCam, float>(camera, "angleV") = Camera.Value.y;
				}
			}
		}
	}
}
