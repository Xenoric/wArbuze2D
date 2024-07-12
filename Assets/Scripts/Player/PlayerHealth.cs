using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using Warbuzz.UI;
namespace Warbuzz.Player
{
	public class PlayerHealth: NetworkBehaviour
	{
		[Header("Health")]
		[SyncVar] public int health = 1000;
		[SyncVar] public int healthMax = 1000;
		public bool isDeath = false;
		
		
		[SerializeField] private PlayerController playerController;
	
		[Client]
		public void StartClient()
        {
			if (isLocalPlayer)
			{
				GameManager.Instance.uiManager.gameMenu.UpdateHealthMaxValue(health);
				GameManager.Instance.uiManager.gameMenu.UpdateHealt(health);
			}
		}

		[Server]
		public void StartServer()
        {
			
			health = healthMax;
			
		}
	
		[Server]
		public void TakeDamage(int amount)
		{
			if (!isServer) return;

			health -= amount;
			if (health <= 0)
			{
				
				Death();
			
			}
			RpcUpdateHealth(health);
		}

		[Server]
		public void Death()
		{
			isDeath = true;
			StopCoroutine(playerController.shield.RestoreShield());
			RpcDeath();
			playerController.p_animation.AnimDeath(true);
					}
		[ClientRpc]
		public void RpcDeath()
		{
			isDeath = true;
			if (isLocalPlayer)
            {
				GameManager.Instance.uiManager.gameMenu.EnablePanelDeath(true);
				
			}
			playerController.weaponController.currentWeapon.gameObject.SetActive(false);

		}

		[Client]
		public void PlayAgain()
        {
			CmdPlayAgain();

		}
		[Command]
		public void CmdPlayAgain()
        {

			health = healthMax;
			playerController.shield.StartServer();
			transform.position = GameController.Instance.spawnPoints[Random.Range(0, GameController.Instance.spawnPoints.Count)].transform.position;
			isDeath = false;
			playerController.p_animation.AnimDeath(false);
			
			RpcPlayAgain();
		}
		[ClientRpc]
		public void RpcPlayAgain()
		{
			if (isLocalPlayer)
			{
				GameManager.Instance.uiManager.gameMenu.UpdateHealthMaxValue(healthMax);
				GameManager.Instance.uiManager.gameMenu.UpdateHealt(healthMax);
				playerController.shield.StartClient();
				GameManager.Instance.uiManager.gameMenu.EnablePanelDeath(false);
				
			}
			playerController.weaponController.currentWeapon.gameObject.SetActive(true);

		}
		
		[ClientRpc]
		public void RpcUpdateHealth(int health)
		{
			this.health = health;
			if(isLocalPlayer)
				GameManager.Instance.uiManager.gameMenu.UpdateHealt(health);
		}
	
	}
}

