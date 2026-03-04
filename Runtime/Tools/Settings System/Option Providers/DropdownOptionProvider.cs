using UnityEngine;
using System.Collections;

namespace BlueMuffinGames.Tools.SettingsSystem
{
	public abstract class DropdownOptionProvider : DropdownOptionProvider<DropdownOption>
	{
		public abstract string OptionToString(DropdownOption option);
	}
}