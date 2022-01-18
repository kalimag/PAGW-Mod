using UnityEngine;

namespace Kalimag.Modding.Unity.Visualization.Colliders
{

	internal abstract class ColliderVisualizer<TCollider> : ColliderVisualizer
        where TCollider : Collider
    {

        protected GameObject VisualObject { get; private set; }
        protected MeshRenderer VisualRenderer { get; private set; }
        public new TCollider Collider { get; private set; }

        public ColliderVisualizer(TCollider collider)
            : base(collider)
        {
            Collider = collider;
        }

        protected abstract GameObject CreateVisualObject();


        protected override void Start()
        {
            VisualObject = CreateVisualObject();
            VisualRenderer = VisualObject.GetComponent<MeshRenderer>();
            if (Config != null)
                VisualRenderer.sharedMaterial = Config.Material;
        }

        protected override void OnConfigChanged()
        {
            if (VisualRenderer)
                VisualRenderer.sharedMaterial = Config?.Material;
        }

        protected override void OnEnable()
        {
            if (VisualObject)
                VisualObject.SetActive(true);
        }

        protected override void OnDisable()
        {
            if (VisualObject)
                VisualObject.SetActive(false);
        }

        public override void Destroy()
        {
            if (VisualObject)
                UnityEngine.Object.Destroy(VisualObject);

            // This seems to have a significant effect on garbage collection
            Collider = null;
            VisualObject = null;
            VisualRenderer = null;
        }

    }
}
