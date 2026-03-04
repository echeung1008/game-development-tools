using UnityEngine;
using System.Collections;

namespace BlueMuffinGames.Tools.SettingsSystem
{
	public struct DropdownOption
	{
		public readonly string id;
		public readonly object value;

		public DropdownOption(string id, object value)
		{
			this.id = id;
			this.value = value;
		}

		public override string ToString() => $"DropdownOption (ID: {id} | Value: {value})";
	}
}