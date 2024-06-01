using UnityEngine;
using UnityEngine.EventSystems;

namespace DaggerfallWorkshop.Game
{
    sealed class TouchCamera : EventTrigger
    {
        public Vector2 CurrentTouchDelta { get; private set; }

        private const float Smoothness = 5.0f;
        private int _pointerId = -1;

        public override void OnDrag(PointerEventData data)
        {
            if (data.pointerId == _pointerId)
            {
                CurrentTouchDelta = data.delta / Smoothness;
            }
        }

        public override void OnPointerDown(PointerEventData data)
        {
            if (_pointerId >= 0)
            {
                return;
            }

            _pointerId = data.pointerId;
            CurrentTouchDelta = data.delta / Smoothness;
        }

        public override void OnPointerUp(PointerEventData data)
        {
            if (data.pointerId == _pointerId)
            {
                _pointerId = -1;
                CurrentTouchDelta = Vector2.zero;
            }
        }

        private void LateUpdate()
        {
            CurrentTouchDelta = Vector2.zero;
        }
    }
}