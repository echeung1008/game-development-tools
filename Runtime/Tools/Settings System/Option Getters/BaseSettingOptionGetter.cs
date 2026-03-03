using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
	public abstract class BaseSettingOptionGetter : ScriptableObject
	{
        [SerializeField] private string _targetId;

        public string TargetId => _targetId;

        protected List<object> _options;

        public abstract List<object> GetOptions(bool useCache);
    }
}