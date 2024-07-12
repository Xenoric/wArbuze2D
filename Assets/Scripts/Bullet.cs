using UnityEngine;
using Warbuzz.Player;
using Mirror;
using Warbuzz.Network;

namespace Warbuzz.Weapons
{
    public class Bullet : NetworkBehaviour
    {
        public PlayerController currentPlayer;
        public float multDestroyDistance = 1;
       
        public float bulletForce = 100;
        public PrefabPool prefabPool;
        
        public Vector2 moveDir = Vector2.zero;


        [Server]
        private void DestroyBullet()
        {
            NetworkServer.UnSpawn(gameObject);
            prefabPool.Return(gameObject);
            print("DestroyBullet");
        }

        private void BulletHit(GameObject hitObj)
        {
            if (isServer)
            {
               
                if (hitObj && currentPlayer)
                {
                    if (hitObj.tag == "Player" && hitObj != currentPlayer.gameObject)
                    {
                        hitObj.GetComponent<PlayerController>().shield.TakeDamage(currentPlayer.weaponController.currentWeapon.damage);
                    }
                    if (hitObj.tag != "Bullet")
                        DestroyBullet();

                }
            }
           
        }
       
       
        private void Update()
        {
           
           
            if (isServer)
            {
                transform.position += new Vector3(moveDir.x, moveDir.y, 0) * bulletForce * Time.deltaTime;
                float dist = Vector2.Distance(currentPlayer.transform.position, transform.position);
                if (dist >= currentPlayer.weaponController.currentWeapon.fireRange * multDestroyDistance)
                    DestroyBullet();

                RaycastHit2D hit = gameObject.scene.GetPhysicsScene2D().Raycast(transform.position, transform.forward, 0.1f);
              
                if (hit.collider)
                {

                    BulletHit(hit.collider.gameObject);
                }
            }
          
        }

    }
}

