using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    sealed class ScreenControls : MonoBehaviour
    {
        private const string HideControlsKey = "hide_screen_controls";

        public static ScreenControls Instance;

        public static bool HideControls
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
        [SerializeField]
        private LongPressButton _btnPause;
        [SerializeField]
        private GameObject _extraBtnsHolder;
        [SerializeField]
        private Button _extraBtnsToggle;
        [SerializeField]
        private LongPressButton _btnCastSpell;
        [SerializeField]
        private LongPressButton _btnSneak;
        [SerializeField]
        private LongPressButton _btnRun;
        [SerializeField]
        private LongPressButton _btnQuickSave;
        [SerializeField]
        private LongPressButton _btnQuickLoad;
        [SerializeField]
        private LongPressButton _btnRecastSpell;
        [SerializeField]
        private LongPressButton _btnCrouch;
        [SerializeField]
        private LongPressButton _btnStealMode;
        [SerializeField]
        private LongPressButton _btnGrabMode;
        [SerializeField]
        private LongPressButton _btnInfoMode;
        [SerializeField]
        private LongPressButton _btnTalkMode;
        [SerializeField]
        private LongPressButton _btnRest;
        [SerializeField]
        private LongPressButton _btnCharacterSheet;
        [SerializeField]
        private LongPressButton _btnChangeWeapon;
        [SerializeField]
        private LongPressButton _btnConsole;
        [SerializeField]
        private LongPressButton _btnLocalMap;
        [SerializeField]
        private LongPressButton _btnGlobalMap;
        [SerializeField]
        private LongPressButton _btnNotebook;
        [SerializeField]
        private LongPressButton _btnLogBook;
        [SerializeField]
        private LongPressButton _btnUseMagicItem;
        [SerializeField]
        private LongPressButton _btnUseTransport;

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
            _btnPause.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.Escape);
            _btnCastSpell.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.CastSpell);
            _btnSneak.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.Sneak);
            _btnRun.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.Run);
            _btnQuickSave.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.QuickSave);
            _btnQuickLoad.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.QuickLoad);
            _btnRecastSpell.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.RecastSpell);
            _btnCrouch.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.Crouch);
            _btnStealMode.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.StealMode);
            _btnTalkMode.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.TalkMode);
            _btnInfoMode.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.InfoMode);
            _btnGrabMode.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.GrabMode);
            _btnRest.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.Rest);
            _btnCharacterSheet.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.CharacterSheet);
            _btnChangeWeapon.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.SwitchHand);
            _btnLocalMap.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.AutoMap);
            _btnGlobalMap.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.TravelMap);
            _btnNotebook.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.NoteBook);
            _btnLogBook.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.LogBook);
            _btnUseMagicItem.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.UseMagicItem);
            _btnUseTransport.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.Transport);
            _btnConsole.OnClick += () => InputManager.Instance.AddAction(InputManager.Actions.ToggleConsole);

            _extraBtnsToggle.onClick.AddListener(()=> _extraBtnsHolder.SetActive(!_extraBtnsHolder.activeSelf));
        }
    }
}