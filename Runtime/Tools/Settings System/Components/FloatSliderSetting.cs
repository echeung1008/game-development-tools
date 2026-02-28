using UnityEngine;
using UnityEngine.UI;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    public sealed class FloatSliderSetting : TypedSetting<float>
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Vector2 _range;

        public override void Initialize()
        {
            base.Initialize();

            if (_slider == null)
            {
                Debug.LogError($"(FloatSliderSetting) No Slider is assigned on {name}.");
                return;
            }

            _slider.minValue = _range.x;
            _slider.maxValue = _range.y;
            _slider.wholeNumbers = false;

            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void SetVisualValue(float value)
        {
            if (_slider == null) return;

            _slider.value = value;
        }
    }
}
