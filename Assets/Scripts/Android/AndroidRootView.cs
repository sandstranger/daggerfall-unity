using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    sealed class AndroidRootView : MonoBehaviour
    {
        private const string MaxFpsKey = "max_fps";

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
        private int _maxFps = 120;
        [SerializeField]
        private TMP_InputField _maxFpsInputFIeld;

        private AndroidRootController _viewController = new AndroidRootController();

        private void Start()
        {
            _maxFpsInputFIeld.onValueChanged.AddListener(value =>
            {
                if (int.TryParse(value, out var maxFps))
                {
                    Application.targetFrameRate = maxFps;
                    PlayerPrefs.SetInt(MaxFpsKey, maxFps);
                }
            });

            _maxFpsInputFIeld.text = PlayerPrefs.GetInt(MaxFpsKey, _maxFps).ToString();

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