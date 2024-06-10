using UnityEngine;
using PlayerPrefs = DaggerfallWorkshop.Game.PlayerPrefsExtensions;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(CanvasGroup))]
    sealed class ButtonPositionHelper : MonoBehaviour
    {
        private string _buttonId;

        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        private string _positionKey;
        private string _sizeKey;
        private string _alphaKey;

        private string _defaultPositionKey;
        private string _defaultSizeKey;
        private string _defaultAlphaKey;

        public string ButtonId => _buttonId;

        public Vector3 Position
        {
            get => _rectTransform.position;

            set
            {
                _rectTransform.position = value;
                PlayerPrefs.SetVector3(_positionKey,value);
            }
        }

        public Vector2 Size
        {
            get => _rectTransform.sizeDelta;
            set
            {
                _rectTransform.sizeDelta = value;
                PlayerPrefs.SetVector2(_sizeKey, value);
            }
        }

        public float Alpha
        {
            get => _canvasGroup.alpha;

            set
            {
                _canvasGroup.alpha = value;
                PlayerPrefs.SetFloat(_alphaKey, value);
            }
        }

        private void Start()
        {
            ScreenControlsConfigurator.ResetToDefaults += Reset;

            _buttonId = gameObject.name;
            _defaultPositionKey = $"{_buttonId}_default_position";
            _defaultSizeKey = $"{_buttonId}_default_size";
            _defaultAlphaKey = $"{_buttonId}_default_alpha";

            _positionKey = $"{_buttonId}_position";
            _sizeKey = $"{_buttonId}_size";
            _alphaKey = $"{_buttonId}_alpha";

            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();

            SaveDefaultValues();

            _rectTransform.position = PlayerPrefs.GetVector3(_positionKey, _rectTransform.position);
            _rectTransform.sizeDelta = PlayerPrefs.GetVector2(_sizeKey, _rectTransform.sizeDelta);

            float alpha = PlayerPrefs.GetFloat(_alphaKey, _canvasGroup.alpha);
            _canvasGroup.alpha = alpha;
        }

        public void Reset()
        {
            _rectTransform.position = PlayerPrefs.GetVector3(_defaultPositionKey, _rectTransform.position);

            _rectTransform.sizeDelta = PlayerPrefs.GetVector2(_defaultSizeKey, _rectTransform.sizeDelta);

            float alpha = PlayerPrefs.GetFloat(_defaultAlphaKey, _canvasGroup.alpha);
            _canvasGroup.alpha = alpha;

            PlayerPrefs.DeleteKey(_alphaKey);
            PlayerPrefs.DeleteVector2Key(_sizeKey);
            PlayerPrefs.DeleteVector3Key(_positionKey);
        }

        private void SaveDefaultValues()
        {
            if (!PlayerPrefs.HasKey(_defaultPositionKey))
            {
                PlayerPrefs.SetVector3(_defaultPositionKey,_rectTransform.position);
            }

            if (!PlayerPrefs.HasKey(_defaultSizeKey))
            {
                PlayerPrefs.SetVector2(_defaultSizeKey, _rectTransform.sizeDelta);
            }

            if (!PlayerPrefs.HasKey(_defaultAlphaKey))
            {
                PlayerPrefs.SetFloat(_defaultAlphaKey, _canvasGroup.alpha);
            }
        }
    }
}