using System;
using UnityEngine;

namespace Kalimag.Modding.Unity.Visualization.Colliders
{
	internal class CapsuleColliderVisualizer : ColliderVisualizer<CapsuleCollider>
    {

        private Vector3 _prevCenter = Vector3.zero;
        private float _prevRadius = float.MinValue;
        private float _prevHeight = float.MinValue;
        private float _prevDirection = -1;

        public CapsuleColliderVisualizer(CapsuleCollider collider)     
            : base(collider)    
        { }
         
        protected override GameObject CreateVisualObject()
        {
            var obj = GraphicsHelper.CreatePrimitive(PrimitiveType.Capsule);
            obj.name = "CapsuleColliderVisualizer VisualObject";
            obj.transform.SetParent(Collider.transform, false);
            return obj;
        }

        public override void LateUpdate()
        {
            Vector3 colliderCenter = Collider.center;
            if (colliderCenter != _prevCenter)
            {
                _prevCenter = colliderCenter;
                VisualObject.transform.localPosition = colliderCenter;
            }

            float colliderRadius = Collider.radius;
            float colliderHeight = Collider.height;
            int colliderDirection = Collider.direction;
            if (colliderRadius != _prevRadius || colliderHeight != _prevHeight || colliderDirection != _prevDirection)
            {
                _prevRadius = colliderRadius;
                _prevHeight = colliderHeight;
                _prevDirection = colliderDirection;

                float radiusRatio = colliderRadius / GraphicsHelper.DefaultCapsuleColliderRadius;
                float heightRatio = colliderHeight / GraphicsHelper.DefaultCapsuleColliderHeight;
                var visualScale = new Vector3(radiusRatio, heightRatio, radiusRatio);

                Vector3 visualRotation;

                Vector3 colliderScale = Collider.transform.lossyScale;
                float heightScale;
                float radiusScale1;
                float radiusScale2;

                switch (colliderDirection)
                {
                    case 0: // X
                        heightScale = colliderScale.x;
                        radiusScale1 = colliderScale.y;
                        radiusScale2 = colliderScale.z;
                        visualRotation = new Vector3(0f, 0f, 90f);
                        break;
                    case 1: // Y
                        heightScale = colliderScale.y;
                        radiusScale1 = colliderScale.x;
                        radiusScale2 = colliderScale.z;
                        visualRotation = Vector3.zero;
                        break;
                    case 2: // Z
                        heightScale = colliderScale.z;
                        radiusScale1 = colliderScale.x;
                        radiusScale2 = colliderScale.y;
                        visualRotation = new Vector3(90f, 0f, 0f);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid CapsuleCollider.direction");
                }

                heightScale = Mathf.Abs(heightScale);
                radiusScale1 = Mathf.Abs(radiusScale1);
                radiusScale2 = Mathf.Abs(radiusScale2);
                float maxRadiusScale = Mathf.Max(radiusScale1, radiusScale2);

                float scaledRadius = colliderRadius * maxRadiusScale;
                float scaledHeight = colliderHeight * heightScale;
                float additionalHeightScale = Mathf.Max(1f, (scaledRadius * 2) / scaledHeight); // diameter overrides height if greater

                //adjust scaling (collision cylinder stays perfectly circular, ends stay perfect half spheres)
                visualScale.x *= maxRadiusScale / radiusScale1;
                visualScale.y *= additionalHeightScale;
                visualScale.z *= maxRadiusScale / radiusScale2;

                VisualObject.transform.localScale = visualScale;
                VisualObject.transform.localEulerAngles = visualRotation;
            }

        }
    }
}
