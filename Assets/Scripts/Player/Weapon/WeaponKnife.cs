using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Warbuzz.Player;

namespace Warbuzz.Weapons
{
    [System.Serializable]
    public class WeaponKnife : Weapon
    {
         
        [Header("Effects")]
        [SerializeField] private ParticleSystem knife_Slash_MeleeTrail;


        private int Slash => Animator.StringToHash("Slash");
      
        [ClientRpc]
        private void RpcAnimFire()
        {
            animator.CrossFade(Slash, 0.1f);
            knife_Slash_MeleeTrail.Play();
        }

        [Client]
        public override void Fire()
        {
           // Vector3 dir = firePoint.position - center.position;
            CmdFireWeapon(Vector3.zero, firePoint.position, firePoint.rotation);
            
        }
        [Command]
        public override void CmdFireWeapon(Vector3 dir, Vector3 pos, Quaternion rot)
        {
            RpcAnimFire();
        }
       

    }
}

