using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kalimag.Modding.Unity.Visualization.Colliders
{

	internal class ColliderVisualizerComponent : MonoBehaviour
    {

        private Dictionary<Collider, ColliderVisualizer> _colliderVisualizers = new Dictionary<Collider, ColliderVisualizer>(1);


        public ColliderTypes CurrentColliderTypes { get; private set; }


        //private ColliderVisualizerComponent()
        //{
        //    VisualizationController.VisualizationUpdate += OnVisualizationUpdate;
        //}

        protected void Start()
        {
            VisualizationController.VisualizationUpdate += OnVisualizationUpdate;
        }

        internal void OnVisualizationUpdate(bool complete)
        {
            try
            {
                if (!this)
                {
                    Debug.Log("OnVisualizationUpdate called on dead ColliderVisualizerComponent");
                    OnDestroy();
                    return;
                }

                if (!complete)
                    OnBeforeVisualizationUpdate();
                else
                    OnAfterVisualizationUpdate();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        private void OnBeforeVisualizationUpdate()
        {
            foreach (var visualizer in _colliderVisualizers.Values)
                visualizer.Stale = true;
        }

        private void OnAfterVisualizationUpdate()
        {
            bool anyFresh = false;
            foreach (var visualizer in _colliderVisualizers.Values)
            {
                if (visualizer.Stale)
                    visualizer.Enabled = false;
                else
                    anyFresh = true;
            }

            enabled = anyFresh;

            //if (!anyFresh)
            //    DebugHelper.Log(this);
        }

        //protected void OnEnable()
        //{
        //    foreach (var kvp in _colliders)
        //        kvp.Value.Enabled = !kvp.Value.Stale && kvp.Key.enabled;
        //}

        protected void OnDisable()
        {
            foreach (var visualizer in _colliderVisualizers.Values)
                visualizer.Enabled = false;
        }

        protected void LateUpdate()
        {
            foreach (var kvp in _colliderVisualizers)
            {
                kvp.Value.Enabled = !kvp.Value.Stale && kvp.Key && kvp.Key.enabled;
                if (kvp.Value.Enabled)
                    kvp.Value.LateUpdate();
            }
        }

        protected void OnDestroy()
        {
            VisualizationController.VisualizationUpdate -= OnVisualizationUpdate;

            if (_colliderVisualizers != null)
                foreach (var visualizer in _colliderVisualizers.Values)
                    visualizer.Destroy();

            _colliderVisualizers = null;
        }



        public void SetVisualizer(Collider collider, ColliderVisualizationConfig config)
        {
            // TODO: do destroyed objects throw on equals/gethashcode?
            if (!_colliderVisualizers.TryGetValue(collider, out var visualizer))
            {
                visualizer = CreateVisualizer(collider);
                if (visualizer == null)
                    return;
                _colliderVisualizers[collider] = visualizer;
            }
            visualizer.Config = config;
            visualizer.Stale = false;
            enabled = true;
        }

        public static ColliderVisualizerComponent SetVisualizer(GameObject obj, Collider collider, ColliderVisualizationConfig config)
        {
            var component = GetOrAdd(obj);
            component.SetVisualizer(collider, config);
            return component;
        }

        public static ColliderVisualizerComponent GetOrAdd(GameObject obj)
        {
            var visualizer = obj.GetComponent<ColliderVisualizerComponent>();
            if (!visualizer)
                visualizer = obj.AddComponent<ColliderVisualizerComponent>();
            return visualizer;
        }




        private static ColliderVisualizer CreateVisualizer(Collider collider)
        {
            switch (collider)
            {
                case SphereCollider sphereCollider:
                    return new SphereColliderVisualizer(sphereCollider);

                case BoxCollider boxCollider:
                    return new BoxColliderVisualizer(boxCollider);

                case CapsuleCollider capsuleCollider:
                    return new CapsuleColliderVisualizer(capsuleCollider);

                case MeshCollider meshCollider:
                    return new MeshColliderVisualizer(meshCollider);

                default:
                    Debug.Log("ColliderVisualizer does not support " + collider.GetType().Name);
#if DEBUG
                     PAGW.Mod.ModController.AddNotification("ColliderVisualizer does not support " + collider.GetType().Name);
#endif
                    return null;
            }
        }



        //private static readonly List<Collider> _colliderResultList = new List<Collider>(capacity: 4);
        //public static void AddVisualizers(GameObject obj, ColliderVisualizationConfig config, bool destroyOnVisualizationUpdate = false, List<ColliderVisualizer> results = null)
        //{
        //    //Debug.Log($"ColliderVisualizer.AddVisualizers({DebugHelper.GetName(obj)}, visualizeTriggers={visualizeTriggers})");

        //    obj.GetComponents(_colliderResultList);
        //    foreach (var collider in _colliderResultList)
        //    {
        //        if (collider.Matches(config.ColliderTypes))
        //        {
        //            var visualizer = AddVisualizer(collider, config, destroyOnVisualizationUpdate);
        //            if (visualizer != null && results != null)
        //                results.Add(visualizer);
        //        }
        //    }
        //}

    }

}
