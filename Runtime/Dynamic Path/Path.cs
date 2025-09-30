using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace BlueMuffinGames.Tools.DynamicPath
{
    [System.Serializable]
    public class Path : MonoBehaviour
    {
        [SerializeField]
        private GameObject nodePrefab;

        [SerializeField]
        private bool _clampNodesBetweenPrevAndNext = true;

        public List<Node> nodes = new List<Node>();

        public void AddNodeToFront()
        {
            if (nodes.Count == 0)
            {
                AddRootNode();
                return;
            }

            Node first = nodes.First();
            Node node = GetNewNode(first != null ? first.Position + Vector3.left * 2f : transform.position);

            nodes.Insert(0, node);
            RebuildNeighborReferences(0, 2);
            UpdateNodeNames();
        }

        public void AddNodeToEnd()
        {
            if (nodes.Count == 0)
            {
                AddRootNode();
                return;
            }

            Node last = nodes.Last();
            Node node = GetNewNode(last != null ? last.Position + Vector3.right * 2f : transform.position);

            nodes.Add(node);
            RebuildNeighborReferences(nodes.Count - 3, nodes.Count - 1);
            UpdateNodeNames();
        }

        public void InsertNewNode(int index)
        {
            if (nodes.Count == 0)
            {
                AddRootNode();
                return;
            }

            if (index < 0 || index > nodes.Count - 1) return;

            if (index == 0)
            {
                AddNodeToFront();
                return;
            }
            else if (index == nodes.Count - 1)
            {
                AddNodeToEnd();
                return;
            }

            Node next = nodes[index];
            Node prev = next.Previous;

            if (next != null && prev != null)
            {
                Node node = GetNewNode((next.Position + prev.Position) / 2f);

                nodes.Insert(index, node);
            }

            RebuildNeighborReferences(index - 1, index + 1);
            UpdateNodeNames();
        }

        public void DeleteNode(int index)
        {
            if (index < 0 || index >= nodes.Count) return;

#if UNITY_EDITOR
            Undo.RecordObject(this, "Delete Path Node");
#endif

            var removed = nodes[index];
            nodes.RemoveAt(index);

#if UNITY_EDITOR
            if (removed) Undo.DestroyObjectImmediate(removed.gameObject);
#else
    if (removed) Destroy(removed.gameObject);
#endif

            RebuildNeighborReferences(index - 1, index);
            UpdateNodeNames();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void ClearPath(bool clearImmediate)
        {
            try
            {
                ReleaseNodes();
            }
            catch
            {
                nodes = new List<Node>();
            }
        }

        public void MoveNodeUp(int index)
        {
            if (index <= 0 || index > nodes.Count - 1) return;

            var tmp = nodes[index];
            nodes[index] = nodes[index - 1];
            nodes[index - 1] = tmp;

            RebuildNeighborReferences(index - 2, index + 1);
            UpdateNodeNames();
        }

        public void MoveNodeDown(int index)
        {
            if (index < 0 || index >= nodes.Count - 1) return;

            var tmp = nodes[index];
            nodes[index] = nodes[index + 1];
            nodes[index + 1] = tmp;

            RebuildNeighborReferences(index - 1, index + 2);
            UpdateNodeNames();
        }

        public void ExtendPath(Vector3 position)
        {
            if (nodes.Count == 0) AddRootNode(position);

            nodes.Add(GetNewNode(position));

            RebuildNeighborReferences(nodes.Count - 3, nodes.Count - 1);
        }

        public void AddRootNode(Vector3 position)
        {
            if (nodes.Count != 0) return;

            nodes.Add(GetNewNode(position));

            UpdateNodeNames();
        }

        public void AddRootNode(Node node)
        {
            if (nodes.Count != 0) return;

            nodes.Add(node);
            Debug.Log("Used existing Node as the new root: " + node.Position);

            UpdateNodeNames();
        }

        public void UpdateNodeNames()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].gameObject.name = "Node " + i;
                nodes[i].transform.SetSiblingIndex(i);
            }
        }

        public void ReleaseNodes()
        {
            if (nodes.Count == 0) return;

            Node last = nodes.Last();

            if (last.Next != null)
            {
                nodes.RemoveAt(nodes.Count - 1);
                last.transform.SetParent(last.Next.transform.parent, true);
            }

            if (NodePool.Instance != null) NodePool.ReleaseNodes(nodes.ToArray());
            nodes.Clear();
        }

        public bool TryGetSurroundingNodes(float x, out Node left, out Node right)
        {
            left = right = null;
            if (nodes == null || nodes.Count < 2) return false;

            if (x <= nodes[0].Position.x || x >= nodes[nodes.Count - 1].Position.x) return false;

            int lo = 0;
            int hi = nodes.Count - 1;
            while (hi - lo > 1)
            {
                int mid = (lo + hi) / 2;
                float mx = nodes[mid].Position.x;

                if (x < mx) hi = mid;
                else lo = mid;
            }

            left = nodes[lo];
            right = nodes[hi];
            return true;
        }

        public virtual void CopyShape(Vector3 startPosition, List<Vector3> target, bool disableCollider = false)
        {
            if (target == null || target.Count == 0) return;

            ClearPath(false);

            AddRootNode(startPosition);

            Vector3 reference = target[0];

            for (int i = 1; i < target.Count; i++)
            {
                ExtendPath(startPosition + WorldToLocal(reference, target[i]));
            }

            Vector3 WorldToLocal(Vector3 reference, Vector3 worldPosition) => worldPosition - reference;
        }

        public virtual List<Vector3> GetMirror()
        {
            if (nodes == null || nodes.Count == 0) return null;
            if (nodes.Count == 1)
            {
                return new List<Vector3>() { nodes[0].Position };
            }

            List<Vector3> mirror = new List<Vector3>();
            float center = nodes.Last().Position.x + nodes.First().Position.x / 2f;
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                Node node = nodes[i];
                Vector3 newPos = new Vector3(
                    node.X + 2f * (center - node.X),
                    node.Y,
                    node.Position.z
                );
                mirror.Add(newPos);
            }

            return mirror;
        }

        public bool IsPointInsidePolygon(Vector3 point) => nodes.Select((n) => n.Position).ToList().IsPointInsidePolygon(point);

        private void AddRootNode()
        {
            AddRootNode(transform.position);
        }

        private Node GetNewNode(Vector3 position)
        {
            GameObject go;
            // Don’t use pooling in edit-time
            if (Application.isPlaying && NodePool.Instance != null)
            {
                var pooled = NodePool.GetNode();
                pooled.transform.position = position;
                pooled.transform.SetParent(transform, true);
                return pooled;
            }

            // Editor / Prefab Mode safe instantiate
#if UNITY_EDITOR
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                // Instantiate into the prefab stage scene so it’s saved to the asset
                go = (GameObject)PrefabUtility.InstantiatePrefab(nodePrefab, stage.scene);
                Undo.RegisterCreatedObjectUndo(go, "Add Path Node");
                // Parent under the Path instance that lives inside the stage
                go.transform.SetParent(transform, true);
            }
            else
            {
                go = (GameObject)PrefabUtility.InstantiatePrefab(nodePrefab);
                Undo.RegisterCreatedObjectUndo(go, "Add Path Node");
                go.transform.SetParent(transform, true);
            }
#else
            go = Instantiate(nodePrefab);
            go.transform.SetParent(transform, true);
#endif

            go.transform.position = position;
#if UNITY_EDITOR
            EditorUtility.SetDirty(go);
#endif
            Node result = go.GetComponent<Node>();
            result.clampBetweenPrevAndNext = _clampNodesBetweenPrevAndNext;
            return result;
        }

        private void RebuildNeighborReferences(int start, int end)
        {
            start = Mathf.Max(start, 0);
            end = Mathf.Min(end, nodes.Count - 1);

            for (int i = start; i <= end; i++)
            {
                if (i == nodes.Count - 1 && i > 0)
                {
                    nodes[i].Previous = nodes[i - 1];
                }
                else
                {
                    if (i > start) nodes[i].Previous = nodes[i - 1];
                    if (i < end) nodes[i].Next = nodes[i + 1];
                }
            }
        }

    }
}
