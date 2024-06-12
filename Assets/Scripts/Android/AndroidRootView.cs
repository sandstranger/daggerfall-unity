using TMPro;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    sealed class AndroidRootView : MonoBehaviour
    {
        [SerializeField]
        private Button _startGameButton;
        [SerializeField]
        private Button _screenControlsBtn;
        [SerializeField]
        private Button _copyGameContentButton;
        [SerializeField]
        private Button _exitGameButton;
        [SerializeField]
        private Toggle _hideControlsToggle;
        [SerializeField]
        private ScreenControlsConfigurator _controlsConfigurator;
        [SerializeField]
        private TMP_InputField _maxFpsInputFIeld;

        private AndroidRootController _viewController = new AndroidRootController();

        private void Start()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            DaggerfallUnityApplication.CreateAndroidGameFolder();
            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
#endif
            _maxFpsInputFIeld.text = DaggerfallUnity.Settings.TargetFrameRate.ToString();
            _maxFpsInputFIeld.onValueChanged.AddListener(_viewController.ChangeFps);
#if UNITY_ANDROID && !UNITY_EDITOR
            }
#endif
            _hideControlsToggle.isOn = ScreenControls.HideControls;

            _hideControlsToggle.onValueChanged.AddListener(hideControls =>
            {
                ScreenControls.HideControls = hideControls;
            });

            _startGameButton.onClick.AddListener(()=>
            {
                _viewController.StartGame();
            });

            _screenControlsBtn.onClick.AddListener(() => _controlsConfigurator.Show());

            _copyGameContentButton.onClick.AddListener(() =>
            {
                _viewController.CopyStreamingAssetsToInternalMemory();
            });

            _exitGameButton.onClick.AddListener(_viewController.QuitGame);
        }
    }
}