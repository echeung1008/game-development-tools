using System;
using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    [Serializable]
    public struct SettingDefinition
    {
        [SerializeField] private string _id;
        [SerializeField] private Type _type;
        [SerializeField] private string _defaultValue;

        [SerializeReference, SerializeReferenceTypePicker] private BaseSettingBehaviour _behaviour;
        [SerializeReference, SerializeReferenceTypePicker] private BaseOptionProvider _optionProvider;

        public string ID => _id;
        public Type SettingType => _type;
        public BaseSettingBehaviour Behaviour => _behaviour;
        public BaseOptionProvider OptionProvider => _optionProvider;

        public bool TryGetDefaultValue(out object defaultValue)
        {
            defaultValue = default;
            switch (SettingType)
            {
                case Type.Bool:
                    if (bool.TryParse(_defaultValue, out bool boolValue))
                    {
                        defaultValue = boolValue;
                        return true;
                    }
                    return false;
                case Type.Int:
                    if (int.TryParse(_defaultValue, out int intValue))
                    {
                        defaultValue = intValue;
                        return true;
                    }
                    return false;
                case Type.Float:
                    if (float.TryParse(_defaultValue, out float floatValue))
                    {
                        defaultValue = floatValue;
                        return true;
                    }
                    return false;
                case Type.String:
                    defaultValue = _defaultValue;
                    return true;
                default:
                    return false;
            }
        }

        public enum Type
        {
            None,
            Bool,
            Int,
            Float,
            String,
        }
    }
}
