using UnityEngine;

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
}
