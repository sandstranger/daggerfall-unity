using UnityEngine;
using UnityEngine.EventSystems;

namespace DaggerfallWorkshop.Game
{
    sealed class DragAndDrop : MonoBehaviour, IDragHandler
    {
        private RectTransform _rectTransform;

        private void Awake() => _rectTransform = GetComponent<RectTransform>();

        public void OnDrag(PointerEventData eventData) => _rectTransform.position = Input.mousePosition;
    }
}