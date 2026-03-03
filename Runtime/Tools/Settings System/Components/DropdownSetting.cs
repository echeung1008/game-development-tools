using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public class DropdownSetting : TypedSetting<DropdownOption>
	{
		[SerializeField] private Dropdown _dropdown;

        protected override void SetVisualValue(DropdownOption value)
        {
            
        }
    }
}