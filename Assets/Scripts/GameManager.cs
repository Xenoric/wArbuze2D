using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Warbuzz.Player;
using Warbuzz.UI;
using Warbuzz.Network;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Controllers")]
    public MatchNetworkManager networkManager;
    public PlayerController localPlayer;
    public UIManager uiManager;

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

    public static void DeleteSingletonInstance()
    {
        if (Instance)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }
}
