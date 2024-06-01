using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using System.Linq;

namespace DaggerfallWorkshop.Game
{
    public class TouchscreenKeyboardManager : MonoBehaviour
    {
        public static TouchscreenKeyboardManager Instance { get; private set; }
        public static bool DidSubmit { get; private set; }
        
        [SerializeField] private TMPro.TMP_InputField dummyInputField;
        private TextBox currentTextbox;
        private HashSet<TextBox> registeredTextboxes = new HashSet<TextBox>();

        private void Awake()
        {
            if (Instance) {
                Debug.LogError("Extra instance of touchscreen keyboard manager singleton is present! Destroying self.");
                Destroy(this);
                return;
            }
    
            Instance = this;
        }
        private void Start()
        {
            GameObject inputFieldGO = new GameObject("Dummy Input Field");
            inputFieldGO.transform.parent = transform;
            dummyInputField.gameObject.SetActive(false);
            dummyInputField.onValueChanged.AddListener(OnDummyInputFieldChanged);
            dummyInputField.onSubmit.AddListener(OnDummyInputFieldSubmit);
            dummyInputField.onEndEdit.AddListener((string str) => ToggleKeyboardOff());
        }
        private bool IsTextboxVisible(TextBox textbox)
        {
            // checks if textbox is visible on the screen
            BaseScreenComponent cur = textbox;
            while(cur != null)
            {
                if (cur.Parent != null && DaggerfallUI.UIManager.TopWindow.ParentPanel == cur.Parent)
                    return true;
                cur = cur.Parent;
            }
            return false;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // check if mouse clicked within a valid textbox
                Vector2 mousePos = Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y;
                var activeTextboxes = registeredTextboxes.Where(p => IsTextboxVisible(p) && !p.ReadOnly);
                Debug.Log("Checking " + activeTextboxes.Count() + " textboxes if " + mousePos + " is contained within " + (activeTextboxes.Count() > 0 ? activeTextboxes.Take(1).Single().Rectangle.ToString() : "n/a"));
                TextBox textBox = activeTextboxes.Where(p => { Rect rect = p.Rectangle; rect.width = rect.width == 0 ? 1000 : rect.width; return rect.Contains(mousePos); }).Take(1).SingleOrDefault();
                if (textBox != default(TextBox))
                    // it did! Open the keyboard.
                    ToggleKeyboardOn(textBox);
            }
        }
        public void RegisterTextbox(TextBox textBox) => registeredTextboxes.Add(textBox);
        public void UnregisterTextbox(TextBox textBox) => registeredTextboxes.Remove(textBox);
        public void ToggleKeyboardOn(TextBox textBox)
        {
            Debug.Log("Opening android keyboard");
            this.currentTextbox = textBox;
            dummyInputField.text = textBox.Text;
            dummyInputField.gameObject.SetActive(true);
            dummyInputField.Select();
        }
        public void ToggleKeyboardOff()
        {
            currentTextbox = null;
            dummyInputField.text = "";
            dummyInputField.gameObject.SetActive(false);
        }
        private IEnumerator SubmitCoroutine()
        {
            // ensure everything sees 'didsubmit' for a single frame, regardless of script execution order
            yield return new WaitForEndOfFrame();
            DidSubmit = true;
            yield return new WaitForEndOfFrame();
            DidSubmit = false;
        }
        private void OnDummyInputFieldSubmit(string submittedVal)
        {
            StartCoroutine(SubmitCoroutine());
            ToggleKeyboardOff();
        }
        private void OnDummyInputFieldChanged(string newVal)
        {
            Debug.Log("dummy input changed: " + newVal);
            if(currentTextbox != null)
                currentTextbox.Text = newVal;
        }
    }
}