
using System;
using System.Collections.Generic;
using System.Linq;
using Kalimag.Modding.Unity.Visualization;
using Kalimag.Modding.Unity.Visualization.Colliders;
using UnityEngine;

namespace PAGW.Mod.UI
{
	internal class AddColliderVisualizationWindow : Window
    {
        private static readonly Dictionary<Type, bool> _triggerBehaviours = new Dictionary<Type, bool>();

        private static readonly GUIContent[] ColliderTypeItems =
        {
            new GUIContent("Both"),
            new GUIContent("Trigger"),
            new GUIContent("Collision"),
        };
        private static readonly ColliderTypes[] ColliderTypeValues =
        {
            ColliderTypes.Both,
            ColliderTypes.Trigger,
            ColliderTypes.Collision,
        };


        protected override WindowId Id => WindowId.AddColliderVisualizationWindow;
        private static Rect _windowPosition;
        protected override Rect WindowPosition { get => _windowPosition; set => _windowPosition = value; }
        protected override Rect InitialPosition => new Rect(510, 200, 300, 500);
        protected override string Title => "Add Trigger";



        private List<SelectorItem> _selectors;
        private Vector2 _scollPosition;
        private int _selectedColliderType;



        protected override void Awake()
        {
            base.Awake();

            _selectors = BuildSelectorList();
        }


        protected override void OnDestroy()
        {
            _selectors = null;
        }

        protected override void DrawWindow()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Collider Type:");
            _selectedColliderType = GUILayout.Toolbar(_selectedColliderType, ColliderTypeItems);

            GUILayout.Space(10);

            _scollPosition = GUILayout.BeginScrollView(_scollPosition, Styles.ExpandHeight);

            foreach (var selector in _selectors)
                DrawSelectorItem(selector);

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close"))
                Close();

            GUILayout.EndVertical();
        }

        private void DrawSelectorItem(SelectorItem selector)
        {
            if (GUILayout.Button(selector.Label, Styles.AddColliderVisualization.SelectorButtonLayout))
            {
                ColliderVisualizationConfig config;
                ColliderVisualizationConfig existing;

                var colliderTypes = ColliderTypeValues[_selectedColliderType];

                if (selector.Selector is string tag)
                    config = ColliderVisualizationConfig.ForTag(tag, colliderTypes);
                else if (selector.Selector is Type behaviour)
                    config = ColliderVisualizationConfig.ForBehaviour(behaviour, colliderTypes);
                else if (selector.Selector is int layer)
                    config = ColliderVisualizationConfig.ForLayer(layer, colliderTypes);
                else
                    return;

                existing = VisualizationController.CollisionVisualizationConfigs.FirstOrDefault(cfg => cfg.Equals(config));

                if (existing == null)
                {
                    config.Enabled = true;
                    VisualizationController.CollisionVisualizationConfigs.Add(config);
                }
                else
                {
                    existing.Enabled = true;
                }
                VisualizationController.UpdateVisualizations();
            }
        }



        private List<SelectorItem> BuildSelectorList()
        {
            var behaviourCounts = new Dictionary<Type, int>();
            var tagCounts = new Dictionary<string, int>();
            var layerCounts = new Dictionary<int, int>();

            var behaviours = new List<MonoBehaviour>(16);

            foreach (Collider collider in Resources.FindObjectsOfTypeAll<Collider>())
            {
                var gameObject = collider.gameObject;
                if (!gameObject || !gameObject.scene.isLoaded)
                    continue;

                //if (collider.isTrigger)
                {
                    var tag = collider.tag;
                    if (tag != "Untagged" && !String.IsNullOrEmpty(tag))
                        Increment(tagCounts, tag);

                    gameObject.GetComponents(behaviours);
                    foreach (var behaviour in behaviours)
                        if (behaviour.GetType().Assembly != typeof(ColliderVisualizerComponent).Assembly)
                            Increment(behaviourCounts, behaviour.GetType());
                }

                Increment(layerCounts, gameObject.layer);
            }

            var selectorList = new List<SelectorItem>(behaviourCounts.Count + tagCounts.Count);
            foreach (var count in tagCounts)
                selectorList.Add(new SelectorItem(count.Key, count.Value));
            foreach (var count in behaviourCounts)
                selectorList.Add(new SelectorItem(count.Key, count.Value));
            selectorList.Sort((a, b) => a.Label.CompareTo(b.Label));
            foreach (var count in layerCounts)
                selectorList.Add(new SelectorItem(count.Key, count.Value));

            return selectorList;



            void Increment<T>(Dictionary<T, int> dict, T key)
            {
                if (!dict.TryGetValue(key, out int count))
                    count = 0;
                count++;
                dict[key] = count;
            }

            bool IsTriggerBehaviour(Type type)
            {
                if (!_triggerBehaviours.TryGetValue(type, out var isTrigger))
                {
                    isTrigger = HasMethod("OnTriggerEnter") || HasMethod("OnTriggerExit") || HasMethod("OnTriggerStay");
                    //Debug.Log($"IsTriggerBehaviour({type.FullName}) = {isTrigger}");
                    _triggerBehaviours[type] = isTrigger;
                }
                return isTrigger;

                bool HasMethod(string name) => type.GetMethod(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic) != null;
            }
        }


        private struct SelectorItem
        {
            public string Label { get; }
            public object Selector { get; }
            public int Instances { get; }

            public SelectorItem(object selector, int instances)
            {
                Selector = selector;
                Instances = instances;

                if (selector is Type type)
                    Label = type.FullName;
                else if (selector is string tag)
                    Label = "#" + tag;
                else if (selector is int layer)
                    Label = "L" + layer + " " + LayerMask.LayerToName(layer);
                else
                    throw new ArgumentException($"Invalid selector type {selector?.GetType()}");

                Label += " (" + Instances + ")";
            }

        }
    }

    partial class Styles
    {
        public static class AddColliderVisualization
        {
            public static readonly GUILayoutOption[] SelectorButtonLayout = new [] { GUILayout.Width(255) };
        }
    }
}
