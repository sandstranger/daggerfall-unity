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

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)controlsConfigurator.Canvas.transform, eventData.position,
                controlsConfigurator.RenderCamera, out var position);

            _buttonPositionHelper.Position = controlsConfigurator.Canvas.transform.TransformPoint(position);
        }

        public void OnPointerDown(PointerEventData eventData) =>
            controlsConfigurator.CurrentButton = _buttonPositionHelper;
    }
}