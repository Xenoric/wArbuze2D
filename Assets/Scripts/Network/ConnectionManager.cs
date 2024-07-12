using UnityEngine;
using Mirror;
using kcp2k;
using System;



public class ConnectionManager : MonoBehaviour
{
    [SerializeField]
    private NetworkManager networkManager;
    [SerializeField]
    public bool autoStartServer = false;
    [SerializeField]
    public bool autoStartHost = false;
    private void Start()
    {
       
        ushort port;
        string[] args = Environment.GetCommandLineArgs();

        if (autoStartHost)
        {
             networkManager.StartHost();
        }else
        if (autoStartServer)
        {
            print("StartServer............");
            networkManager.StartServer();
           
        }

        for (int i = 0; i < args.Length; i++)
       {
           if (args[i] == "-port")
           {
               port = ushort.Parse(args[i + 1]);
              

               networkManager.StartServer();
           }
           else if (args[i] == "-server")
           {

               networkManager.StartServer();
           }
       }

    }

}
