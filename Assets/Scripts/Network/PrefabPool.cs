using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Warbuzz.Network 
{
    public class PrefabPool : MonoBehaviour
    {
     
       // [Header("Settings")]
        public GameObject prefab;

        [Header("Debug")]
        public int currentCount;
        public Pool<GameObject> pool;
    
        private uint creatureAssetId;

        void Start()
        {
            InitializePool();
            // NetworkClient.RegisterPrefab(prefab, SpawnHandler, UnspawnHandler);
            creatureAssetId = prefab.GetComponent<NetworkIdentity>().assetId;
            NetworkClient.RegisterSpawnHandler(creatureAssetId, SpawnHandler, UnspawnHandler);
        }
      
        GameObject SpawnHandler(SpawnMessage msg) => Get(msg.position, msg.rotation);
        
        void UnspawnHandler(GameObject spawned) => Return(spawned);

        void OnDestroy()
        {
            NetworkClient.UnregisterSpawnHandler(creatureAssetId);
           // NetworkClient.UnregisterPrefab(prefab);
        }

        private void InitializePool()
        {
            pool = new Pool<GameObject>(CreateNew, 15);
        }

        private GameObject CreateNew()
        {
            GameObject next = Instantiate(prefab, transform);
            next.name = $"{prefab.name}_pooled_{currentCount}";
            next.SetActive(false);
            currentCount++;
            return next;
        }
       
        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject next = pool.Get();
           
            next.transform.position = position;
            next.transform.rotation = rotation;
            next.SetActive(true);
            return next;
        }

      
        public void Return(GameObject spawned)
        {
            spawned.SetActive(false);
            pool.Return(spawned);
        }
    }
}



