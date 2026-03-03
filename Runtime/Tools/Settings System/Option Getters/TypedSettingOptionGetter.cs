using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public abstract class TypedSettingOptionGetter<T> : BaseSettingOptionGetter
    {
        public sealed override List<object> GetOptions(bool useCache)
        {
            if (useCache && _options != null && _options.Count > 0) return _options;

            _options = GetTypedOptions().Select(t => (object)t).ToList();
            return _options;
        }

        protected abstract List<T> GetTypedOptions();
    }
}