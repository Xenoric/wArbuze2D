using UnityEngine;
using Mirror;
using TMPro;
using Warbuzz.Network;
using UnityEngine.SceneManagement;

namespace Warbuzz.Player
{
    public partial class PlayerController : NetworkBehaviour
    {
        public bool isEnable = false;
        public PlayerData playerData;
        
        [Header("Controllers")]
        public PlayerCamera p_camera;
        public PlayerInput input;
        public PlayerMovement movement;
        public PlayerHealth health;
        public PlayerShield shield;
        public WeaponController weaponController;
        public PlayerAnimation p_animation;

        [Header("Other")]
        public SubScene currentScene;
                
        public override void OnStartClient()
        {
            input.InitializeAim(isLocalPlayer);
            movement.StartClient();
            weaponController.StartClient();
            health.StartClient();
            shield.StartClient();
          
        }
        public override void OnStartLocalPlayer()
        {
            p_camera.Init();
            GameManager.Instance.localPlayer = this;

        }
        public override void OnStartServer()
        {
            input.InitializeAim(isLocalPlayer);
            movement.StartServer();
            weaponController.StartServer();
            health.StartServer();
            shield.StartServer();
          
        }
        private void Start()
        {
                 
            isEnable = true;
        }
        private void Update()
        {
            if (!isEnable) return;
            if (health.isDeath) return;
            if (isLocalPlayer)
            {
                
                p_camera.UpdateCameraPosition(input.aimTransform.position);
                input.UpdateAimPosition();
                movement.UpdatePlayerRotation(input.aimTransform.position);
                movement.UpdateEnergyBar();
                weaponController.UpdateWeaponRotation(input.aimTransform.position);
                weaponController.CheckChangeWeapon();
                weaponController.CanShootCheck();
            }


        }
        private void FixedUpdate()
        {
            if (!isEnable) return;
            if (health.isDeath) return;
            if (isLocalPlayer)
            {
                movement.GroundedCheck();
                if (Input.GetKey(input.keyBtnFly))
                {
                    movement.HandleFly(new Vector2(input.horizontalInputRaw, Mathf.Clamp(input.verticalInputRaw, 0, 1)));
                   
                }
                else
                {
                    
                    movement.HandleMove(input.horizontalInputRaw);
                    movement.HandleJump(Input.GetKey(input.keyBtnJump));
                   
                }
               
            }
        }

    }
}

