using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Warbuzz.Weapons;
using TMPro;

namespace Warbuzz.UI
{
    public class WeaponItem : MonoBehaviour
    {
        [Header("Components")]
        public Image imageWeapon;
        [SerializeField] private TMP_Text textCurrentBullets;
        [SerializeField] private TMP_Text textAllBullets;

        private int current_Bullets;
        public int currentBullets 
        {
            get
            {
                return current_Bullets;
            }
            set
            {
                currentBullets = value;
                textCurrentBullets.text = currentBullets.ToString();
            }
        }

        private int all_Bullets;
        public int allBullets
        {
            get
            {
                return all_Bullets;
            }
            set
            {
                all_Bullets = value;
                textCurrentBullets.text = all_Bullets.ToString();
            }
        }
        public void EnableCurrentBullets(bool en)
        {
            textCurrentBullets.gameObject.SetActive(en);
          
        }
        public void EnableAllBullets(bool en)
        {
            textAllBullets.gameObject.SetActive(en);
        }

    }
}

