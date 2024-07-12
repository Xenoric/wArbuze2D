using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Warbuzz.Weapons
{
   
    [CreateAssetMenu(fileName = "WeaponData", menuName = "SO/WeaponData", order = 1)]
    
    public class WeaponData : ScriptableObject
    {
        [System.Serializable]
        public class Data
        {
            public Weapon.WeaponType weaponType;
            public Sprite spriteWeapon;
        }


        public List<Data> uIWeaponDatas;
        public Data GetWeaponData(Weapon.WeaponType weaponType)
        {
            foreach (Data uIWeaponData in uIWeaponDatas)
            {
                if (uIWeaponData.weaponType == weaponType)
                    return uIWeaponData;
            }

            return null;
        }
    }

  

}

