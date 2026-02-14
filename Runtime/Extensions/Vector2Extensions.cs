using UnityEngine;

namespace BlueMuffinGames.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 AddAngle(this Vector2 vector2, float angleToAdd)
        {
            float magnitude = vector2.magnitude;
            float currentAngle = Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;

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

        public static bool Approximately(this Vector2 vector3, Vector2 other)
        {
            return Mathf.Approximately(vector3.x, other.x) &&
                Mathf.Approximately(vector3.y, other.y);
        }
    }
}