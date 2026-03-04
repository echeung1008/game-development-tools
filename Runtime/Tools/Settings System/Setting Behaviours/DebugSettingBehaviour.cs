using System;
using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    [Serializable]
    public class DebugSettingBehaviour : BaseSettingBehaviour
    {
        public override void OnValueChanged(object value)
        {
            Debug.Log($"Value CHANGED to {value.ToString()}");
        }

        public override void OnValueApplied(object value)
        {
            Debug.Log($"Value APPLIED to {value.ToString()}");
        }
    }
}
