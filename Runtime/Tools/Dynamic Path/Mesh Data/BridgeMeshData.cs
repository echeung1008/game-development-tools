using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BlueMuffinGames.Tools.DynamicPath
{
    public class BridgeMeshData : MeshData
    {
        public override void Initialize(Vector3[] vertices, float height)
        {
            if (vertices.Length < 3)
            {
                Debug.LogWarning($"Tried to initialize a BridgeMeshData with less than three vertices ({vertices.Length})");
                return;
            }

            List<Vector3> remappedVertices = new List<Vector3>();

            for (int i = 0; i < vertices.Length; i++)
            {
                remappedVertices.Add(vertices[i] + Vector3.down * height);
            }
            remappedVertices = remappedVertices.Concat(vertices).ToList();

            this.vertices = remappedVertices.ToArray();
            triangles = new int[(vertices.Length - 1) * 6];

            for (int i = 0; i < vertices.Length - 1; i++)
            {
                AddTriangle(i, i + vertices.Length, i + 1);
                AddTriangle(i + 1, i + vertices.Length, i + 1 + vertices.Length);
            }

            preMergeLength = this.vertices.Length;
        }

        public override int GetBottomLeftIndex() => 0;
        public override int GetBottomRightIndex() => preMergeLength / 2 - 1;
        public override int GetTopLeftIndex() => preMergeLength / 2;
        public override int GetTopRightIndex() => preMergeLength - 1;
    }
}
