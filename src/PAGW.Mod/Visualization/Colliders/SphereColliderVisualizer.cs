using UnityEngine;

namespace Kalimag.Modding.Unity.Visualization.Colliders
{
	internal class SphereColliderVisualizer : ColliderVisualizer<SphereCollider>
    {

        private float _prevRadius = float.MinValue;
        private Vector3 _prevCenter = Vector3.zero;

        public SphereColliderVisualizer(SphereCollider collider)
            : base(collider)
        { }


        protected override GameObject CreateVisualObject()
        {
            var obj = GraphicsHelper.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = "SphereColliderVisualizer VisualObject";
            obj.transform.SetParent(Collider.transform, false);
            return obj;
        }

        public override void LateUpdate()
        {
            float colliderRadius = Collider.radius;
            if (colliderRadius != _prevRadius)
            {
                _prevRadius = colliderRadius;

                float radiusScale = colliderRadius / GraphicsHelper.DefaultSphereColliderRadius;
                var visualScale = new Vector3(radiusScale, radiusScale, radiusScale);

                // adjust for uneven scaling (collision sphere stays spherical and uses max scale for all directions)
                Vector3 colliderScale = Collider.transform.lossyScale;
                float maxColliderScale = Mathf.Max(colliderScale.x, Mathf.Max(colliderScale.y, colliderScale.z));
                visualScale.x *= maxColliderScale / colliderScale.x;
                visualScale.y *= maxColliderScale / colliderScale.y;
                visualScale.z *= maxColliderScale / colliderScale.z;

                VisualObject.transform.localScale = visualScale;
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
