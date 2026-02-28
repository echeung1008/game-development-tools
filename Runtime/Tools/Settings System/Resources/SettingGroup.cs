using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    [Serializable]
    public struct SettingGroup
    {
        [SerializeField] private string _id;
        [SerializeField] private List<SettingDefinition> _definitions;

        public string ID => _id;
        public IReadOnlyList<SettingDefinition> Definitions => _definitions;
    }
}
