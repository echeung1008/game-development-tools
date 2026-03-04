using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public abstract class DropdownOptionProvider<T> : BaseOptionProvider
    {
        public sealed override List<object> GetBoxedOptions(bool useCache)
        {
            if (useCache && _options != null && _options.Count > 0) return _options;

            _options = GetOptions().Select(t => (object)t).ToList();
            return _options;
        }

        public abstract List<T> GetOptions();
    }
}