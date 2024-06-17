using UnityEngine;

namespace DaggerfallWorkshop.Game
{
    sealed class Joystick : MonoBehaviour
    {
        [SerializeField]
        private zFrame.UI.Joystick _joystick;
        [SerializeField]
        private RectTransform _joystickKnob;
        [SerializeField]
        private float _forceMultiplier = 1.5f;

        private void Awake()
        {
            var positionHelper = GetComponent<ButtonPositionHelper>();
            positionHelper.OnRepositionFinished += () => _joystick.maxRadius = _joystickKnob.rect.width;
        }

        private void Start()
        {
            _joystick.OnValueChanged.AddListener(v =>
            {
                if (v.magnitude != 0)
                {
                    var finalForce = v * _forceMultiplier;
                    InputManager.Instance.ApplyHorizontalForce(finalForce.x);
                    InputManager.Instance.ApplyVerticalForce(finalForce.y);
                }
            });
        }
    }
}