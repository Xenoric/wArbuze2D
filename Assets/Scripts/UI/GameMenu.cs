using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Mirror;
using Warbuzz.Weapons;

namespace Warbuzz.UI
{
    public class GameMenu : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TMP_Text healthText;

        [Header("Shield Settings")]
        public Slider shieldSlider;
        [SerializeField] private TMP_Text shieldText;

        [Header("Energy Settings")]
        public Slider energySlider;
 
        [Header("Weapon Settings")]
      
        [SerializeField] private WeaponItem currentWeaponItem;
        [SerializeField] private WeaponItem secondaryWeaponItem;
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private TMP_Text currentBulletsText;
        [SerializeField] private TMP_Text currentAllBulletsText;
        [SerializeField] private TMP_Text secondaryBulletsText;
        [SerializeField] private TMP_Text secondaryAllBulletsText;

        [Header("Death Settings")]
        [SerializeField]private GameObject panelDeath;

        public void ChangeWeapon(Weapon current, Weapon secondary)
        {
            currentWeaponItem.EnableCurrentBullets(current.isUseBullets);
            currentWeaponItem.EnableAllBullets(current.isUseBullets);

            WeaponData.Data data = weaponData.GetWeaponData(current.weaponType);
            currentWeaponItem.imageWeapon.sprite = data.spriteWeapon;

            WeaponData.Data s_data = weaponData.GetWeaponData(secondary.weaponType);
            secondaryWeaponItem.imageWeapon.sprite = s_data.spriteWeapon;
            UpdateCurrentBullets(current.bullets, current.allBullets);
            UpdateSecondaryBullets(secondary.bullets, secondary.allBullets);
        }

        public void UpdateHealt(int val)
        {
            if (val < 0) val = 0;
            healthSlider.value = val;
            healthText.text = val + "/" + healthSlider.maxValue;
        }
        public void UpdateHealthMaxValue(int val)
        {
            healthSlider.maxValue = val;
        }
        public void UpdateShieldMaxValue(int val)
        {
            shieldSlider.maxValue = val;

        }
        public void UpdateShield(int val)
        {
            if (val < 0) val = 0;
            shieldSlider.value = val;
            shieldText.text = val + "/" + shieldSlider.maxValue;
        }
        public void UpdateEnergyMaxValue(int val)
        {
            energySlider.maxValue = val;

        }
        public void UpdateEnergy(int val)
        {
            if (val < 0) val = 0;
            energySlider.value = val;
          
        }

        public void EnablePanelDeath(bool en)
        {
            panelDeath.SetActive(en);
        }
        public void OnBtnPlayAgainClick()
        {
             GameManager.Instance.localPlayer?.health.PlayAgain();
        }
        public void UpdateCurrentBullets(string bullets, string allBullets)
        {
            currentBulletsText.text = bullets;
            currentAllBulletsText.text = allBullets;
        }
        public void UpdateCurrentBullets(int bullets, int allBullets)
        {
            currentBulletsText.text = bullets.ToString();
            currentAllBulletsText.text = allBullets.ToString();
        }
        public void UpdateSecondaryBullets(int bullets, int allBullets)
        {
            secondaryBulletsText.text = bullets.ToString();
            secondaryAllBulletsText.text = allBullets.ToString();
        }
        private void OnDestroy()
        {
          
        }
    }
}

