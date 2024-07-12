using Mirror;
using UnityEngine;

namespace Warbuzz.Network
{
    public class PhysicsSimulator : MonoBehaviour
    {
        
        public PhysicsScene2D physicsScene2D;

        bool simulatePhysicsScene2D;

        void Awake()
        {
            
            if (NetworkServer.active)
            {
         
                physicsScene2D = gameObject.scene.GetPhysicsScene2D();
                simulatePhysicsScene2D = physicsScene2D.IsValid() && physicsScene2D != Physics2D.defaultPhysicsScene;
            }
            else
            {
                enabled = false;
            }
        }

        [ServerCallback]
        void FixedUpdate()
        {

            if (simulatePhysicsScene2D)
            {
                physicsScene2D.Simulate(Time.fixedDeltaTime);

            }  
        }
    }
}


