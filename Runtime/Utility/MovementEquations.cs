using System.Runtime.CompilerServices;
using UnityEngine;

namespace BlueMuffinGames.GameDevelopmentTools.Utility
{
    public static class MovementEquations
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static float Clamp01(float t) => t < 0f ? 0f : (t > 1f ? 1f : t);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static float Sqr(float x) => x * x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static float Cube(float x) => x * x * x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInAndOut01(float t, float power = 2f)
        {
            t = Clamp01(t);
            if (t < 0.5f)
            {
                var x = t * 2f;
                return 0.5f * (power == 2f ? Sqr(x) : (power == 3f ? Cube(x) : Mathf.Pow(x, power)));
            }
            else
            {
                var x = (1f - t) * 2f;
                var val = 0.5f * (power == 2f ? Sqr(x) : (power == 3f ? Cube(x) : Mathf.Pow(x, power)));
                return 1f - val;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseIn01(float t, float power = 2f)
        {
            t = Clamp01(t);
            if (power == 2f) return Sqr(t);
            if (power == 3f) return Cube(t);
            return Mathf.Pow(t, power);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOut01(float t, float power = 2f)
        {
            t = Clamp01(t);
            var oneMinus = 1f - t;
            if (power == 2f) return 1f - oneMinus * oneMinus;
            if (power == 3f) return 1f - oneMinus * oneMinus * oneMinus;
            return 1f - Mathf.Pow(oneMinus, power);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BackAndUp01(float t, float backFactor = 1.0f)
        {
            t = Clamp01(t);
            backFactor = Mathf.Max(1e-6f, backFactor);
            var t2 = Sqr(t);
            var t3 = Cube(t);
            return backFactor * (t3 + (t2 / backFactor) - t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float UpAndBack01(float t, float upFactor = 0f)
        {
            t = Clamp01(t);
            var t2 = t * t;
            var exp = 1f + upFactor;
            float tPow = (Mathf.Approximately(exp, 2f)) ? t2 : Mathf.Pow(t, exp);
            return t2 * (3f - 2f * tPow);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float EaseInAndOut(float start, float end, float normalizedTime, float easeFactor) => Mathf.LerpUnclamped(start, end, EaseInAndOut01(normalizedTime, easeFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 EaseInAndOut(Vector2 start, Vector2 end, float normalizedTime, float easeFactor) => Vector2.LerpUnclamped(start, end, EaseInAndOut01(normalizedTime, easeFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 EaseInAndOut(Vector3 start, Vector3 end, float normalizedTime, float easeFactor) => Vector3.LerpUnclamped(start, end, EaseInAndOut01(normalizedTime, easeFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector4 EaseInAndOut(Vector4 start, Vector4 end, float normalizedTime, float easeFactor) => Vector4.LerpUnclamped(start, end, EaseInAndOut01(normalizedTime, easeFactor));

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float EaseIn(float start, float end, float normalizedTime, float easeFactor = 2f) => Mathf.LerpUnclamped(start, end, EaseIn01(normalizedTime, easeFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 EaseIn(Vector2 start, Vector2 end, float normalizedTime, float easeFactor = 2f) => Vector2.LerpUnclamped(start, end, EaseIn01(normalizedTime, easeFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 EaseIn(Vector3 start, Vector3 end, float normalizedTime, float easeFactor = 2f) => Vector3.LerpUnclamped(start, end, EaseIn01(normalizedTime, easeFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector4 EaseIn(Vector4 start, Vector4 end, float normalizedTime, float easeFactor = 2f) => Vector4.LerpUnclamped(start, end, EaseIn01(normalizedTime, easeFactor));

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float EaseOut(float start, float end, float normalizedTime, float easeFactor = 2f) => Mathf.LerpUnclamped(start, end, EaseOut01(normalizedTime, easeFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 EaseOut(Vector2 start, Vector2 end, float normalizedTime, float easeFactor = 2f) => Vector2.LerpUnclamped(start, end, EaseOut01(normalizedTime, easeFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 EaseOut(Vector3 start, Vector3 end, float normalizedTime, float easeFactor = 2f) => Vector3.LerpUnclamped(start, end, EaseOut01(normalizedTime, easeFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector4 EaseOut(Vector4 start, Vector4 end, float normalizedTime, float easeFactor = 2f) => Vector4.LerpUnclamped(start, end, EaseOut01(normalizedTime, easeFactor));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float BackAndUp(float start, float end, float normalizedTime, float backFactor = 1f) => Mathf.LerpUnclamped(start, end, BackAndUp01(normalizedTime, backFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 BackAndUp(Vector2 start, Vector2 end, float normalizedTime, float backFactor = 1f) => Vector2.LerpUnclamped(start, end, BackAndUp01(normalizedTime, backFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 BackAndUp(Vector3 start, Vector3 end, float normalizedTime, float backFactor = 1f) => Vector3.LerpUnclamped(start, end, BackAndUp01(normalizedTime, backFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector4 BackAndUp(Vector4 start, Vector4 end, float normalizedTime, float backFactor = 1f) => Vector4.LerpUnclamped(start, end, BackAndUp01(normalizedTime, backFactor));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float UpAndBack(float start, float end, float normalizedTime, float upFactor = 0f) => Mathf.LerpUnclamped(start, end, UpAndBack01(normalizedTime, upFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2 UpAndBack(Vector2 start, Vector2 end, float normalizedTime, float upFactor = 0f) => Vector2.LerpUnclamped(start, end, UpAndBack01(normalizedTime, upFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 UpAndBack(Vector3 start, Vector3 end, float normalizedTime, float upFactor = 0f) => Vector3.LerpUnclamped(start, end, UpAndBack01(normalizedTime, upFactor));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector4 UpAndBack(Vector4 start, Vector4 end, float normalizedTime, float upFactor = 0f) => Vector4.LerpUnclamped(start, end, UpAndBack01(normalizedTime, upFactor));
    }
}
