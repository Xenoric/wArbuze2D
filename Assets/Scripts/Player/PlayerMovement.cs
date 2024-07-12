using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using Warbuzz.UI;

namespace Warbuzz.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [Header("Move")]
        [SerializeField] private float moveSpeed = 1600f;

        [Header("Jump")]
        [SerializeField] private float jumpHeight = 15f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        [Header("Fly")]
        public float flySmoothTime = 0.3F;
        public int flyEnergy = 100;
        public float flyMoveSpeed = 0.1F;
        [SyncVar] public bool isFly = false;
        public Vector3 offserEnergyBar = Vector3.zero;

        
        public bool isGrounded;
        
        [SerializeField] private Rigidbody2D rb2d;

        #region private
        private Vector2 velocity = Vector2.zero;
        private Vector2 flyPos = Vector2.zero;
      
        private GameMenu gameMenu;
        private Vector2 moveVel = Vector2.zero;
        private float jumpYVel = 0;

        #endregion
        public void StartServer()
        {
            flyEnergy = playerController.playerData.flyEnergy;
            StartCoroutine(FlyEnergyCounter(1));
        }
        public void StartClient()
        {
            flyEnergy = playerController.playerData.flyEnergy;
            if (isLocalPlayer)
            {
                gameMenu = GameManager.Instance.uiManager.gameMenu;
                gameMenu.energySlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + offserEnergyBar);
                gameMenu.energySlider.gameObject.SetActive(true);
                gameMenu.UpdateEnergyMaxValue(flyEnergy);
                gameMenu.UpdateEnergy(flyEnergy);
            }
        }
  
        public void UpdateEnergyBar()
        {
            if (gameMenu)
                gameMenu.energySlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + offserEnergyBar);
        }
        public void UpdatePlayerRotation(Vector3 aimPos)
        {
            Vector3 direction = aimPos - transform.position;
            direction = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.localScale = new Vector3(angle > 90 ? -1 : 1, 1, 1);
           
            
        }
    
        [Client]
        public void GroundedCheck()
        {
            isGrounded = gameObject.scene.GetPhysicsScene2D().OverlapCircle(groundCheck.position, 0.5f, groundLayer);
           
        }
      
        [Client]
        public void HandleMove(float dir)
        {
            isFly = false;
            playerController.p_animation.CmdAnimFly(false);
            if (isGrounded)
            {
                moveVel = rb2d.velocity;
                moveVel.x = dir * moveSpeed * Time.fixedDeltaTime;
                rb2d.velocity = moveVel;
              
                playerController.p_animation.CmdAnimFacing((int)playerController.input.horizontalInputRaw == transform.localScale.x);
                playerController.p_animation.CmdAnimMove(playerController.input.horizontalInputRaw);
            }
            else
            {
                playerController.p_animation.CmdAnimMove(0);
            }

        }
        [Client]
        public void HandleJump(bool isJmp)
        {
            if (isGrounded && isJmp)
            {
                jumpYVel =  Mathf.Sqrt((-2 * rb2d.gravityScale * Physics2D.gravity.y * jumpHeight));
             
                moveVel = rb2d.velocity;
                moveVel.y = jumpYVel;
                rb2d.velocity = moveVel;
               
            }
            playerController.p_animation.CmdAnimJump(isJmp);
        }

     
        [Client]
        public void HandleFly(Vector2 dir)
        {
            isFly = true;
            playerController.p_animation.CmdAnimFly(true);
            flyPos = Vector2.SmoothDamp(transform.position, new Vector2(transform.position.x + (dir.x + flyMoveSpeed), transform.position.y + 1), ref velocity, flySmoothTime);
            rb2d.MovePosition(flyPos);
           
        }
        public IEnumerator FlyEnergyCounter(float delay)
        {

            while (true)
            {
                if (isFly)
                {
                    if (flyEnergy > 0)
                    {
                        flyEnergy -= 1;
                        RpcUpdateFlyEnergy(flyEnergy);
                    }

                }
                yield return new WaitForSeconds(delay);
            }

        }
      
        [ClientRpc]
        private void RpcUpdateFlyEnergy(int energy)
        {
            if (playerController.isLocalPlayer)
                gameMenu.UpdateEnergy(energy);
        }



    }
}
