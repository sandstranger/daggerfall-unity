using System;
using TMPro;
using UnityEngine;
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
            _maxFpsInputFIeld.onValueChanged.AddListener(_viewController.ChangeFps);
            _maxFpsInputFIeld.text = DaggerfallUnity.Settings.TargetFrameRate.ToString();

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