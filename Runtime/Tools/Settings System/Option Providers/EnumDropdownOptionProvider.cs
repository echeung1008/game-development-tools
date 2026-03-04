using UnityEngine;
using System;
using System.Collections.Generic;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public abstract class EnumDropdownOptionProvider<T> : DropdownOptionProvider
        where T : Enum
    {
        public override List<DropdownOption> GetOptions()
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