using UnityEngine;
using UnityEngine.UI;

namespace DaggerfallWorkshop.Game
{
    public class RadialProgress : MonoBehaviour
    {
        public Text ProgressIndicator;
        public Image LoadingBar;
        float currentValue;
        public float speed;

        private void Update()
        {
            if (currentValue < 100)
            {
                currentValue += speed * Time.deltaTime;
                ProgressIndicator.text = ((int)currentValue).ToString() + "%";
            }
            else
            {
                currentValue = 0;
            }

            LoadingBar.fillAmount = currentValue / 100;
        }
    }
}