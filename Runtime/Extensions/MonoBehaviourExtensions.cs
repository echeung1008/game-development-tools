using BlueMuffinGames.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace BlueMuffinGames.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static Coroutine StartTimer(this MonoBehaviour monoBehaviour, float duration, ProgressTracker progressTracker = null, Action onTimeout = null)
        {
            if (duration <= 0f) return null;

            return monoBehaviour.StartCoroutine(Timer(duration, progressTracker, onTimeout));
        }

        private static IEnumerator Timer(float duration, ProgressTracker progressTracker = null, Action onTimeout = null)
        {
            if (duration <= 0f) { onTimeout?.Invoke(); yield break; }

            float currentTime = 0f;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                if (progressTracker != null) progressTracker.Progress = currentTime / duration;
            }

            if (progressTracker != null) progressTracker.Progress = 1f;
            onTimeout?.Invoke();
        }
    }
}