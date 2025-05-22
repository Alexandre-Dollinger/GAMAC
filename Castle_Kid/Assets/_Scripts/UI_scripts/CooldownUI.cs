using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Scripts.UI_scripts
{
    public class CooldownUI : MonoBehaviour
    {
        [SerializeField] private Image darkener;
        [SerializeField] private TMP_Text textCooldown;

        private void Awake()
        {
            textCooldown.enabled = false;
            darkener.fillAmount = 0.0f;
        }

        private void DisableCooldown()
        {
            textCooldown.enabled = false;
            darkener.fillAmount = 0.0f;
        }

        private void UpdateCooldown(float currentTime, float cooldown)
        {
            textCooldown.enabled = true;
            textCooldown.text = Mathf.RoundToInt(currentTime).ToString();
            darkener.fillAmount = currentTime / cooldown;
        }
        
        public void CooldownManagement(float currentTime, float cooldown)
        {
            if (currentTime <= 0)
                DisableCooldown();
            else
                UpdateCooldown(currentTime, cooldown);
        }
    }
}
