using UnityEngine;
using System;
using System.Collections.Generic;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public abstract class EnumDropdownOptionGetter<T> : TypedSettingOptionGetter<DropdownOption>
        where T : Enum
    {
        protected override List<DropdownOption> GetTypedOptions()
        {
            List<DropdownOption> options = new();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                options.Add(new DropdownOption(e.ToString(), e));
            }

            return options;
        }
    }
}