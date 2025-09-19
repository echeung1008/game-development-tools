using System.Linq;
using UnityEngine;

namespace BlueMuffinGames.Tools.DynamicPath
{
    public class ConcaveUpMeshData : MeshData
    {
        public override void Initialize(Vector3[] vertices, float height)
        {
            if (vertices.Length < 3)
            {
                Debug.LogWarning($"Tried to initialize a ConcaveUpMeshData with less than three vertices ({vertices.Length})");
                return;
            }

            Vector3 center = vertices[vertices.Length / 2];
            Vector3 bottomCenter = center + Vector3.down * height;

            Vector3 bottomLeft = vertices[0];
            bottomLeft.y -= height;

            Vector3 bottomRight = vertices[vertices.Length - 1];
            bottomRight.y -= height;

            int centerIndex = (vertices.Length / 2) + 3;

            this.vertices = new Vector3[] { bottomLeft, bottomRight, bottomCenter };
            this.vertices = this.vertices.Concat(vertices).ToArray();

            triangles = new int[(vertices.Length + 2) * 3];

            // left
            for (int i = 3; i < centerIndex; i++)
            {
                AddTriangle(0, i, i + 1);
            }

            // center
            AddTriangle(0, centerIndex, 2);
            AddTriangle(2, centerIndex, 1);

            // right
            for (int i = centerIndex; i < this.vertices.Length - 1; i++)
            {
                AddTriangle(1, i, i + 1);
            }

            preMergeLength = this.vertices.Length;
        }

        public override int GetBottomLeftIndex() => 0;
        public override int GetBottomRightIndex() => 1;
        public override int GetTopLeftIndex() => 3;
        public override int GetTopRightIndex() => preMergeLength - 1;
    }
}
