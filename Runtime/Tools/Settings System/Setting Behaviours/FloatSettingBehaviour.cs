using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    [CreateAssetMenu(fileName = "FloatSettingBehaviour", menuName = "Scriptable Objects/FloatSettingBehaviour")]
    public abstract class FloatSettingBehaviour : TypedSettingBehaviour<float>
    {
    }
}
