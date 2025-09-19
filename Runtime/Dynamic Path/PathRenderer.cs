using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BlueMuffinGames.GameDevelopmentTools.DynamicPath.ConcavityRuns;
using static ConcavityRuns;

namespace BlueMuffinGames.Tools.DynamicPath
{
    [System.Serializable]
    public class PathRenderer : MonoBehaviour
    {
        [SerializeField]
        public MeshFilter meshFilter;

        [SerializeField]
        public Path targetPath;

        [SerializeField]
        public float meshHeight;

        [SerializeField]
        public bool stitchMeshes = true;

        [HideInInspector] public string renderResult;

        public void RenderPath(out string debug)
        {
            if (targetPath == null || targetPath.nodes.Count < 3 || meshFilter == null)
            {
                debug = "Failed to Render Path.";
                if (targetPath == null) debug += "\n\tTarget Path is null";
                if (targetPath.nodes.Count < 3) debug += "\n\tTarget Path has less than 3 nodes";
                if (meshFilter == null) debug += "\n\tMesh Filter is null";
                return;
            }

            (Kind Kind, Node[] Nodes)[] runs = SplitRuns(targetPath.nodes);
            List<MeshData> meshes = new List<MeshData>();

            debug = "---------RENDER RESULTS---------\n" + runs.Length.ToString() + " Runs were detected.\n";

            List<int> excludedRuns = new List<int>();

            for (int i = 0; i < runs.Length; i++)
            {
                if (excludedRuns.Contains(i)) continue;

                (Kind Kind, Node[] Nodes) = (runs[i].Kind, runs[i].Nodes);

                MeshData mesh = null;

                if (Kind == Kind.Down)
                {
                    mesh = new ConcaveDownMeshData();
                    debug += $"\tConcave Down ({Nodes.Length})";
                }
                else if (Kind == Kind.Up)
                {
                    mesh = new ConcaveUpMeshData();
                    debug += $"\tConcave Up ({Nodes.Length})";
                }
                else
                {
                    mesh = new BridgeMeshData();
                    debug += $"\tBridge ({Nodes.Length})";
                }


                if (mesh != null && Nodes.Length >= 3)
                {
                    Transform t = meshFilter.transform;
                    List<Vector3> path = Nodes.Where(n => n != null).Select(n => n.LocalPosition).ToList();

                    bool hasPreceedingStraggler = false;
                    bool hasProceedingStraggler = false;

                    // check next for stragglers 
                    if (i < runs.Length - 1 && !excludedRuns.Contains(i + 1) && runs[i + 1].Nodes.Length <= 2)
                    {
                        foreach (Node n in runs[i + 1].Nodes)
                        {
                            path.Add(n.LocalPosition);
                        }
                        excludedRuns.Add(i + 1);
                        hasProceedingStraggler = true;
                    }

                    // check previous for stragglers 
                    if (i > 0 && !excludedRuns.Contains(i - 1) && runs[i - 1].Nodes.Length <= 2)
                    {
                        for (int j = runs[i - 1].Nodes.Length - 1; j >= 0; j--)
                        {
                            path.Insert(0, runs[i - 1].Nodes[j].LocalPosition);
                        }
                        excludedRuns.Add(i - 1);
                        hasPreceedingStraggler = true;
                    }

                    mesh.Initialize(path.ToArray(), meshHeight);
                    meshes.Add(mesh);
                    debug += $" ... Successfully initialized:\n\t\tVertices ({mesh.vertices.Length})\n\t\tTriangles ({mesh.triangles.Length / 3})\n";

                    if (hasPreceedingStraggler)
                    {
                        debug += $"\t\tIncluded 1 preceeding straggler\n";
                    }

                    if (hasProceedingStraggler)
                    {
                        debug += $"\t\tIncluded 1 proceeding straggler\n";
                    }
                }
                else
                {
                    debug += " ... Patching...\n";
                }
            }

            if (meshes.Count > 0)
            {
                if (meshes.Count > 1)
                {
                    for (int i = meshes.Count - 2; i >= 0; i--)
                    {
                        MeshData other = meshes[i + 1];
                        meshes[i].Merge(other, stitchMeshes);
                    }
                }

                MeshData result = meshes[0];

                meshFilter.sharedMesh = result.CreateMesh(out string meshDebug);

                debug += $"Successfully created a mesh.\n{meshDebug}";
            }
            else debug += "ERROR: No meshes were built from this path.\n";
        }
    }

    public static class ConcavityRuns
    {
        public enum Kind { Up, Down, Bridge }

        public static (Kind Kind, Node[] Nodes)[] SplitRuns(IList<Node> src)
        {
            if (src == null || src.Count == 0) return Array.Empty<(Kind, Node[])>();

            const int MIN_LEN = 3; // runs shorter than this become Bridge

            var runs = new List<(Kind Kind, List<Node> Nodes)>();
            var currentList = new List<Node>();
            Kind currentKind = Kind.Bridge;

            void Flush()
            {
                if (currentList.Count == 0) return;
                runs.Add((currentKind, currentList));
                currentList = new List<Node>();
            }

            for (int i = 0; i < src.Count; i++)
            {
                var n = src[i];
                var k = KindOf(n);

                if (currentList.Count == 0)
                {
                    currentKind = k;
                    currentList.Add(n);
                }
                else if (k == currentKind)
                {
                    currentList.Add(n);
                }
                else
                {
                    Flush();
                    currentKind = k;
                    currentList.Add(n);
                }
            }
            Flush();

            var dirty = new List<(Kind Kind, List<Node> Nodes)>(runs.Count);
            foreach (var r in runs)
            {
                var kind = (r.Nodes.Count < MIN_LEN) ? Kind.Bridge : r.Kind;
                dirty.Add((kind, r.Nodes));
            }

            var coalesced = new List<(Kind Kind, List<Node> Nodes)>();
            foreach (var r in dirty)
            {
                if (coalesced.Count > 0 && coalesced[^1].Kind == r.Kind)
                {
                    // append nodes to the last run of the same kind
                    coalesced[^1].Nodes.AddRange(r.Nodes);
                }
                else
                {
                    // start a new run (clone list to avoid aliasing)
                    coalesced.Add((r.Kind, new List<Node>(r.Nodes)));
                }
            }

            var result = new (Kind Kind, Node[] Nodes)[coalesced.Count];
            for (int i = 0; i < coalesced.Count; i++)
                result[i] = (coalesced[i].Kind, coalesced[i].Nodes.ToArray());

            return result;
        }

        private static Kind KindOf(Node n)
        {
            if (n == null) return Kind.Bridge;
            bool up = n.ConcaveUp && !n.ConcaveDown;
            bool down = n.ConcaveDown && !n.ConcaveUp;
            if (up) return Kind.Up;
            if (down) return Kind.Down;
            return Kind.Bridge; // flat/unknown/both
        }
    }
}
