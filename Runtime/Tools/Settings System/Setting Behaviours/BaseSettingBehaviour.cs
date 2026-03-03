using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public abstract class BaseSettingBehaviour : ScriptableObject
    {
        [SerializeField] private string _targetId;
        public string TargetId => _targetId;
        public abstract void OnValueChanged(object value);
        public abstract void OnValueApplied(object value);
    }
}
