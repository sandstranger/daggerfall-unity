using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(Image))]
    sealed class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnClick;
        private bool _eventClicked;

        public void OnPointerDown(PointerEventData eventData)
        {
            _eventClicked = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _eventClicked = false;
        }

        private void Update()
        {
            if (_eventClicked)
            {
                OnClick?.Invoke();
            }
        }
    }
}