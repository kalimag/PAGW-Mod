using UnityEngine;

namespace Kalimag.Modding.Unity.Visualization.Colliders
{
	internal class MeshColliderVisualizer : ColliderVisualizer<MeshCollider>
    {

        public MeshColliderVisualizer(MeshCollider collider)
            : base(collider)
        { }

        protected override GameObject CreateVisualObject()
        {
            var obj = new GameObject("MeshColliderVisualizer VisualObject");
            obj.transform.SetParent(Collider.transform, false);

            var meshFilter = obj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = Collider.sharedMesh;

            //if (collider.sharedMesh)
            //{
            //var newMesh = Instantiate(collider.sharedMesh);
            //Physics.BakeMesh(newMesh.GetInstanceID(), collider.convex);
            //meshFilter.sharedMesh = newMesh;
            //}

            obj.AddComponent<MeshRenderer>();


            return obj;
        }

        public override void LateUpdate()
        {
        }

    }
}
