using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    sealed class ScreenControlsConfigurator : MonoBehaviour
    {
        public static event Action ResetToDefaults;

        public ButtonPositionHelper CurrentButton
        {
            get => _currentButton;
            set
            {
                _currentButton = value;

                if (value != null)
                {
                    _currentButtonNameText.text = value.ButtonId;
                }
            }
        }

        public Canvas Canvas => _canvas;
        public Camera RenderCamera => _renderCamera;

        private ButtonPositionHelper _currentButton;

        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private Camera _renderCamera;
        [SerializeField]
        private TMP_Text _currentButtonNameText;
        [SerializeField]
        private Button _alphaPlusBtn;
        [SerializeField]
        private Button _alphaMinusBtn;
        [SerializeField]
        private Button _sizePlusBtn;
        [SerializeField]
        private Button _sizeMinusBtn;
        [SerializeField]
        private Button _resetToDefaultsBtn;
        [SerializeField]
        private Button _backBtn;
        [SerializeField]
        private GameObject _rootPanel;

        [SerializeField]
        private float _changeSizeValue = 20.0f;
        [SerializeField]
        private float _changeAlphaValue = 0.1f;

        private void Awake()
        {
            _alphaPlusBtn.onClick.AddListener(OnAlphaPlusBtnClicked);
            _alphaMinusBtn.onClick.AddListener(OnAlphaMinusBtnClicked);
            _sizePlusBtn.onClick.AddListener(OnSizePlusBtnClicked);
            _sizeMinusBtn.onClick.AddListener(OnSizeMinusBtnClicked);
            _resetToDefaultsBtn.onClick.AddListener(() => ResetToDefaults?.Invoke());
            _backBtn.onClick.AddListener(Close);
        }

        public void Show()
        {
            _rootPanel.SetActive(true);
            _renderCamera.gameObject.SetActive(true);
            Input.multiTouchEnabled = false;
        }

        private void Close()
        {
            _rootPanel.SetActive(false);
            _renderCamera.gameObject.SetActive(false);
            Input.multiTouchEnabled = true;
            CurrentButton = null;
            _currentButtonNameText.text = string.Empty;
            PlayerPrefs.Save();
        }

        private void OnAlphaPlusBtnClicked()
        {
            if (CurrentButton == null)
            {
                return;
            }

            if ( !Mathf.Approximately(CurrentButton.Alpha,1.0f))
            {
                CurrentButton.Alpha += _changeAlphaValue;
            }
        }

        private void OnAlphaMinusBtnClicked()
        {
            if (CurrentButton == null)
            {
                return;
            }

            if ( !Mathf.Approximately(CurrentButton.Alpha,0.0f))
            {
                CurrentButton.Alpha -= _changeAlphaValue;
            }
        }

        private void OnSizePlusBtnClicked()
        {
            if (CurrentButton == null)
            {
                return;
            }

            CurrentButton.Size = new Vector2( CurrentButton.Size.x + _changeSizeValue, CurrentButton.Size.y + _changeSizeValue);
        }

        private void OnSizeMinusBtnClicked()
        {
            if (CurrentButton == null)
            {
                return;
            }

            CurrentButton.Size = new Vector2( CurrentButton.Size.x - _changeSizeValue, CurrentButton.Size.y - _changeSizeValue);
        }
    }
}