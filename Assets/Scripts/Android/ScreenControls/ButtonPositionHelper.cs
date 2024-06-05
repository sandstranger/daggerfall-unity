using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PlayerPrefs = DaggerfallWorkshop.Game.PlayerPrefsExtensions;

namespace DaggerfallWorkshop.Game
{
    sealed class ButtonPositionHelper : MonoBehaviour
    {
        private string _buttonId;

        private Image _buttonImage;
        private Image[] _images;
        private TMP_Text _text;
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
            get => _buttonImage.color.a;

            set
            {
                foreach (var image in _images)
                {
                    var imageColor = image.color;
                    image.color = new Color(imageColor.r, imageColor.g, imageColor.b, value);
                }

                if (_text != null)
                {
                    Color textColor = _text.color;
                    _text.color = new Color(textColor.r, textColor.g, textColor.b, value);
                }

                PlayerPrefs.SetFloat(_alphaKey, value);
            }
        }

        private void Awake()
        {
            ScreenControlsConfigurator.ResetToDefaults += Reset;

            _buttonId = gameObject.name;
            _defaultPositionKey = $"{_buttonId}_default_position";
            _defaultSizeKey = $"{_buttonId}_default_size";
            _defaultAlphaKey = $"{_buttonId}_default_alpha";

            _positionKey = $"{_buttonId}_position";
            _sizeKey = $"{_buttonId}_size";
            _alphaKey = $"{_buttonId}_alpha";

            _buttonImage = GetComponent<Image>() ?? GetComponentInChildren<Image>();
            _images = GetComponentsInChildren<Image>() ?? Array.Empty<Image>();

            _text = GetComponentInChildren<TMP_Text>();
            _rectTransform = GetComponent<RectTransform>();

            SaveDefaultValues();

            _rectTransform.position = PlayerPrefs.GetVector3(_positionKey, _rectTransform.position);
            _rectTransform.sizeDelta = PlayerPrefs.GetVector2(_sizeKey, _rectTransform.sizeDelta);

            Color color = _buttonImage.color;

            float alpha = PlayerPrefs.GetFloat(_alphaKey, color.a);
            _buttonImage.color = new Color(color.r, color.g, color.b,alpha);

            foreach (var image in _images)
            {
                var imageColor = image.color;
                image.color = new Color(imageColor.r, imageColor.g, imageColor.b, alpha);
            }

            if (_text != null)
            {
                Color textColor = _text.color;
                _text.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            }
        }

        public void Reset()
        {
            _rectTransform.position = PlayerPrefs.GetVector3(_defaultPositionKey, _rectTransform.position);
            _rectTransform.sizeDelta = PlayerPrefs.GetVector2(_defaultSizeKey, _rectTransform.sizeDelta);

            Color color = _buttonImage.color;

            float alpha = PlayerPrefs.GetFloat(_defaultAlphaKey, color.a);
            _buttonImage.color = new Color(color.r, color.g, color.b,alpha);

            foreach (var image in _images)
            {
                var imageColor = image.color;
                image.color = new Color(imageColor.r, imageColor.g, imageColor.b, alpha);
            }

            if (_text != null)
            {
                Color textColor = _text.color;
                _text.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            }

            PlayerPrefs.DeleteKey(_alphaKey);
            PlayerPrefs.DeleteVector2Key(_sizeKey);
            PlayerPrefs.DeleteVector3Key(_positionKey);
            PlayerPrefs.Save();
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
                PlayerPrefs.SetFloat(_defaultAlphaKey, _buttonImage.color.a);
            }
        }
    }
}