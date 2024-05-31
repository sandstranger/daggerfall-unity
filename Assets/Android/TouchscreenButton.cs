using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    public class TouchscreenButton : Button
    {
        public InputManager.Actions myAction = InputManager.Actions.Unknown;
        //public KeyCode myKey = KeyCode.None;
        public override void OnPointerDown(PointerEventData eventData)
        {
            KeyCode myKey = InputManager.Instance.GetBinding(myAction);
            TouchscreenInputManager.SetKey(myKey, true);

        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            KeyCode myKey = InputManager.Instance.GetBinding(myAction);
            TouchscreenInputManager.SetKey(myKey, false);
        }
    }
}