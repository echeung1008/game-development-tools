using System.Linq;
using UnityEngine;

namespace BlueMuffinGames.Tools.DynamicPath
{
    public abstract class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;

        protected int preMergeLength = 0;
        private int triangleIndex = 0;

        public abstract void Initialize(Vector3[] path, float height);
        public abstract int GetBottomLeftIndex();
        public abstract int GetBottomRightIndex();
        public abstract int GetTopLeftIndex();
        public abstract int GetTopRightIndex();

        public void AddTriangle(int a, int b, int c)
        {
            if (triangles == null) return;

            if (triangleIndex >= triangles.Length)
            {
                Debug.LogWarning($"Attempted to add a triangle of index {triangleIndex} to triangles array of size {triangles.Length}");
                return;
            }

            triangles[triangleIndex++] = a;
            triangles[triangleIndex++] = b;
            triangles[triangleIndex++] = c;
        }

        public Mesh CreateMesh(out string debug)
        {
            if (vertices == null || triangles == null)
            {
                debug = "\t\tFailed because vertices or triangles wasn't initialized";
                return null;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            debug = $"\tVertices ({vertices.Length})\n\tTriangles ({triangles.Length / 3})";

            return mesh;
        }

        public void Merge(MeshData other, bool stitch)
        {
            if (vertices == null || triangles == null || other == null || other.vertices == null || other.triangles == null) return;

            int offset = vertices.Length;

            if (stitch)
            {
                // stitch
                int[] stitches = new int[] {
                GetBottomRightIndex(), GetTopRightIndex(), other.GetBottomLeftIndex() + offset,
                other.GetBottomLeftIndex() + offset, GetTopRightIndex(), other.GetTopLeftIndex() + offset
            };
                triangles = triangles.Concat(stitches).ToArray();
            }

            for (int i = 0; i < other.triangles.Length; i++)
            {
                other.triangles[i] += offset;
            }

            vertices = vertices.Concat(other.vertices).ToArray();
            triangles = triangles.Concat(other.triangles).ToArray();
        }
    }
}
