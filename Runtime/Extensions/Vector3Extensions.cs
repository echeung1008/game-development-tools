using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 ClampMagnitudeBetween(this Vector3 vector3, float min, float max)
    {
        Vector3 result = Vector3.ClampMagnitude(vector3, max);
        if (vector3.magnitude < min) result = result.normalized * min;
        return result;
    }
}
