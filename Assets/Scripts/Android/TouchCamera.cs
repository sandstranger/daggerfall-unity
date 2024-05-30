using UnityEngine;
using UnityEngine.EventSystems;

namespace DaggerfallWorkshop.Game
{
    sealed class TouchCamera : EventTrigger
    {
        public static Vector2 CurrentTouchDelta { get; private set; }

        private const float Smoothness = 5.0f;

        public override void OnDrag(PointerEventData data)
        {
            if (data.pointerId > 0)
            {
                return;
            }

            CurrentTouchDelta = data.delta / Smoothness;
        }

        public override void OnPointerDown(PointerEventData data)
        {
            if (data.pointerId > 0)
            {
                return;
            }

            CurrentTouchDelta = data.delta / Smoothness;
        }

        public override void OnPointerUp(PointerEventData data)
        {
            if (data.pointerId <= 0)
            {
                CurrentTouchDelta = Vector2.zero;
            }
        }

        private void LateUpdate()
        {
            CurrentTouchDelta = Vector2.zero;
        }
    }
}