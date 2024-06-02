using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    sealed class ScreenControls : MonoBehaviour
    {
        private const string HideControlsKey = "hide_screen_controls";

        public static ScreenControls Instance;

        public bool HideControls
        {
            get => PlayerPrefsExtensions.GetBool(HideControlsKey, defaultValue: false);
            set => PlayerPrefsExtensions.SetBool(HideControlsKey, value);
        }

        public TouchCamera ButtonAttackSwing => _buttonAttackSwing;

        public TouchCamera TouchCamera => _touchCamera;

        [SerializeField]
        private GameObject _screenControlsRoot;

        [SerializeField]
        private TouchCamera _touchCamera;

        [SerializeField]
        private LongPressButton _jumpButton;

        [SerializeField]
        private LongPressButton _buttonAttack;
        [SerializeField]
        private TouchCamera _buttonAttackSwing;
        [SerializeField]
        private LongPressButton _buttonInventory;
        [SerializeField]
        private LongPressButton _buttonPrepareWeapon;
        [SerializeField]
        private LongPressButton _buttonUse;

        private void Awake()
        {
            SceneManager.sceneLoaded += (arg0, mode) =>
            {
                if (arg0.name == "DaggerfallUnityGame" && !HideControls)
                {
                    _screenControlsRoot.SetActive(true);
                }
            };

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _jumpButton.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.Jump);
            _buttonAttack.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.SwingWeapon);
            _buttonPrepareWeapon.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.ReadyWeapon);
            _buttonInventory.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.Inventory);
            _buttonUse.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.ActivateCenterObject);
        }
    }
}