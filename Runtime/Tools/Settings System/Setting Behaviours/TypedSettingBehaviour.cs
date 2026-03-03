using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public abstract class TypedSettingBehaviour<T> : BaseSettingBehaviour
        where T : struct
    {
        public sealed override void OnValueChanged(object value)
        {
            if (value is not T casted)
            {
                Debug.LogError($"(TypedSettingBehaviour) Failed to cast value {value} to type {typeof(T)} for setting ID {TargetId}");
                return;
            }

            OnValueChanged(casted);
        }

        public sealed override void OnValueApplied(object value)
        {
            if (value is not T casted)
            {
                Debug.LogError($"(TypedSettingBehaviour) Failed to cast value {value} to type {typeof(T)} for setting ID {TargetId}");
                return;
            }

            OnValueApplied(casted);
        }

        protected virtual void OnValueChanged(T value) { }
        protected virtual void OnValueApplied(T value) { }
    }
}
