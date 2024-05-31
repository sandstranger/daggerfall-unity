using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaggerfallWorkshop.Game
{
    sealed class ScreenControls : MonoBehaviour
    {
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
                if (arg0.name == "DaggerfallUnityGame")
                {
                    _screenControlsRoot.SetActive(true);
                }
            };

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}