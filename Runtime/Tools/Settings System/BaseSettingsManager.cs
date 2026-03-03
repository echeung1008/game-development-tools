using System.Collections.Generic;
using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public class BaseSettingsManager : MonoBehaviour
    {
        public static BaseSettingsManager Instance { get; private set; }

        [SerializeField] private SettingsRegistry _registry;
        [SerializeField] private List<BaseSettingBehaviour> _behaviours;
        [SerializeField] private List<BaseSettingOptionGetter> _optionGetters;

        private Dictionary<string, SettingGroup> _registeredSettingGroups = new();
        private Dictionary<string, SettingDefinition> _registeredSettingDefinitions = new();

        private Dictionary<string, object> _registeredValues = new();
        private Dictionary<string, BaseSettingBehaviour> _registeredBehaviours = new();
        private Dictionary<string, BaseSettingOptionGetter> _registeredOptionGetters = new();
        private Dictionary<string, object> _changeRegistry = new();

        public virtual bool TryGetValue<T>(string id, out T value, bool onlyApplied = true)
        {
            value = default;

            object uncastedValue = null;

            // allow the change registry to return value if not onlyApplied
            if (!onlyApplied && !_changeRegistry.TryGetValue(id, out uncastedValue))
            {
                Debug.LogError($"(BaseSettingsManager) Change Registry does not contain the id {id}");
                return false;
            }
            else if (!_registeredValues.TryGetValue(id, out uncastedValue))
            {
                Debug.LogError($"(BaseSettingsManager) Registry does not contain the id {id}");
                return false;
            }

            if (uncastedValue is not T castedValue)
            {
                Debug.LogError($"(BaseSettingsManager) Failed to cast value {uncastedValue} to type {typeof(T)}");
                return false;
            }

            value = castedValue;
            return true;
        }

        public virtual void RecordChange(string id, object value)
        {
            _changeRegistry[id] = value;

            if (_registeredBehaviours.TryGetValue(id, out var behaviour))
            {
                behaviour.OnValueChanged(value);
            }
        }

        public virtual void PushAllChanges()
        {
            foreach (var pair in _changeRegistry)
            {
                if (!_registeredBehaviours.TryGetValue(pair.Key, out var behaviour)) continue;

                behaviour.OnValueApplied(pair.Value);
                SaveSetting(pair.Key, pair.Value);

                _registeredValues[pair.Key] = pair.Value;
                
                // check if it was set back to the default value
                if (_registeredSettingDefinitions.TryGetValue(pair.Key, out var definition) && 
                    definition.TryGetDefaultValue(out var defaultValue) &&
                    defaultValue.Equals(pair.Value)
                )
                {
                    var saveKey = GetSaveKey(pair.Key);
                    if (PlayerPrefs.HasKey(saveKey)) PlayerPrefs.DeleteKey(saveKey);
                }
            }
        }

        public virtual void ResetSetting(string id)
        {
            if (!_registeredSettingDefinitions.TryGetValue(id, out var definition))
            {
                Debug.LogError($"(BaseSettingsManager) Failed to get the definition of setting {id} for resetting.");
                return;
            }

            if (definition.SettingType == SettingDefinition.Type.None) return;

            if (!definition.TryGetDefaultValue(out var defaultValue))
            {
                Debug.LogError($"(BaseSettingsManager) Failed to parse setting definition's {definition.ID} default value to the setting's type {definition.SettingType}.");
                return;
            }

            RecordChange(id, defaultValue);
        }

        public virtual void ClearAllChanges()
        {
            _changeRegistry.Clear();
        }

        public virtual void ResetAllSettingsInGroup(string groupId, bool applyChanges = false)
        {
            if (!_registeredSettingGroups.TryGetValue(groupId, out var group))
            {
                Debug.LogError($"(BaseSettingsManager) Registered Groups does not contain a group with id {groupId}");
                return;
            }

            foreach (var definition in group.Definitions)
            {
                ResetSetting(definition.ID);
            }

            if (applyChanges) PushAllChanges();
        }

        public virtual bool TryGetSettingOptions(string id, out List<object> options, bool useCache = true)
        {
            options = null;
            if (!_registeredOptionGetters.TryGetValue(id, out var getter))
            {
                Debug.LogError($"(BaseSettingsManager) Option Getter Registry does not contain a getter for id {id}");
                return false;
            }

            options = getter.GetOptions(useCache);
            return true;
        }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                RegisterSettingBehaviours();
                RegisterOptionGetters();
                RegisterSettings();
            }
            else Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        protected virtual bool TryGetSavedSetting(string id, SettingDefinition.Type type, out object value)
        {
            value = default;
            var key = GetSaveKey(id);

            if (!PlayerPrefs.HasKey(key)) return false;

            switch (type)
            {
                case SettingDefinition.Type.Bool:
                    var boolString = PlayerPrefs.GetString(key);
                    if (bool.TryParse(boolString, out bool boolValue))
                    {
                        value = boolValue;
                        return true;
                    }
                    return false;
                case SettingDefinition.Type.Int:
                    value = PlayerPrefs.GetInt(key);
                    return true;
                case SettingDefinition.Type.Float:
                    value = PlayerPrefs.GetFloat(key);
                    return true;
                case SettingDefinition.Type.String:
                    value = PlayerPrefs.GetString(key);
                    return true;
                default:
                    return false;
            }
        }

        protected virtual void SaveSetting<T>(string id, T value)
        {
            var key = GetSaveKey(id);

            switch (value)
            {
                case int intValue:
                    PlayerPrefs.SetInt(key, intValue);
                    break;
                case float floatValue:
                    PlayerPrefs.SetFloat(key, floatValue);
                    break;
                default:
                    PlayerPrefs.SetString(key, value.ToString());
                    break;
            }
        }

        private void RegisterSettingBehaviours()
        {
            foreach (var behaviour in _behaviours)
            {
                if (_registeredBehaviours.ContainsKey(behaviour.TargetId))
                {
                    Debug.LogWarning($"(BaseSettingsManager) A BaseSettingBehaviour with TargetId {behaviour.TargetId} has already been registered. Skipping...");
                    continue;
                }

                _registeredBehaviours[behaviour.TargetId] = behaviour;
            }
        }

        private void RegisterOptionGetters()
        {
            foreach (var optionGetter in _optionGetters)
            {
                if (_registeredOptionGetters.ContainsKey(optionGetter.TargetId))
                {
                    Debug.LogWarning($"(BaseSettingsManager) A BaseSettingOptionGetter with TargetId {optionGetter.TargetId} has already been registered. Skipping...");
                    continue;
                }

                _registeredOptionGetters[optionGetter.TargetId] = optionGetter;
            }
        }

        private void RegisterSettings()
        {
            foreach (var group in _registry.Groups)
            {
                if (_registeredSettingGroups.ContainsKey(group.ID))
                {
                    Debug.LogWarning($"(BaseSettingsManager) A SettingGroup with ID {group.ID} has already been registered. Skipping...");
                    continue;
                }

                _registeredSettingGroups[group.ID] = group;

                foreach (var definition in group.Definitions)
                {
                    if (_registeredSettingDefinitions.ContainsKey(definition.ID))
                    {
                        Debug.LogWarning($"(BaseSettingsManager) A SettingDefinition with ID {definition.ID} has already been registered. Skipping...");
                        continue;
                    }

                    _registeredSettingDefinitions[definition.ID] = definition;

                    if (definition.TryGetDefaultValue(out var defaultValue))
                    {
                        object initialValue = defaultValue;
                        if (TryGetSavedSetting(definition.ID, definition.SettingType, out object savedValue)) initialValue = savedValue;

                        RecordChange(definition.ID, initialValue);
                    }
                    else if (definition.SettingType != SettingDefinition.Type.None)
                    {
                        Debug.LogError($"(BaseSettingsManager) Failed to parse setting definition's {definition.ID} default value to the setting's type {definition.SettingType}.");
                    }
                }
            }

            PushAllChanges();
        }

        protected virtual string GetSaveKey(string id) => $"Setting: {id}";
    }
}
