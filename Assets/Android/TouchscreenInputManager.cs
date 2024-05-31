using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


namespace DaggerfallWorkshop.Game
{
    public class TouchscreenInputManager : MonoBehaviour
    {
        #region singleton
        public static TouchscreenInputManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance)
            {
                Debug.LogError("Two TouchscreenInputManager singletons are present!");
                Destroy(gameObject);
            }
            Instance = this;
        }


        #endregion

        #region monobehaviour
        [SerializeField] private Button editTouchscreenControlsButton;
        [SerializeField] private Canvas editControlsCanvas;
        [SerializeField] private TMPro.TMP_Dropdown editControlsDropdown;
        [SerializeField] private Camera renderCamera;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private bool debugInEditor = false;

        private float startAlpha;
        private RenderTexture renderTex;
        private TouchscreenButton currentlyEditingButton;
        
        private void Start()
        {
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
            editTouchscreenControlsButton.onClick.AddListener(ToggleEditTouchscreenControls);
        }
        private void Update()
        {
            _isInDaggerfallGUI = Time.timeScale == 0;
            canvasGroup.alpha = editControlsCanvas.enabled ? 1 : IsTouchscreenActive ? startAlpha : 0;
            canvasGroup.interactable = IsTouchscreenActive;
        }
        private void OnGUI()
        {
            if (IsTouchscreenActive)
            {
                GUI.depth = 0;
                DaggerfallUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTex, ScaleMode.StretchToFill, true);
            }
        }

        public bool IsEditingControls() => editControlsCanvas.enabled;

        public void EditTouchscreenButton(TouchscreenButton touchscreenButton)
        {
            if (currentlyEditingButton)
            {
                StopEditingCurrentButton();
                return;
            }
            editControlsDropdown.gameObject.SetActive(true);
            editControlsDropdown.value = (int)touchscreenButton.myAction;
            editControlsDropdown.Select();
            currentlyEditingButton = touchscreenButton;
        }
        private void StopEditingCurrentButton()
        {
            editControlsDropdown.gameObject.SetActive(false);
            currentlyEditingButton = null;
        }
        private void OnEditControlsDropdownValueChanged(int newVal)
        {
            if (currentlyEditingButton)
            {
                currentlyEditingButton.myAction = (InputManager.Actions)newVal;
                currentlyEditingButton = null;
                editControlsDropdown.gameObject.SetActive(false);
            }
        }
        private void ToggleEditTouchscreenControls()
        {
            editControlsCanvas.enabled = !editControlsCanvas.enabled;
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