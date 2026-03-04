using BlueMuffinGames.Tools.MenuController;
using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public class BaseSettingsMenu : MenuPage
    {
        public override void Initialize()
        {
            base.Initialize();

            foreach (var setting in GetComponentsInChildren<BaseSetting>())
            {
                setting.Initialize();
            }
        }
    }
}
