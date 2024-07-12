using UnityEngine;
using Mirror;
using Warbuzz.Weapons;
using Warbuzz.UI;
using UnityEngine.SceneManagement;
using Warbuzz.Network;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Warbuzz.Player
{
    public class WeaponController : NetworkBehaviour
    {
        [SerializeField] private PlayerController playerController;
  
        //[SerializeField] private WeaponData weaponData;
        [SerializeField] private GameObject bulletPref;
        public List<Weapon> weapons;
        public Weapon knifeWeapon;
        public Weapon firstWeapon;
        public Weapon secondaryWeapon;
        public Weapon currentWeapon;

        private GameMenu gameMenu;
        private float _nextFire;
        [Header("Pool")]
        public PrefabPool prefabPool;


        public void StartClient()
        {
            if (isLocalPlayer)
            {
                gameMenu = GameManager.Instance.uiManager.gameMenu;
            }
        }
        public void StartServer()
        {
            // InitPool();
            
        }

        [Server]
        public void InitPool()
        {
           
            foreach (GameObject go in gameObject.scene.GetRootGameObjects())
            {
                if (go.name == "PrefabPool")
                {
                    prefabPool = go.GetComponent<PrefabPool>();
                   
                }
            }
           
        }
        [Client]
        public void CheckChangeWeapon()
        {

            if (Input.GetKeyDown(playerController.input.keyBtnWeapon1))
            {
                CmdChangeWeapon(firstWeapon.weaponType);
            }
            if (Input.GetKeyDown(playerController.input.keyBtnWeapon2))
            {
                CmdChangeWeapon(secondaryWeapon.weaponType);
            }
            if (Input.GetKeyDown(playerController.input.keyBtnWeapon3))
            {
                CmdChangeWeapon(knifeWeapon.weaponType);
            }
          
        }
        [Command]
        private void CmdChangeWeapon(Weapon.WeaponType weaponType)
        {
            currentWeapon = GetWeapon(weaponType);
            RpcChangeWeapon(weaponType);
            
        }
        [ClientRpc]
        private void RpcChangeWeapon(Weapon.WeaponType weaponType)
        {
            DisableWeapons();
            currentWeapon = GetWeapon(weaponType);
            currentWeapon.gameObject.SetActive(true);
            
            if (isLocalPlayer)
            {
                if(currentWeapon.weaponType == firstWeapon.weaponType)
                {
                    gameMenu.ChangeWeapon(firstWeapon, secondaryWeapon);
                }
                if (currentWeapon.weaponType == secondaryWeapon.weaponType)
                {
                    gameMenu.ChangeWeapon(secondaryWeapon, firstWeapon);
                }
                if (currentWeapon.weaponType == knifeWeapon.weaponType)
                {
                    gameMenu.ChangeWeapon(knifeWeapon, firstWeapon);
                }
            }


        }
        private void DisableWeapons()
        {
            foreach (Weapon weapon in weapons)
            {
                weapon.gameObject.SetActive(false);
            }
        }
        [Client]
        public void UpdateWeaponRotation(Vector3 aimPos)
        {

           currentWeapon.CmdUpdateWeaponRotation(aimPos);
        }
      
        [Client]
        public void CanShootCheck()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!Input.GetButton("Fire1") || !(Time.time > _nextFire))
                return;

            _nextFire = Time.time + 1f / currentWeapon.fireRate;

            currentWeapon.Fire();
        }

        public Weapon GetWeapon(Weapon.WeaponType weaponType)
        {
            foreach (Weapon weapon in weapons)
                if (weapon.weaponType == weaponType)
                    return weapon;
            return null;
        }
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying && playerController && isLocalPlayer)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(currentWeapon.firePoint.position, playerController.input.aimTransform.position);
            }
            
        }
    }
}

