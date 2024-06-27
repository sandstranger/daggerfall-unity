// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2024 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Vincent Wing (vwing@uci.edu)
// Contributors:
// 
// Notes:
//

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
        public bool isInMouseLookMode = false;

        public PointerEventData CurrentPointerEventData {get; private set;}
        public Vector2 TouchStartPos{get; private set;}
        public static VirtualJoystick JoystickThatIsCurrentlyMouseLooking{get; private set;} = null;

        private Vector2 inputVector;
        private float joystickRadius;
        private bool isTouching = false;
        private Camera myCam;
        private RectTransform rootRectTF;

        void Start()
        {
            // get refs
            myCam = GetComponentInParent<Canvas>().worldCamera;
            rootRectTF = transform as RectTransform;
            while (rootRectTF.parent is RectTransform)
                rootRectTF = rootRectTF.parent as RectTransform;

            // set size to half of the screen area
            (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rootRectTF.rect.width / 2f);

            // set vars
            Rect joystickRect = UnityUIUtils.GetScreenspaceRect(background, myCam);
            Rect knobRect = UnityUIUtils.GetScreenspaceRect(knob, myCam);
            joystickRadius = joystickRect.width / 2f - knobRect.width/2f;

            // Initially invisible
            SetJoystickVisibility(false);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (Cursor.visible)
                return;
            CurrentPointerEventData = eventData;
            TouchStartPos = eventData.position;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background.parent as RectTransform, TouchStartPos, myCam, out Vector2 backgroundPos))
            {
                background.localPosition = backgroundPos;
            }
            knob.position = background.position;
            SetJoystickVisibility(true);
            isTouching = true;
            OnDrag(eventData);
            if (isInMouseLookMode && !JoystickThatIsCurrentlyMouseLooking)
                JoystickThatIsCurrentlyMouseLooking = this;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetJoystickVisibility(false);
            inputVector = Vector2.zero;
            UpdateVirtualAxes(Vector2.zero);
            isTouching = false;
            if (JoystickThatIsCurrentlyMouseLooking == this)
                JoystickThatIsCurrentlyMouseLooking = null;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isTouching)
                return;

            Vector2 direction = eventData.position - TouchStartPos;
            inputVector = Vector2.ClampMagnitude(direction / joystickRadius, 1f);
            Vector2 knobPosScreenSpace = TouchStartPos + inputVector * joystickRadius;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(knob.parent as RectTransform, knobPosScreenSpace, myCam, out Vector2 knobPos))
                knob.localPosition = knobPos;
            if (Mathf.Abs(inputVector.x) < deadzone.x)
                inputVector.x = 0;
            if (Mathf.Abs(inputVector.y) < deadzone.y)
                inputVector.y = 0;
            UpdateVirtualAxes(inputVector);
        }

        private void UpdateVirtualAxes(Vector2 inputVec)
        {
            if (isInMouseLookMode)
                return;

            TouchscreenInputManager.SetAxis(horizontalAxisAction, inputVec.x);
            TouchscreenInputManager.SetAxis(verticalAxisAction, inputVec.y);
        }

        private void SetJoystickVisibility(bool isVisible)
        {
            background.gameObject.SetActive(isVisible && !isInMouseLookMode);
            knob.gameObject.SetActive(isVisible && !isInMouseLookMode);
        }
    }
}
