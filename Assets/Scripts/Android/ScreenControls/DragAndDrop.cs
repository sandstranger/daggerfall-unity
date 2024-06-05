using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(ButtonPositionHelper))]
    sealed class DragAndDrop : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [SerializeField]
        private ScreenControlsConfigurator controlsConfigurator;

        [SerializeField]
        private ButtonPositionHelper _buttonPositionHelper;

        public void OnDrag(PointerEventData eventData) =>
            _buttonPositionHelper.Position = Input.mousePosition;

        public void OnPointerDown(PointerEventData eventData) =>
            controlsConfigurator.CurrentButton = _buttonPositionHelper;
    }
}