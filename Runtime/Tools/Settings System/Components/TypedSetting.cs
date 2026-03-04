using System;
using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public abstract class TypedSetting<T> : BaseSetting
    {
        public override void Initialize()
        {
            base.Initialize();

            if (BaseSettingsManager.Instance == null)
            {
                Debug.LogError($"(TypedSetting) Tried to initialize TypedSetting before BaseSettingsManager was initialized.");
                return;
            }

            if (!BaseSettingsManager.Instance.TryGetValue(ID, out T value))
            {
                Debug.LogError($"(TypedSetting) Failed to get the initial value of setting {ID}.");
                return;
            }

            SetVisualValue(value);
        }

        public override void ResetSetting()
        {
            base.ResetSetting();

            // set visual to new value
            if (BaseSettingsManager.Instance != null && BaseSettingsManager.Instance.TryGetValue(ID, out T value, onlyApplied: false)) 
                SetVisualValue(value); 
        }

        protected abstract void SetVisualValue(T value);

        protected virtual void OnValueChanged(T value)
        {
            SetSetting(value);
        }
    }
}
