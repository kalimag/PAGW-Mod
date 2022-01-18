using System;
using Kalimag.Modding.Unity.Visualization;
using Kalimag.Modding.Unity.Visualization.Colliders;
using UnityEngine;

namespace PAGW.Mod.UI
{
	internal class VisualizationWindow : Window
	{
		private static readonly Color[] Colors =
		{
			Color.green,
			Color.red,
			Color.magenta,
			Color.blue,
			Color.yellow,
			new Color(1, .5f, 0),
			Color.cyan,
			Color.gray,
			Color.white,
			Color.black
		};

		private AddColliderVisualizationWindow _addWindow;

		protected override WindowId Id => WindowId.VisualizationWindow;
		private static Rect _windowPosition;
		private static Vector2 _scrollPosition;

		protected override Rect WindowPosition { get => _windowPosition; set => _windowPosition = value; }
		protected override Rect InitialPosition => new Rect(50, 200, 450, 500);
		protected override string Title => "Visualizations";
		protected override bool StayOpen => true;



		protected override void Awake()
		{
			base.Awake();
			VisualizationController.UpdateVisualizations();
		}

		protected override void OnDestroy()
		{
			if (_addWindow)
				_addWindow.Close();
			base.OnDestroy();
		}

		protected override void DrawWindow()
		{
			GUILayout.BeginVertical(Styles.ExpandHeight, GUILayout.MaxHeight(800));

			VisualizationController.VisualizationsEnabled = GUILayout.Toggle(VisualizationController.VisualizationsEnabled, "Enable Visualizations (F4)");

			GUILayout.Space(10);

			ColliderVisualizationConfig deleteConfig = null;

			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

			foreach (var visualizationConfig in VisualizationController.CollisionVisualizationConfigs)
				DrawVisualizationItem(visualizationConfig, ref deleteConfig, visualizationConfig.BehaviourSelectors == null);

			DrawVisualizationItem(VisualizationController.AnyCollisionVisualizationConfig, ref deleteConfig, false);

			GUILayout.EndScrollView();

			if (deleteConfig != null)
				VisualizationController.CollisionVisualizationConfigs.Remove(deleteConfig);

			GUILayout.Space(10);

			if (GUILayout.Button("Add other trigger") && !_addWindow)
			{
				_addWindow = new GameObject().AddComponent<AddColliderVisualizationWindow>();
			}

			GUILayout.Space(10);
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Close"))
				Close();

			GUILayout.EndVertical();
		}

		private void DrawVisualizationItem(ColliderVisualizationConfig config, ref ColliderVisualizationConfig deleteConfig, bool allowDelete)
		{
			GUILayout.BeginHorizontal();

			bool enabled = GUILayout.Toggle(config.Enabled, config.SelectorLabel);
			if (enabled != config.Enabled)
			{
				config.Enabled = enabled;
				VisualizationController.UpdateVisualizations();
			}

			var colorButtonStyle = Styles.VisualizationWindow.ColorButtonStyle;
			colorButtonStyle.normal.textColor = config.Color;
			colorButtonStyle.hover.textColor = config.Color;
			bool nextColor = GUILayout.Button(config.ColorLabel, colorButtonStyle, Styles.Width100);
			if (nextColor)
			{
				var colorIndex = Array.IndexOf(Colors, config.Color) + 1;
				if (colorIndex >= Colors.Length)
					colorIndex = 0;
				config.Color = Colors[colorIndex];
			}

			bool nextShader = GUILayout.Button(config.Shader.ToString(), Styles.Width100);
			if (nextShader)
			{
				switch (config.Shader)
				{
					case ShapeVisualizationShader.Grid:
						config.Shader = ShapeVisualizationShader.Transparent;
						break;
					case ShapeVisualizationShader.Transparent:
						config.Shader = ShapeVisualizationShader.Opaque;
						break;
					case ShapeVisualizationShader.Opaque:
						config.Shader = ShapeVisualizationShader.Overlay;
						break;
					default:
						config.Shader = ShapeVisualizationShader.Grid;
						break;
				}
			}

			if (allowDelete)
			{
				if (GUILayout.Button("X", Styles.DontExpandWidth))
					deleteConfig = config;
			}
			else
			{
				GUILayout.Space(26);
			}

			GUILayout.EndHorizontal();
		}

	}

	partial class Styles
	{
		public static class VisualizationWindow
		{
			public static readonly GUIStyle ColorButtonStyle = new GUIStyle(GUI.skin.button);
		}
	}
}
