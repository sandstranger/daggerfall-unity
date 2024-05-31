using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


namespace DaggerfallWorkshop.Game
{
    public class TouchscreenInputManager : MonoBehaviour
    {
        #region monobehaviour
        [SerializeField] private Button toggleTouchscreenInputButton;
        [SerializeField] private Camera renderCamera;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private bool debugInEditor = false;

        private float startAlpha;
        private RenderTexture renderTex;
        
        private void Start()
        {
            startAlpha = canvasGroup.alpha;
            canvasGroup.alpha = _isTouchscreenInputActive ? startAlpha : 0;
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
            toggleTouchscreenInputButton.onClick.AddListener(ToggleTouchscreenInput);
        }
        private void OnGUI()
        {
            GUI.depth = 0;
            DaggerfallUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTex, ScaleMode.StretchToFill, true);
        }
        private void ToggleTouchscreenInput()
        {
            _isTouchscreenInputActive = !_isTouchscreenInputActive;
            canvasGroup.alpha = _isTouchscreenInputActive ? startAlpha : 0;
        }
        #endregion

        #region statics
        private static bool _isTouchscreenInputActive
        {
            get { return PlayerPrefs.GetInt("IsTouchscreenActive", 1) == 1; }
            set { PlayerPrefs.SetInt("IsTouchscreenActive", value ? 1 : 0); }
        }
        public static bool IsTouchscreenActive => _isTouchscreenInputActive && isMobilePlatform;
        public static bool IsTouchscreenTouched => _isTouchscreenInputActive && isMobilePlatform && (axes.Any(p => p.Value != 0) || keys.Any(p => p.Value));
        private static Dictionary<int, float> axes = new Dictionary<int, float>();
        private static Dictionary<int, bool> keys = new Dictionary<int, bool>();
        private static bool isMobilePlatform => Application.isMobilePlatform || Application.isEditor && _debugInEditor;
        private static bool _debugInEditor = false;

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