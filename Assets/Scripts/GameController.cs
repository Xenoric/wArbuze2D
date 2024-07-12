using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Warbuzz.Network;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public List<NetworkStartPosition> spawnPoints;
    public PhysicsSimulator physicsSimulator;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
}
