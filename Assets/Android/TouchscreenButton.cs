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
        public TMPro.TMP_Text myLabel;
        private InputManager.Actions myLastAction;

        protected override void Start()
        {
            base.Start();

            myAction = GetSavedAction();
            myLastAction = myAction;
            myLabel.text = myAction.ToString();
        }
        private void Update()
        {
            if (!Application.isPlaying)
                return;

            if (myLastAction != myAction)
            {
                myLabel.text = myAction.ToString();
                myLastAction = myAction;
                SetSavedAction(myAction);
            }
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (TouchscreenInputManager.Instance.IsEditingControls())
                return;

            KeyCode myKey = InputManager.Instance.GetBinding(myAction);
            TouchscreenInputManager.SetKey(myKey, true);

        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (TouchscreenInputManager.Instance.IsEditingControls())
                TouchscreenInputManager.Instance.EditTouchscreenButton(this);

            KeyCode myKey = InputManager.Instance.GetBinding(myAction);
            TouchscreenInputManager.SetKey(myKey, false);
        }
        private InputManager.Actions GetSavedAction()
        {
            int savedActionInt = PlayerPrefs.GetInt("TouchscreenButtonAction_" + gameObject.name, -1);
            if (savedActionInt < 0)
                return myAction;
            return (InputManager.Actions)savedActionInt;
        }
        private void SetSavedAction(InputManager.Actions action)
        {
            PlayerPrefs.SetInt("TouchscreenButtonAction_" + gameObject.name, (int)action);
        }
    }
}