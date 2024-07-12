using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Warbuzz.Player
{
    public class PlayerAnimation : NetworkBehaviour
    {
        [Header("Animations")]
        [SerializeField] private Animator animator;
        [SerializeField] private WeaponController weaponController;

        private int DeathBack => Animator.StringToHash("DeathBack");
        private int MoveDir => Animator.StringToHash("MoveDir");
        private int Jump => Animator.StringToHash("Jump");
        private int Hit => Animator.StringToHash("Hit");
        private int Grounded => Animator.StringToHash("Grounded");
        private int FacingRight => Animator.StringToHash("FacingRight");
        private int Fly => Animator.StringToHash("Fly");
       
        [Server]
        public void AnimDeath(bool death)
        {
            RpcAnimDeath(death);
        }
        [ClientRpc]
        public void RpcAnimDeath(bool death)
        {
            animator.SetBool(DeathBack, death);
        }
        [Command]
        public void CmdAnimMove(float dir)
        {
            RpcAnimMove(dir);
        }
       
        [ClientRpc]
        public void RpcAnimMove(float dir)
        {
            animator.SetInteger(MoveDir, (int)dir);
        }
        [Command]
        public void CmdAnimJump(bool isJump)
        {
            RpcAnimJump(isJump);
        }
        [ClientRpc]
        public void RpcAnimJump(bool isJump)
        {
            animator.SetBool(Jump, isJump);
        }
        [Server]
        public void AnimHit()
        {
            RpcHit();
        }
        [ClientRpc]
        public void RpcHit()
        {
            animator.SetTrigger(Hit);

        }

        //public void AnimGrounded(bool isGround)
      //  {
       //     animator.SetBool(Grounded, isGround);
      //  }
        [Command]
        public void CmdAnimFacing(bool isRight)
        {
            RpcAnimFacing(isRight);
        }
        [ClientRpc]
        public void RpcAnimFacing(bool isRight)
        {
            animator.SetBool(FacingRight, isRight);
        }
        [Command]
        public void CmdAnimFly(bool isFly)
        {
            RpcAnimFly(isFly);
        }
        [ClientRpc]
        public void RpcAnimFly(bool isFly)
        {
            animator.SetBool(Fly, isFly);
           // if (isFly)
           // {
               // AnimMove(0);
           // }
        }
    }
}

