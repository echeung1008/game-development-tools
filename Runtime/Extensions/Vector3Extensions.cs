using System.Collections.Generic;
using UnityEngine;

namespace BlueMuffinGames.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 ClampMagnitudeBetween(this Vector3 vector3, float min, float max)
        {
            Vector3 result = Vector3.ClampMagnitude(vector3, max);
            if (vector3.magnitude < min) result = result.normalized * min;
            return result;
        }

        public static bool Approximately(this Vector3 vector3, Vector3 other)
        {
            return Mathf.Approximately(vector3.x, other.x) &&
                Mathf.Approximately(vector3.y, other.y) &&
                Mathf.Approximately(vector3.z, other.z);
        }

        /// <summary>
        /// Returns whether or not a given point is inside a closed polygon.
        /// </summary>
        /// <param name="path">The vertices of the polygon. The polygon is assumed to be closed.</param>
        /// <param name="p">The point in question.</param>
        /// <param name="useXZ">Whether to use the X and Z coordinates in the path. Uses X and Y if not.</param>
        /// <returns></returns>
        public static bool IsPointInsidePolygon(this List<Vector3> polygon, Vector2 p, bool useXZ = true)
        {
            if (polygon.Count < 3) return true;

            const float EPSILON = 1e-6f;
            int n = polygon.Count;

            // check if p is on an edge of the polygon
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                if (IsPointOnEdge(p, Vector3ToVector2(polygon[j]), Vector3ToVector2(polygon[i]))) return true;
            }

            bool inside = false;

            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                var A = Vector3ToVector2(polygon[j]);
                var B = Vector3ToVector2(polygon[i]);

                // check if the edge between a and b should be considered
                bool includeEdge = (A.y < p.y) != (B.y < p.y);

                if (includeEdge)
                {
                    // calculate x where the edge between A and B meets the horizontal line y = p.y
                    float intersection = (p.y - A.y) * (B.x - A.x) / (B.y - A.y) + A.x;

                    // if this intersection is to the right of point p, flip whether its inside
                    //  even number of intersections    =>  p is outside the polygon
                    //  odd number of intersections     =>  p is inside the polygon
                    if (p.x < intersection) inside = !inside;
                }
            }

            return inside;

            Vector2 Vector3ToVector2(Vector3 vector3)
            {
                return new Vector2(vector3.x, useXZ ? vector3.z : vector3.y);
            }

            bool IsPointOnEdge(Vector2 p, Vector3 a, Vector3 b)
            {
                // check if the point p lies on the edge between a and b
                // use cross product instead of slope to avoid divide by 0 error
                float cross = (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);
                if (Mathf.Abs(cross) > EPSILON) return false;

                // check if the point p is between the endpoints a and b
                float dot = (p.x - a.x) * (b.x - a.x) + (p.y - a.y) * (b.y - a.y);
                if (dot < -EPSILON) return false;
                float len2 = (b - a).sqrMagnitude;
                if (dot > len2 + EPSILON) return false;

                // only return true if the point lies on the same line drawn between A and B and is between A and B
                return true;
            }
        }
    }
}