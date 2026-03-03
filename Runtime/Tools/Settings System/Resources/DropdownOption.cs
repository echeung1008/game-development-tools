using UnityEngine;
using System.Collections;

namespace BlueMuffinGames.Tools.SettingsSystem
{
	public struct DropdownOption
	{
		public readonly string name;
		public readonly object value;

		public DropdownOption(string name, object value)
		{
			this.name = name;
			this.value = value;
		}
	}
}