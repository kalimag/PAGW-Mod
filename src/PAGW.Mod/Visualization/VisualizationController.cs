extern alias GameScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using Kalimag.Modding.Unity.Visualization.Colliders;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kalimag.Modding.Unity.Visualization
{
	internal static class VisualizationController
	{
		private static readonly int PlayerLayer = LayerMask.NameToLayer("Default");

		private static bool _initialized;

		private static List<MonoBehaviour> _sharedBehaviourList = new List<MonoBehaviour>(16);
		private static List<Collider> _sharedColliderList = new List<Collider>(4);

		private static bool _visualizationsEnabled;
		public static bool VisualizationsEnabled
		{
			get => _visualizationsEnabled;
			set
			{
				if (value != _visualizationsEnabled)
				{
					_visualizationsEnabled = value;
					UpdateVisualizations();
				}
			}
		}

		public static List<ColliderVisualizationConfig> CollisionVisualizationConfigs { get; } = new List<ColliderVisualizationConfig>();
		public static ColliderVisualizationConfig AnyCollisionVisualizationConfig { get; private set; }

		private static readonly ConfigSet TriggerConfigSet = new ConfigSet();
		private static readonly ConfigSet CollisionConfigSet = new ConfigSet();

		public delegate void VisualizationUpdateHandler(bool complete);
		public static event VisualizationUpdateHandler VisualizationUpdate;


		private static void Initialize()
		{
			if (_initialized)
				return;
			_initialized = true;

			SceneManager.sceneLoaded += OnSceneLoaded;

			CollisionVisualizationConfigs.Add(ColliderVisualizationConfig.ForTag("FailBox", ColliderTypes.Both, enabled: true, Color.red));
			CollisionVisualizationConfigs.Add(ColliderVisualizationConfig.ForTag("HiddenZone", ColliderTypes.Both, enabled: true, Color.yellow));

			CollisionVisualizationConfigs.Add(ColliderVisualizationConfig.ForBehaviour(typeof(GameScripts.transmission), ColliderTypes.Both, enabled: true, Color.blue));

			CollisionVisualizationConfigs.Add(ColliderVisualizationConfig.ForTag("Player", ColliderTypes.Trigger, enabled: false, Color.magenta));
			CollisionVisualizationConfigs.Add(ColliderVisualizationConfig.ForTag("Player", ColliderTypes.Collision, enabled: false, Color.gray));

			var gameScripts = typeof(GameScripts.ep02Dialogue).Assembly.GetTypes();

			var fruitBehaviours = gameScripts.Where(t => t.Name.StartsWith("AddFruitScore") || t.Name.StartsWith("moveFruit") || t.Name.StartsWith("selfKill"))
				.Concat(new[] { typeof(GameScripts.lastApple) });
			CollisionVisualizationConfigs.Add(ColliderVisualizationConfig.ForBehaviours(
				fruitBehaviours,
				"Fruit", ColliderTypes.Trigger, enabled: false, Color.cyan
			));
			CollisionVisualizationConfigs.Add(ColliderVisualizationConfig.ForBehaviours(
				fruitBehaviours,
				"Fruit", ColliderTypes.Both, enabled: true, Color.cyan, shader: ShapeVisualizationShader.Transparent
			));

			CollisionVisualizationConfigs.Add(ColliderVisualizationConfig.ForBehaviours(
				gameScripts.Where(t => t.Name.EndsWith("Dialogue") && t.Name != "Dialogue"),
				"Dialogue", ColliderTypes.Both, enabled: true, Color.green
			));

			CollisionVisualizationConfigs.Add(ColliderVisualizationConfig.ForBehaviours(
				gameScripts.Where(t => t.Name.Contains("inBattle") || t.Name == "LastBattleIn"),
				"Battle", ColliderTypes.Trigger, enabled: true, new Color(1, .5f, 0), shader: ShapeVisualizationShader.Grid
			));

			CollisionVisualizationConfigs.Add(ColliderVisualizationConfig.ForBehaviours(
				gameScripts.Where(t => t.Name.StartsWith("Nav")),
				"Black Orbs", ColliderTypes.Both, enabled: true, Color.red
			));

			AnyCollisionVisualizationConfig = ColliderVisualizationConfig.ForOther("Any", ColliderTypes.Collision, enabled: false, color: Color.white, shader: ShapeVisualizationShader.Grid);
		}

		private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			UpdateVisualizations();
		}

		public static void UpdateVisualizations()
		{
			Initialize();

			VisualizationUpdate?.Invoke(complete: false);

			if (VisualizationsEnabled)
			{
				UpdateConfigSets();

				foreach (Collider collider in Resources.FindObjectsOfTypeAll<Collider>())
					UpdateVisualizations(collider);
			}

			_sharedBehaviourList.Clear();

			VisualizationUpdate?.Invoke(complete: true);
		}

		public static void UpdateVisualizations(GameObject obj)
		{
			if (!VisualizationsEnabled || !obj)
				return;

			var component = obj.GetComponent<ColliderVisualizerComponent>();
			if (component)
				component.OnVisualizationUpdate(false);

			obj.GetComponents(_sharedColliderList);
			foreach (var collider in _sharedColliderList)
				component = UpdateVisualizations(collider) ?? component;

			_sharedBehaviourList.Clear();
			_sharedColliderList.Clear();

			if (component)
				component.OnVisualizationUpdate(true);
		}

		private static ColliderVisualizerComponent UpdateVisualizations(Collider collider)
		{
			if (!collider)
				return null;
			var gameObject = collider.gameObject;
			if (!gameObject || !gameObject.scene.isLoaded)
				return null;

			var configs = collider.isTrigger ? TriggerConfigSet : CollisionConfigSet;

			if (configs.Tags.TryGetValue(gameObject.tag, out var tagVisualization))
			{
				return ColliderVisualizerComponent.SetVisualizer(gameObject, collider, tagVisualization);
			}

			if (configs.Behaviours.Count > 0)
			{
				gameObject.GetComponents(_sharedBehaviourList);
				foreach (var behaviour in _sharedBehaviourList)
				{
					var behaviourType = behaviour.GetType();
					if (configs.Behaviours.TryGetValue(behaviourType, out var behaviourVisualization))
					{
						return ColliderVisualizerComponent.SetVisualizer(gameObject, collider, behaviourVisualization);
					}
				}
			}

			if (configs.Layers.TryGetValue(gameObject.layer, out var layerVisualization))
			{
				return ColliderVisualizerComponent.SetVisualizer(gameObject, collider, layerVisualization);
			}

			if (AnyCollisionVisualizationConfig.Enabled && collider.IsCollision())
				return ColliderVisualizerComponent.SetVisualizer(gameObject, collider, AnyCollisionVisualizationConfig);

			return null;
		}

		private static void UpdateConfigSets()
		{
			TriggerConfigSet.Clear();
			CollisionConfigSet.Clear();

			foreach (var config in CollisionVisualizationConfigs)
			{
				if (!config.Enabled)
					continue;

				if (config.BehaviourSelector != null)
				{
					Add(TriggerConfigSet.Behaviours, ColliderTypes.Trigger, config.BehaviourSelector, config);
					Add(CollisionConfigSet.Behaviours, ColliderTypes.Collision, config.BehaviourSelector, config);
				}

				if (config.BehaviourSelectors != null)
				{
					foreach (var behaviour in config.BehaviourSelectors)
					{
						Add(TriggerConfigSet.Behaviours, ColliderTypes.Trigger, behaviour, config);
						Add(CollisionConfigSet.Behaviours, ColliderTypes.Collision, behaviour, config);
					}
				}
				else if (config.TagSelector != null)
				{
					Add(TriggerConfigSet.Tags, ColliderTypes.Trigger, config.TagSelector, config);
					Add(CollisionConfigSet.Tags, ColliderTypes.Collision, config.TagSelector, config);
				}
				else if (config.LayerSelector != null)
				{
					Add(TriggerConfigSet.Layers, ColliderTypes.Trigger, config.LayerSelector.Value, config);
					Add(CollisionConfigSet.Layers, ColliderTypes.Collision, config.LayerSelector.Value, config);
				}
			}



			void Add<TKey>(Dictionary<TKey, ColliderVisualizationConfig> dict, ColliderTypes type, TKey key, ColliderVisualizationConfig config)
			{
				if (config.ColliderTypes.Matches(type) && !dict.ContainsKey(key))
					dict.Add(key, config);
			}
		}

		public static bool IsCollision(this Collider collider)
		{
			return !collider.isTrigger && !Physics.GetIgnoreLayerCollision(collider.gameObject.layer, PlayerLayer);
		}

		public static ColliderTypes GetColliderType(this Collider collider)
			=> collider.isTrigger ? ColliderTypes.Trigger : ColliderTypes.Collision;


		public static bool Matches(this Collider collider, Visualization.ColliderTypes type)
			=> (type & collider.GetColliderType()) != 0;

		public static bool Matches(this Visualization.ColliderTypes type, Visualization.ColliderTypes other)
			=> (type & other) != 0;

		private class ConfigSet
		{
			public Dictionary<string, ColliderVisualizationConfig> Tags { get; } = new Dictionary<string, ColliderVisualizationConfig>();
			public Dictionary<Type, ColliderVisualizationConfig> Behaviours { get; } = new Dictionary<Type, ColliderVisualizationConfig>();
			public Dictionary<int, ColliderVisualizationConfig> Layers { get; } = new Dictionary<int, ColliderVisualizationConfig>();

			public void Clear()
			{
				Tags.Clear();
				Behaviours.Clear();
				Layers.Clear();
			}
		}
	}
}
