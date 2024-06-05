// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2024 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Vincent Wing (vwing@uci.edu)
// Contributors:
// 
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    public class TouchscreenInputManager : MonoBehaviour
    {
        public static TouchscreenInputManager Instance { get; private set; }

        #region monobehaviour
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Canvas editControlsCanvas;
        [SerializeField] private TMPro.TMP_Dropdown editControlsDropdown;
        [SerializeField] private Canvas joystickCanvas;
        [SerializeField] private Canvas buttonsCanvas;
        [SerializeField] private UnityUIPopup confirmChangePopup;
        [SerializeField] private Camera renderCamera;
        [SerializeField] private TouchscreenButton editTouchscreenControlsButton;
        [SerializeField] private TouchscreenButton resetButtonTransformsButton;
        [SerializeField] private TouchscreenButton resetButtonMappingsButton;
        [SerializeField] private Button editControlsBackgroundButton;
        [SerializeField] private Toggle showLabelsToggle;
        [SerializeField] private bool debugInEditor = false;


        public Camera RenderCamera { get { return renderCamera; } }
        public bool IsEditingControls { get { return editControlsCanvas.enabled; } }
        public bool ShouldShowLabels { get { return IsEditingControls && showLabelsToggle.isOn; } }
        public TouchscreenButton CurrentlyEditingButton { get { return currentlyEditingButton; } }

        public event System.Action<bool> onEditControlsToggled;
        public event System.Action<TouchscreenButton> onCurrentlyEditingButtonChanged;
        public event System.Action onResetButtonActionsToDefaultValues;
        public event System.Action onResetButtonTransformsToDefaultValues;

        private float startAlpha;
        private RenderTexture renderTex;
        private TouchscreenButton currentlyEditingButton;

        private void Awake()
        {
            #region singleton
            if (Instance)
            {
                Debug.LogError("Two TouchscreenInputManager singletons are present!");
                Destroy(gameObject);
            }
            Instance = this;

            #endregion

            Setup();
        }

        private void Setup()
        {
            editControlsCanvas.enabled = false;

            startAlpha = canvasGroup.alpha;
            canvasGroup.alpha = IsTouchscreenInputEnabled ? startAlpha : 0;
            renderCamera.aspect = Camera.main.aspect;
            renderTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            renderTex.isPowerOfTwo = false;
            renderCamera.targetTexture = renderTex;

            _debugInEditor = debugInEditor;
            if (!isMobilePlatform)
            {
                Debug.Log("Disabling touchscreen input manager");
                gameObject.SetActive(false);
            }

            // edit controls canvas setup
            editControlsDropdown.ClearOptions();
            List<string> options = new List<string>();
            for (int i = 0; i <= (int)InputManager.Actions.Unknown; ++i)
                options.Add(((InputManager.Actions)i).ToString());
            editControlsDropdown.AddOptions(options);
            editControlsDropdown.gameObject.SetActive(false);
            editControlsDropdown.onValueChanged.AddListener(OnEditControlsDropdownValueChanged);
            editControlsBackgroundButton.gameObject.SetActive(false);

            editTouchscreenControlsButton.onClick.AddListener(OnEditTouchscreenControlsButtonClicked);
            resetButtonMappingsButton.onClick.AddListener(OnResetButtonMappingsButtonClicked);
            resetButtonTransformsButton.onClick.AddListener(OnResetButtonTransformsButtonClicked);
            editControlsBackgroundButton.onClick.AddListener(OnEditControlsBackgroundClicked);
        }
        private void Update()
        {
            _isInDaggerfallGUI = !IsEditingControls && GameManager.IsGamePaused;
            canvasGroup.alpha = editControlsCanvas.enabled ? 1 : startAlpha;
            canvas.enabled = IsTouchscreenActive;
            buttonsCanvas.enabled = IsTouchscreenActive;
            joystickCanvas.enabled = !IsEditingControls && IsTouchscreenActive;

        }
        private void OnGUI()
        {
            if (IsTouchscreenActive)
            {
                GUI.depth = 0;
                DaggerfallUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTex, ScaleMode.ScaleAndCrop, true);
            }
        }

        public void EditTouchscreenButton(TouchscreenButton touchscreenButton)
        {
            if(touchscreenButton == null)
            {
                StopEditingCurrentButton();
                return;
            }
            if (touchscreenButton == currentlyEditingButton)
                return;

            if (currentlyEditingButton)
            {
                StopEditingCurrentButton();
            }
            editControlsDropdown.gameObject.SetActive(true);
            editControlsDropdown.value = (int)touchscreenButton.myAction;
            currentlyEditingButton = touchscreenButton;
            onCurrentlyEditingButtonChanged?.Invoke(touchscreenButton);
        }
        private void StopEditingCurrentButton()
        {
            editControlsDropdown.gameObject.SetActive(false);
            currentlyEditingButton = null;
            onCurrentlyEditingButtonChanged?.Invoke(null);
        }
        private void OnEditControlsDropdownValueChanged(int newVal)
        {
            if (currentlyEditingButton)
            {
                currentlyEditingButton.myAction = (InputManager.Actions)newVal;
            }
        }

        private void OnEditControlsBackgroundClicked()
        {
            if (currentlyEditingButton != null)
                StopEditingCurrentButton();
        }
        private void OnResetButtonTransformsButtonClicked()
        {
            if (!resetButtonTransformsButton.WasDragging)
                confirmChangePopup.Open("Do you want to reset the button positions, sizes, and enabled statuses to their default values?", onResetButtonTransformsToDefaultValues);
        }
        private void OnResetButtonMappingsButtonClicked()
        {
            if (!resetButtonMappingsButton.WasDragging)
                confirmChangePopup.Open("Do you want to reset the button action mappings to their default values?", onResetButtonActionsToDefaultValues);
        }
        private void OnEditTouchscreenControlsButtonClicked()
        {
            if (!editTouchscreenControlsButton.WasDragging)
            {
                editControlsCanvas.enabled = !editControlsCanvas.enabled;
                editControlsBackgroundButton.gameObject.SetActive(editControlsCanvas.enabled);
                GameManager.Instance.PauseGame(editControlsCanvas.enabled, true);
                onEditControlsToggled?.Invoke(editControlsCanvas.enabled);
            }
        }
        #endregion

        #region statics
        public static bool IsTouchscreenInputEnabled
        {
            get { return PlayerPrefs.GetInt("IsTouchscreenInputEnabled", 1) == 1; }
            set { PlayerPrefs.SetInt("IsTouchscreenInputEnabled", value ? 1 : 0); }
        }
        public static bool IsTouchscreenActive => IsTouchscreenInputEnabled && isMobilePlatform && !_isInDaggerfallGUI;
        public static bool IsTouchscreenTouched => IsTouchscreenActive && (axes.Any(p => p.Value != 0) || keys.Any(p => p.Value));

        private static Dictionary<int, float> axes = new Dictionary<int, float>();
        private static Dictionary<int, bool> keys = new Dictionary<int, bool>();
        private static bool isMobilePlatform => Application.isMobilePlatform || Application.isEditor && _debugInEditor;
        private static bool _debugInEditor = false;
        private static bool _isInDaggerfallGUI = false;

        public static void SetAxis(InputManager.AxisActions action, float value) => axes[(int)action] = value;
        public static float GetAxis(InputManager.AxisActions action)
        {
            if (!isMobilePlatform)
                return 0;
            return axes.ContainsKey((int)action) ? axes[(int)action] : 0;
        }
        public static void SetKey(KeyCode k, bool value) => keys[(int)k] = value;
        public static bool GetKey(KeyCode k)
        {
            if (!isMobilePlatform)
                return false;
            return keys.ContainsKey((int)k) && keys[(int)k];
        }

        public static bool GetPollKey(KeyCode k)
        {
            if (!isMobilePlatform)
                return false;
            if ((int)k < InputManager.startingAxisKeyCode)
                return GetKey(k);
            else
                return false;
        }
        #endregion
    }
}