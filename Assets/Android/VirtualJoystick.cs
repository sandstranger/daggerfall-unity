using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public RectTransform background;
        public RectTransform knob;

        [Header("Input Settings")]
        public InputManager.AxisActions horizontalAxisAction = InputManager.AxisActions.MovementHorizontal;
        public InputManager.AxisActions verticalAxisAction = InputManager.AxisActions.MovementVertical;
        public Vector2 deadzone = Vector2.zero;

        private Vector2 inputVector;
        private Vector2 touchStartPos;
        private float joystickRadius;
        private bool isTouching = false;
        private Camera myCam;

        void Start()
        {
            myCam = GetComponentInParent<Canvas>().worldCamera;
            joystickRadius = background.sizeDelta.x / 2;

            // Initially invisible
            SetJoystickVisibility(false);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer down");
            if (Cursor.visible)
                return;
            touchStartPos = eventData.position;
            background.position = myCam.ScreenToWorldPoint(new Vector3(touchStartPos.x, touchStartPos.y, 1.05f));
            knob.position = touchStartPos;
            SetJoystickVisibility(true);
            isTouching = true;
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetJoystickVisibility(false);
            inputVector = Vector2.zero;
            UpdateVirtualAxes(Vector2.zero);
            isTouching = false;
        }
        private float lastDragTime = 0;
        public void OnDrag(PointerEventData eventData)
        {
            if (!isTouching)
                return;
            Debug.Log($"{(Time.time - lastDragTime)}\t{Time.deltaTime}");
            lastDragTime = Time.time;
            Vector2 direction = eventData.position - touchStartPos;
            inputVector = Vector2.ClampMagnitude(direction / joystickRadius, 1f);
            Vector2 knobPos2D = touchStartPos + inputVector * joystickRadius;
            knob.position = myCam.ScreenToWorldPoint(new Vector3(knobPos2D.x, knobPos2D.y, 1));
            if (Mathf.Abs(inputVector.x) < deadzone.x)
                inputVector.x = 0;
            if (Mathf.Abs(inputVector.y) < deadzone.y)
                inputVector.y = 0;
            UpdateVirtualAxes(inputVector);
        }

        private void UpdateVirtualAxes(Vector2 inputVec)
        {
            Debug.Log($"{horizontalAxisAction}: {inputVec.x}\t{verticalAxisAction}: {inputVec.y}");
            TouchscreenInputManager.SetAxis(horizontalAxisAction, inputVec.x);
            TouchscreenInputManager.SetAxis(verticalAxisAction, inputVec.y);
        }

        private void SetJoystickVisibility(bool isVisible)
        {
            background.gameObject.SetActive(isVisible);
            knob.gameObject.SetActive(isVisible);
        }
    }
}
