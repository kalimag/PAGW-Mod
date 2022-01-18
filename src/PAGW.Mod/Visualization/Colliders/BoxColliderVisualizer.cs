using UnityEngine;

namespace Kalimag.Modding.Unity.Visualization.Colliders
{
	internal class BoxColliderVisualizer : ColliderVisualizer<BoxCollider>
    {

        private Vector3 _prevSize = Vector3.zero;
        private Vector3 _prevCenter = Vector3.zero;

        public BoxColliderVisualizer(BoxCollider collider)
            : base(collider)
        { }

        protected override GameObject CreateVisualObject()
        {
            var obj = GraphicsHelper.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "BoxColliderVisualizer VisualObject";
            obj.transform.SetParent(Collider.transform, false);
            return obj;
        }

        public override void LateUpdate()
        {
            Vector3 colliderSize = Collider.size;
            if (colliderSize != _prevSize)
            {
                _prevSize = colliderSize;

                VisualObject.transform.localScale = new Vector3(
                    Mathf.Abs(colliderSize.x) / GraphicsHelper.DefaultBoxColliderSize.x,
                    Mathf.Abs(colliderSize.y) / GraphicsHelper.DefaultBoxColliderSize.y,
                    Mathf.Abs(colliderSize.z) / GraphicsHelper.DefaultBoxColliderSize.z
                );
            }

            Vector3 colliderCenter = Collider.center;
            if (colliderCenter != _prevCenter)
            {
                _prevCenter = colliderCenter;
                VisualObject.transform.localPosition = colliderCenter;
            }
        }
    }
}
