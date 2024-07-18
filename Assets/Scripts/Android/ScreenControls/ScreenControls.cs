using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wenzil.Console;

namespace DaggerfallWorkshop.Game
{
    sealed class ScreenControls : MonoBehaviour
    {
        private const string HideControlsKey = "hide_screen_controls";

        public static Vector2 JoystickMovement { get; set; } = Vector2.zero;

        public static ScreenControls Instance { get; private set; }

        public static bool HideControls
        {
            get => PlayerPrefsExtensions.GetBool(HideControlsKey, defaultValue: false);
            set => PlayerPrefsExtensions.SetBool(HideControlsKey, value);
        }

        public static bool EnterPressed { get; private set; }

        public TouchCamera ButtonAttackSwing => _buttonAttackSwing;

        public TouchCamera TouchCamera => _touchCamera;

        [SerializeField] private GameObject _screenControlsRoot;

        [SerializeField] private TouchCamera _touchCamera;
        [SerializeField] private TouchCamera _buttonAttackSwing;
        [SerializeField] private GameObject _extraBtnsHolder;
        [SerializeField] private Button _extraBtnsToggle;
        [SerializeField] private Button _btnConsole;
        [SerializeField] private Camera _renderCamera;
        [SerializeField] private ConsoleUI _console;
        [SerializeField] private Button _enterBtn;

        private static readonly Dictionary<KeyCode, bool> _keys = new Dictionary<KeyCode, bool>();
        private RenderTexture _renderTexture;
        private bool _hideControls = false;
        private bool _isPlayingGame = false;

        private void Awake()
        {
            _hideControls = HideControls;
            Instance = this;

            if(_renderCamera.targetTexture != null)
            {
                Destroy(_renderCamera.targetTexture);
                _renderCamera.targetTexture = null;
            }
            _renderCamera.aspect = Camera.main.aspect;
            _renderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            _renderTexture.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D24_UNorm_S8_UInt;
            _renderTexture.isPowerOfTwo = false;
            _renderCamera.targetTexture = _renderTexture;

            _btnConsole.onClick.AddListener(() => _console.ToggleConsole());
            _extraBtnsToggle.onClick.AddListener(() => _extraBtnsHolder.SetActive(!_extraBtnsHolder.activeSelf));
            _enterBtn.onClick.AddListener(() => EnterPressed = true);
        }

        public static void SetKey(KeyCode keyCode, bool value)
        {
            _keys[keyCode] = value;
        }

        public static bool GetKey(KeyCode keyCode)
        {
            return _keys.TryGetValue(keyCode, out var value) && value;
        }

        private void Update()
        {
            if (_hideControls)
            {
                return;
            }

            bool isPlayingGame = GameManager.Instance.IsPlayingGame();

            if (_isPlayingGame != isPlayingGame)
            {
                _screenControlsRoot.SetActive(isPlayingGame);
                _enterBtn.gameObject.SetActive(!isPlayingGame);
                _renderCamera.gameObject.SetActive(true);
                _isPlayingGame = isPlayingGame;
            }
        }

        private void OnGUI()
        {
            if (_hideControls)
            {
                return;
            }

            GUI.depth = 0;
            DaggerfallUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _renderTexture, ScaleMode.ScaleAndCrop, true);
        }

        private void LateUpdate()
        {
            EnterPressed = false;
        }
    }
}