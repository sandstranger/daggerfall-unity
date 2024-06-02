using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace DaggerfallWorkshop.Game
{
    sealed class TouchCamera : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Vector2 CurrentTouchDelta { get; private set; }

        [SerializeField]
        private float _smoothness = 5.0f;
        [SerializeField]
        private bool _resetOnLateUpdate = true;

        private int _pointerId = -1;

        public void OnDrag(PointerEventData data)
        {
            if (data.pointerId == _pointerId)
            {
                CurrentTouchDelta = data.delta / _smoothness;
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (_pointerId >= 0)
            {
                return;
            }

            _pointerId = data.pointerId;
            CurrentTouchDelta = data.delta / _smoothness;
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (data.pointerId == _pointerId)
            {
                _pointerId = -1;
                CurrentTouchDelta = Vector2.zero;
            }
        }

        private void LateUpdate()
        {
            if (_resetOnLateUpdate)
            {
                CurrentTouchDelta = Vector2.zero;
            }
        }
    }
}