using DaggerfallWorkshop.Game.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DaggerfallWorkshop.Game
{
    sealed class TouchCamera : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private const int DefaultPointerId = -1;

        public Vector2 CurrentTouchDelta { get; private set; }

        [SerializeField]
        private float _smoothness = 5.0f;
        [SerializeField]
        private bool _resetOnLateUpdate = true;

        private int _pointerId = DefaultPointerId;

        private void Start()
        {
            SaveLoadManager.OnLoad += _ => _pointerId = DefaultPointerId;
        }

        public void OnDrag(PointerEventData data)
        {
            if (_pointerId == DefaultPointerId)
            {
                _pointerId = data.pointerId;
            }

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
                _pointerId = DefaultPointerId;
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