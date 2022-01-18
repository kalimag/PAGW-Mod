using UnityEngine;

namespace PAGW.Mod.Components
{
	internal class CursorUserComponent : MonoBehaviour
	{

		private void Start()
		{
			ModController.IncrementCursorUsers();
		}

		private void OnDestroy()
		{
			ModController.DecrementCursorUsers();
		}

		public static void AddTo(GameObject obj)
		{
			if (obj)
				obj.AddComponent<CursorUserComponent>();
		}

		public static void AddTo(Component component)
		{
			if (component)
				AddTo(component.gameObject);
		}

		public static void RemoveFrom(GameObject obj)
		{
			if (obj)
			{
				var component = obj.GetComponent<CursorUserComponent>();
				if (component)
					Destroy(component);
			}
		}

		public static void RemoveFrom(Component component)
		{
			if (component)
				RemoveFrom(component.gameObject);
		}

	}
}
