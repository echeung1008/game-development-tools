using System;
using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    [Serializable]
    public abstract class BaseSettingBehaviour
    {
        public abstract void OnValueChanged(object value);
        public abstract void OnValueApplied(object value);
    }
}
