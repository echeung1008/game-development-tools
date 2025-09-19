using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 AddAngle(this Vector3 vector3, float angleToAdd)
    {
        float magnitude = vector3.magnitude;
        float currentAngle = Mathf.Atan2(vector3.y, vector3.x) * Mathf.Rad2Deg;

        currentAngle += angleToAdd;

        currentAngle = Mathf.Repeat(currentAngle, 360f);

        float angleRad = currentAngle * Mathf.Deg2Rad;

        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * magnitude;
    }

    public static float ToDegrees(this Vector3 vector)
    {
        float angle = Mathf.Atan2(-vector.x, vector.y) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;
        return angle;
    }
}
