using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    [Serializable]
	public abstract class BaseOptionProvider
	{
        public event Action onOptionsChanged = delegate { };

        protected List<object> _options;

        public abstract List<object> GetBoxedOptions(bool useCache);
    }
}