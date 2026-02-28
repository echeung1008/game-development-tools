using System.Collections.Generic;
using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    [CreateAssetMenu(fileName = "SettingsRegistry", menuName = "Scriptable Objects/SettingsRegistry")]
    public class SettingsRegistry : ScriptableObject
    {
        [SerializeField] private List<SettingGroup> _settings = new();

        public IReadOnlyList<SettingGroup> Groups => _settings;
    }
}
