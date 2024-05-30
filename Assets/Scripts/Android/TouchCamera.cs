using UnityEngine;
using UnityEngine.EventSystems;

namespace DaggerfallWorkshop.Game
{
    sealed class TouchCamera : EventTrigger
    {
        public static Vector2 CurrentTouchPosition { get; private set; }

        private const float Smoothness = 5.0f;
        private const float TouchDeadZone = 0.1f;

        private Vector2 _previousTouchPosition;

        public override void OnDrag(PointerEventData data)
        {
            if (data.pointerId > 0)
            {
                return;
            }

            var delta = data.delta / Smoothness;

            if ( Mathf.Abs(_previousTouchPosition.x - delta.x) > TouchDeadZone ||
                 Mathf.Abs(_previousTouchPosition.y - delta.y) > TouchDeadZone )
            {
                CurrentTouchPosition = _previousTouchPosition = delta;
            }
        }

        public override void OnPointerDown(PointerEventData data)
        {
            if (data.pointerId > 0)
            {
                return;
            }

            CurrentTouchPosition = _previousTouchPosition = data.delta / Smoothness;
        }

        public override void OnPointerUp(PointerEventData data)
        {
            PointerEventData pointerEventData = (PointerEventData) data;

            if (pointerEventData.pointerId <= 0)
            {
                CurrentTouchPosition = _previousTouchPosition = Vector2.zero;
            }
        }

        private void LateUpdate()
        {
            CurrentTouchPosition = Vector2.zero;
        }
    }
}
