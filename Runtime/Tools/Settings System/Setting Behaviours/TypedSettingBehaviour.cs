using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public abstract class TypedSettingBehaviour<T> : BaseSettingBehaviour
        where T : struct
    {
        public sealed override void OnValueApplied(object value)
        {
            if (value is not T casted)
            {
                Debug.LogError($"(TypedSettingBehaviour) Failed to cast value {value} to type {typeof(T)} for setting ID {TargetId}");
                return;
            }

            OnTypedValueApplied(casted);
        }

        protected abstract void OnTypedValueApplied(T value);
    }
}
