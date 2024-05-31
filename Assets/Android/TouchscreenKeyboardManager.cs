using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;

namespace DaggerfallWorkshop.Game
{
    public class TouchscreenKeyboardManager : MonoBehaviour
    {
        public static TouchscreenKeyboardManager Instance { get; private set; }
        public static bool DidSubmit { get; private set; }
        
        [SerializeField] private TMPro.TMP_InputField dummyInputField;
        private TextBox currentTextbox;

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
        public void ToggleKeyboardOn(TextBox textBox)
        {
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