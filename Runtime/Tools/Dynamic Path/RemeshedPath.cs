using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlueMuffinGames.Tools.DynamicPath
{
    [System.Serializable]
    public class RemeshedPath : Path
    {
        [SerializeField]
        public Path targetPath;

        [Header("Settings")]
        [SerializeField]
        public float maxVoxelSize = 0.1f;

        [SerializeField]
        private int maxIterations = 1000;

        [SerializeField]
        public RemeshMode remeshMode = RemeshMode.Complex;

        private PolygonCollider2D polygonCollider;

        public override void CopyShape(Vector3 startPosition, List<Vector3> target, bool disableCollider = false)
        {
            base.CopyShape(startPosition, target);

            UpdatePolygonCollider(disableCollider);
        }

        public void SimpleRemesh(bool clearImmediate = false)
        {
            if (targetPath == null || targetPath.nodes.Count <= 2 || maxVoxelSize <= 0.01f) return;

            ClearPath(clearImmediate);

            AddRootNode(targetPath.nodes[0].Position);

            float distanceFromEnd = Node.Distance(targetPath.nodes[0], targetPath.nodes.Last());
            int index = 0;
            Node left = targetPath.nodes[index];
            Node right = targetPath.nodes[index + 1];
            int iterations = 0;

            while (distanceFromEnd > maxVoxelSize && index < targetPath.nodes.Count && iterations < maxIterations)
            {
                // get reference point
                Node lastNode = nodes.Count == 0 ? targetPath.nodes.First() : nodes.Last();
                Vector3 referencePoint = lastNode.Tangent + lastNode.Position;

                // update left and right targetPath nodes
                if (referencePoint.x > right.X)
                {
                    index++;
                    if (index < targetPath.nodes.Count - 1)
                    {
                        left = targetPath.nodes[index];
                        right = targetPath.nodes[index + 1];
                    }
                }

                // calculate weighted tangent
                float distanceA = Vector3.Distance(left.Position, referencePoint);
                float distanceB = Vector3.Distance(right.Position, referencePoint);
                float totalDistance = distanceA + distanceB;
                float weightedSlope = left.M * (1f - distanceA / totalDistance) + right.M * (1f - distanceB / totalDistance);
                Vector3 weightedTangent = new Vector3(1f, weightedSlope, 0f).normalized;

                // extend path along weighted tangent
                ExtendPath(nodes.Last().Position + weightedTangent * maxVoxelSize);

                distanceFromEnd = Node.Distance(targetPath.nodes.Last(), nodes.Last());
                iterations++;
            }

            // check last node
            Node last = nodes.Last();
            if (last.X < targetPath.nodes.Last().X)
            {
                ExtendPath(last.Position + last.Tangent * maxVoxelSize);
                last = nodes.Last();
            }

            if (last.X > targetPath.nodes.Last().X)
            {
                last.transform.position = new Vector3(targetPath.nodes.Last().X, last.M * (targetPath.nodes.Last().X - last.transform.position.x) + last.transform.position.y, last.transform.position.z);
            }

            UpdateNodeNames();
            UpdatePolygonCollider(false);
        }

        public void ComplexRemesh(bool clearImmediate = false)
        {
            if (!targetPath || targetPath.nodes == null || targetPath.nodes.Count < 2) return;
            if (maxVoxelSize <= 0.01f) return;

            // Start clean
            ClearPath(clearImmediate);

            // Gather control points (XY), keep Z from first
            int n = targetPath.nodes.Count;
            var P = new Vector2[n];
            float z0 = targetPath.nodes[0].Position.z;
            for (int i = 0; i < n; i++) P[i] = (Vector2)targetPath.nodes[i].Position;

            // Tangents: Catmull–Rom (cardinal, tension 0.5)
            var T = new Vector2[n];
            const float tension = 0.5f;
            for (int i = 0; i < n; i++)
            {
                if (i == 0) T[i] = tension * (P[1] - P[0]);
                else if (i == n - 1) T[i] = tension * (P[n - 1] - P[n - 2]);
                else T[i] = tension * (P[i + 1] - P[i - 1]);
            }

            // Place the first point
            AddRootNode(new Vector3(P[0].x, P[0].y, z0));
            Vector2 lastPlaced = P[0];

            // We’ll walk segments in order, carrying leftover distance between segments
            float leftover = 0f; // distance already accumulated towards next voxel on the current segment
            int iter = 0;

            for (int seg = 0; seg < n - 1 && iter < maxIterations; seg++)
            {
                // Skip degenerate segments
                if ((P[seg + 1] - P[seg]).sqrMagnitude < 1e-12f)
                    continue;

                float uStart = 0f;
                // If the last placed point is NOT exactly at P[seg], find its param on this segment (rare—usually we end on a boundary).
                // For simplicity we assume we always either start at u=0 or we already advanced uStart within this loop.
                // (We only end a loop on u==1, carrying leftover to next segment.)

                while (iter++ < maxIterations)
                {
                    // Distance we still need before adding the next node
                    float target = maxVoxelSize - leftover;

                    // How much arc length remains to the end of this segment?
                    float segRemaining = ArcLenSegment(P[seg], P[seg + 1], T[seg], T[seg + 1], uStart, 1f);

                    if (segRemaining + 1e-5f < target)
                    {
                        // Not enough length left; carry and go to next segment
                        leftover += segRemaining;
                        break; // move to next seg
                    }

                    // Find u where arc length from uStart to u == target (bisection)
                    float u = FindUForArcDistance(P[seg], P[seg + 1], T[seg], T[seg + 1], uStart, target);

                    Vector2 p = Hermite(P[seg], P[seg + 1], T[seg], T[seg + 1], u);
                    ExtendPath(new Vector3(p.x, p.y, z0));
                    lastPlaced = p;

                    // Reset for the next step on the same segment
                    uStart = u;
                    leftover = 0f;

                    // If we landed at the very end of the segment, hop to next
                    if (u >= 1f - 1e-5f) break;
                }
            }

            // Ensure exact endpoint (allowed to be < maxVoxelSize from last)
            Vector2 end = P[n - 1];
            if (((Vector2)nodes.Last().Position - end).sqrMagnitude > 1e-10f)
                ExtendPath(new Vector3(end.x, end.y, z0));

            UpdateNodeNames();
            UpdatePolygonCollider(false);
        }

        public void Clear(bool clearImmediate = false)
        {
            ClearPath(clearImmediate);

            if (polygonCollider == null) polygonCollider = GetComponent<PolygonCollider2D>();

            if (polygonCollider != null)
            {
                polygonCollider.pathCount = 1;
                List<Vector2> path = nodes.Where(n => n != null).Select(n => (Vector2)n.LocalPosition).ToList();
                polygonCollider.SetPath(0, path);
            }
        }

        private void UpdatePolygonCollider(bool disableCollider)
        {
            if (polygonCollider == null && transform.parent != null) polygonCollider = transform.parent.GetComponent<PolygonCollider2D>();

            if (polygonCollider != null)
            {
                if (!disableCollider)
                {
                    polygonCollider.enabled = true;
                    List<Vector2> path = nodes.Where(n => n != null).Select(n => (Vector2)n.LocalPosition).ToList();
                    path.Insert(0, path.First() + Vector2.down * 20f);
                    path.Add(path.Last() + Vector2.down * 20f);
                    for (int i = path.Count - 10; i > 10; i -= 10)
                    {
                        path.Add(path[i] + Vector2.down * 20f);
                    }
                    polygonCollider.SetPath(0, path);
                }
                else polygonCollider.enabled = false;
            }
        }

        // ---------- Curve math ----------

        // Cubic Hermite position
        private static Vector2 Hermite(Vector2 p0, Vector2 p1, Vector2 m0, Vector2 m1, float u)
        {
            float u2 = u * u, u3 = u2 * u;
            float h00 = 2f * u3 - 3f * u2 + 1f;
            float h10 = u3 - 2f * u2 + u;
            float h01 = -2f * u3 + 3f * u2;
            float h11 = u3 - u2;
            return h00 * p0 + h10 * m0 + h01 * p1 + h11 * m1;
        }

        // Approximate arc length on [ua, ub] with composite chords (adaptive-ish)
        private static float ArcLenSegment(Vector2 p0, Vector2 p1, Vector2 m0, Vector2 m1, float ua, float ub)
        {
            // Use 8 samples (good tradeoff); you can bump to 12–16 for extreme curvature
            const int S = 8;
            float len = 0f;
            Vector2 prev = Hermite(p0, p1, m0, m1, ua);
            for (int i = 1; i <= S; i++)
            {
                float u = Mathf.Lerp(ua, ub, i / (float)S);
                Vector2 cur = Hermite(p0, p1, m0, m1, u);
                len += (cur - prev).magnitude;
                prev = cur;
            }
            return len;
        }

        // Solve for u where arc length from u0 to u equals targetDist (monotonic ⇒ bisection)
        private static float FindUForArcDistance(Vector2 p0, Vector2 p1, Vector2 m0, Vector2 m1, float u0, float targetDist)
        {
            float lo = u0;
            float hi = 1f;
            // Quick reject: if full remaining length < target, return hi
            float rem = ArcLenSegment(p0, p1, m0, m1, u0, 1f);
            if (rem <= targetDist) return 1f;

            for (int it = 0; it < 24; it++) // 24 iters ≈ sub-mm precision typically
            {
                float mid = 0.5f * (lo + hi);
                float len = ArcLenSegment(p0, p1, m0, m1, u0, mid);
                if (len < targetDist) lo = mid; else hi = mid;
                if (hi - lo < 1e-5f) break;
            }
            return hi;
        }
    }

    public enum RemeshMode
    {
        Simple,
        Complex
    }
}
