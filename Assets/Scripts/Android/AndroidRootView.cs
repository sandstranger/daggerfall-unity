using System;
using UnityEngine;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    sealed class AndroidRootView : MonoBehaviour
    {
        [SerializeField]
        private Button _startGameButton;
        [SerializeField]
        private Button _copyGameContentButton;
        [SerializeField]
        private Button _exitGameButton;
        [SerializeField]
        private GameObject _progressBar;

        [SerializeField]
        private Toggle _hideControlsToggle;

        private AndroidRootController _viewController = new AndroidRootController();

        private void Start()
        {
            _hideControlsToggle.isOn = ScreenControls.Instance.HideControls;

            _hideControlsToggle.onValueChanged.AddListener(hideControls =>
            {
                ScreenControls.Instance.HideControls = hideControls;
            });

            _startGameButton.onClick.AddListener(async ()=>
            {
                _progressBar.SetActive(true);
                await _viewController.StartGame();
            });

            _copyGameContentButton.onClick.AddListener(async () =>
            {
                _progressBar.SetActive(true);
                await _viewController.CopyStreamingAssetsToInternalMemory();
                _progressBar.SetActive(false);
            });

            _exitGameButton.onClick.AddListener(_viewController.QuitGame);
        }
    }
}