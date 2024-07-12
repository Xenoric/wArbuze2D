using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Warbuzz.Player;

namespace Warbuzz.Weapons
{
    [System.Serializable]
    public class WeaponAssault : Weapon
    {
        [Header("Bullets")]

        [SyncVar] public float reloadTime = 2;
        [SyncVar] public float bulletForce = 300;

        [Header("Resouces")]
        public GameObject bulletPref;

        private int FireTrigger => Animator.StringToHash("FireTrigger");

        private bool isReloaded = true;

        [ClientRpc]
        private void RpcAnimFire()
        {
            animator.SetTrigger(FireTrigger);
        }

        [Client]
        public override void Fire()
        {
            Vector3 dir = firePoint.position - center.position;
            CmdFireWeapon(dir.normalized, firePoint.position, firePoint.rotation);
          
        }
        IEnumerator Reload()
        {
            
            for (float i = 0; i < reloadTime; i += 0.1f)
            {
                yield return new WaitForSeconds(0.1f);
                RpcReload(i);
            }
            if (allBullets >= bulletsMax)
            {
                allBullets -= bulletsMax;
                bullets += bulletsMax;

            }
            else
            {
                allBullets -= bulletsMax;
                bullets += Mathf.Abs(allBullets);
                allBullets = 0;
            }
            isReloaded = true;
          
            RpcUpdateBullets(bullets, allBullets);
        }

        [ClientRpc]
        public void RpcReload(float time)
        {
            if (isLocalPlayer) 
                GameManager.Instance.uiManager.gameMenu.UpdateCurrentBullets(time.ToString("F1"), allBullets.ToString());
        }

        [Command]
        public override void CmdFireWeapon(Vector3 dir, Vector3 pos, Quaternion rot)
        {
            if (!isReloaded || allBullets <= 0 && bullets <= 0) return;
            if(bullets <= 0)
            {
                isReloaded = false;
                StartCoroutine(Reload());
                return;
            }else
                bullets -= 1;
            
            GameObject bulletGo = playerController.weaponController.prefabPool.Get(pos, rot);
          
            Bullet bullet = bulletGo.GetComponent<Bullet>();
            bullet.currentPlayer = playerController;
            bullet.prefabPool = playerController.weaponController.prefabPool;
            bullet.bulletForce = bulletForce;
            Vector2 bulletDirection = dir;

            // int rndAngle = Random.Range(-fireRandomRange, fireRandomRange + 1);
            // Quaternion randomRotation = Quaternion.Euler(rndAngle, rndAngle, 0);

            // bullet.moveDir = ((randomRotation * bulletDirection).normalized);
            bullet.moveDir = bulletDirection;

            NetworkServer.Spawn(bulletGo);
            RpcUpdateBullets(bullets, allBullets);
            RpcAnimFire();

        }
        [ClientRpc]
        private void RpcUpdateBullets(int bullets, int allBullets)
        {
            this.bullets = bullets;
            this.allBullets = allBullets;
            if (isLocalPlayer)
            {
                GameManager.Instance.uiManager.gameMenu.UpdateCurrentBullets(bullets, allBullets);
               
            }
        }
        [Command]
        public override void CmdUpdateWeaponRotation(Vector3 aimPos)
        {
            Vector3 localDirection = transform.parent.InverseTransformPoint(aimPos + offsetWeaponRotate);
            Debug.DrawRay(firePoint.position, localDirection * 10);
            float angleDegrees = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, 0, angleDegrees);
            transform.localRotation = rot;
            RpcUpdateWeaponRotation(rot);
        }
        [ClientRpc]
        public override void RpcUpdateWeaponRotation(Quaternion rot)
        {
            transform.localRotation = rot;
        }
    }
}

