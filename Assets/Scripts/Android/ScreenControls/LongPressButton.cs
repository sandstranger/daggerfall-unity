using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    [RequireComponent(typeof(Image))]
    sealed class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private InputManager.Actions _keyEvent;

        public void OnPointerDown(PointerEventData eventData)
        {
            ScreenControls.SetKey(InputManager.Instance.GetBinding(_keyEvent), true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ScreenControls.SetKey(InputManager.Instance.GetBinding(_keyEvent), false);
        }
    }
}