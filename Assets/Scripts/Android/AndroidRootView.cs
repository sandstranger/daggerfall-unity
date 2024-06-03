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
        private Toggle _hideControlsToggle;

        private AndroidRootController _viewController = new AndroidRootController();

        private void Start()
        {
            _hideControlsToggle.isOn = ScreenControls.HideControls;

            _hideControlsToggle.onValueChanged.AddListener(hideControls =>
            {
                ScreenControls.HideControls = hideControls;
            });

            _startGameButton.onClick.AddListener(()=>
            {
                _viewController.StartGame();
            });

            _copyGameContentButton.onClick.AddListener(() =>
            {
                _viewController.CopyStreamingAssetsToInternalMemory();
            });

            _exitGameButton.onClick.AddListener(_viewController.QuitGame);
        }
    }
}