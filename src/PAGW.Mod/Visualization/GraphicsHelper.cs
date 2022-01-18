using System.Collections.Generic;
using UnityEngine;

namespace Kalimag.Modding.Unity.Visualization
{
	internal class GraphicsHelper
    {
        private static readonly Dictionary<PrimitiveType, Mesh> PrimitiveMeshes = new Dictionary<PrimitiveType, Mesh>();

        public static float DefaultSphereColliderRadius { get; private set; }
        public static Vector3 DefaultBoxColliderSize { get; private set; }
        public static float DefaultCapsuleColliderRadius { get; private set; }
        public static float DefaultCapsuleColliderHeight { get; private set; }

        public static GameObject CreatePrimitive(PrimitiveType primitiveType)
        {
            var obj = new GameObject(primitiveType.ToString());

            var meshFilter = obj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = GetMesh(primitiveType);

            obj.AddComponent<MeshRenderer>();

            return obj;
        }

        private static Mesh GetMesh(PrimitiveType primitiveType)
        {
            if (!PrimitiveMeshes.TryGetValue(primitiveType, out var mesh))
            {
                var temp = GameObject.CreatePrimitive(primitiveType);
                var meshFilter = temp.GetComponent<MeshFilter>();
                mesh = meshFilter.sharedMesh;
                PrimitiveMeshes[primitiveType] = mesh;

                var collider = temp.GetComponent<Collider>();
                if (collider is SphereCollider sphereCollider)
                {
                    DefaultSphereColliderRadius = sphereCollider.radius;
                }
                else if (collider is BoxCollider boxCollider)
                {
                    DefaultBoxColliderSize = boxCollider.size;
                }
                else if (collider is CapsuleCollider capsuleCollider)
                {
                    DefaultCapsuleColliderRadius = capsuleCollider.radius;
                    DefaultCapsuleColliderHeight = capsuleCollider.height;
                }

                UnityEngine.Object.Destroy(temp);
            }
            return mesh;
        }

    }
}
