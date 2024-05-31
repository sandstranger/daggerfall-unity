using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaggerfallWorkshop.Game
{
    sealed class ScreenControls : MonoBehaviour
    {
        private const string HideControlsKey = "hide_screen_controls";

        public bool HideControls
        {
            get => PlayerPrefsExtensions.GetBool(HideControlsKey, defaultValue: false);
            set => PlayerPrefsExtensions.SetBool(HideControlsKey, value);
        }

        public static ScreenControls Instance;

        public TouchCamera TouchCamera => _touchCamera;

        [SerializeField]
        private GameObject _screenControlsRoot;

        [SerializeField]
        private TouchCamera _touchCamera;

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
        }
    }
}