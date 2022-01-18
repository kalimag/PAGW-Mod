using UnityEngine;

namespace PAGW.Mod.UI
{
	internal abstract class Window : MonoBehaviour
	{

		protected enum WindowId
		{
			VisualizationWindow = 0xEEEE00,
			AddColliderVisualizationWindow,
			OptionsWindow,
			LevelsWindow,
		}



		private GUI.WindowFunction _windowFunction;


		protected abstract WindowId Id { get; }
		protected abstract Rect WindowPosition { get; set; }
		protected abstract Rect InitialPosition { get; }
		protected abstract string Title { get; }
		protected virtual GUILayoutOption[] WindowLayout { get; set; } = new[] { GUILayout.ExpandHeight(true) };
		protected virtual bool StayOpen => false;

		protected virtual void Awake()
		{
			_windowFunction = DrawWindow;
			if (WindowPosition.x > Screen.width - 50 || WindowPosition.y > Screen.height - 50 || WindowPosition.x < -50 || WindowPosition.y < -5)
				WindowPosition = Rect.zero;

			if (StayOpen)
				DontDestroyOnLoad(this);
		}

		protected void OnEnable()
		{
			ModController.IncrementCursorUsers();
		}

		protected void OnDisable()
		{
			ModController.DecrementCursorUsers();
		}

		protected virtual void OnDestroy()
		{ }

		protected void OnGUI()
		{
			//if (UIController.HideAllUI)
			//    return;

			if (WindowPosition == Rect.zero)
			{
				WindowPosition = InitialPosition;
			}

			WindowPosition = GUILayout.Window((int)Id, WindowPosition, _windowFunction, Title, WindowLayout);
		}

		private void DrawWindow(int id)
		{
			DrawWindow();

			GUI.DragWindow();
		}

		protected abstract void DrawWindow();

		public void Close()
		{
			if (this && this.gameObject)
				Destroy(gameObject);
		}

	}
}
