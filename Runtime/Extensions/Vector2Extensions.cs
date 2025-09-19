using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 AddAngle(this Vector2 vector2, float angleToAdd)
    {
        float magnitude = vector2.magnitude;
        float currentAngle = Mathf.Atan2(vector3.y, vector2.x) * Mathf.Rad2Deg;

        currentAngle += angleToAdd;

        currentAngle = Mathf.Repeat(currentAngle, 360f);

        float angleRad = currentAngle * Mathf.Deg2Rad;

        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * magnitude;
    }

    public static float ToDegrees(this Vector2 vector)
    {
        float angle = Mathf.Atan2(-vector.x, vector.y) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;
        return angle;
    }
}
