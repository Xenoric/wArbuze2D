using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Warbuzz.Player;

namespace Warbuzz.Weapons
{
    [System.Serializable]
    public class Weapon : NetworkBehaviour
    {
        public enum WeaponType
        {
            Assault, Sniper, Knife
        }

        public WeaponType weaponType;

        [Header("Parameters")]
        [SyncVar] public int damage;
        [SyncVar] public float fireRate;
        [SyncVar] public float fireRange;
        public int bullets = 100;
        [SyncVar] public int bulletsMax = 100;
        public int allBullets = 1000;
        [SyncVar] public bool isUseBullets = true;
        public int fireRandomRange;
        public Vector3 offsetWeaponRotate = new Vector3(0, -4, 0);
        [Header("Controllers")]
        public Animator animator;
        public PlayerController playerController;

        [Header("Components")]
        [SerializeField]protected List<GameObject> weaponVisibleObjects;
        public Transform center;
        public Transform firePoint;

        public void Show()
        {
            foreach(GameObject go in weaponVisibleObjects)
            {
                go.SetActive(true);
            }
        }
        public void Hide()
        {
            foreach (GameObject go in weaponVisibleObjects)
            {
                go.SetActive(false);
            }
        }

        public virtual void Fire()
        {
          
        }
    
        public virtual void CmdFireWeapon(Vector3 dir, Vector3 pos, Quaternion rot)
        {

        }
        
        public virtual void CmdUpdateWeaponRotation(Vector3 aimPos)
        {
            Vector3 localDirection = transform.parent.InverseTransformPoint(aimPos + offsetWeaponRotate);
            Debug.DrawRay(firePoint.position, localDirection * 10);
            float angleDegrees = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);
            transform.localRotation = rot;
            RpcUpdateWeaponRotation(rot);
        }
       
        public virtual void RpcUpdateWeaponRotation(Quaternion rot)
        {
            transform.localRotation = rot;
        }

    }
}

