using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Warbuzz.Player
{
    public class PlayerShield : NetworkBehaviour
    {
        [Header("Shield")]
       
        public int shield = 500;
        public int shieldMax = 500;
        public float timeRestore = 1f;
        public int amountRestore = 10;

        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerHealth playerHealth;

        [Client]
        public void StartClient()
        {
            if (isLocalPlayer)
            {
                shield = shieldMax;
                GameManager.Instance.uiManager.gameMenu.UpdateShieldMaxValue(shieldMax);
                GameManager.Instance.uiManager.gameMenu.UpdateShield(shield);
                GameManager.Instance.uiManager.gameMenu.shieldSlider.gameObject.SetActive(true);
            }

        }
        [Server]
        public void StartServer()
        {
            shield = shieldMax;
            StartCoroutine(RestoreShield());
        }
      
        [Server]
        public void TakeDamage(int amount)
        {
            if (!isServer)return;

            playerController.p_animation.AnimHit();
            shield -= amount;

            if (shield <= 0)
            {
                playerHealth.TakeDamage(Mathf.Abs(shield));
            }
            if (shield <= 0)
                RpcUpdateShield(0);
            else
                RpcUpdateShield(shield);
        }
        [ClientRpc]
        public void RpcUpdateShield(int shield)
        {
            this.shield = shield;
            if (isLocalPlayer)
                GameManager.Instance.uiManager.gameMenu.UpdateShield(shield);
        }
         
      
        public IEnumerator RestoreShield()
        {
            while (true)
            {
                yield return new WaitForSeconds(timeRestore);
                if(shield < shieldMax) shield += amountRestore;
                
                if (shield > shieldMax) shield = shieldMax;
                RpcUpdateShield(shield);
            }
        }

    }
}

