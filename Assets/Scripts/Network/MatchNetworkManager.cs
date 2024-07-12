using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using Mirror.Examples.MultipleAdditiveScenes;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Warbuzz.Player;

namespace Warbuzz.Network
{
    [System.Serializable]
    public class SubScene
    {
        public Match match;
        public Scene scene;

        public SubScene() { }
        public SubScene(Match match, Scene scene)
        {
            this.match = match;
            this.scene = scene;
        }


    }
    public class MatchNetworkManager : NetworkManager
    {

        public MatchController matchController;
        public NetworkConnectionToClient connectionToClient;
        public FadeInOutScreen fadeInOut;


        private bool isInTransition = false;
        #region Server System Callbacks
        [Header("UI")]
        [SerializeField] private GameObject mainCanvas;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // StartCoroutine(OnServerAddPlayerDelayed(conn));
            print("OnServerAddPlayer" + conn);
           if(conn != null)
            {
               // conn.identity.gameObject.GetComponent<PlayerWeapon>().InitPool();
            }
           
       }
        public void ServerReady(NetworkConnectionToClient conn)
        {
          //  base.OnServerReady(conn);

            if (conn.identity == null)
                StartCoroutine(OnServerAddPlayerDelayed(conn));
        }
        IEnumerator OnServerAddPlayerDelayed(NetworkConnectionToClient conn)
        {

            SubScene subScene = matchController.GetSubScene(conn.connectionId);
           
            conn.Send(new SceneMessage { sceneName = "Level2", sceneOperation = SceneOperation.LoadAdditive, customHandling = true});
           
            yield return new WaitForEndOfFrame();
           


            GameObject player = Instantiate(playerPrefab);
            player.transform.SetParent(null);

            yield return new WaitForEndOfFrame();


            SceneManager.MoveGameObjectToScene(player, subScene.scene);
            Transform startPos = GetStartPosition();

            player.transform.position = startPos.position;
            NetworkServer.AddPlayerForConnection(conn, player);


        }

        public override void OnClientChangeScene(string sceneName, SceneOperation sceneOperation, bool customHandling)
        {
            if (sceneOperation == SceneOperation.UnloadAdditive)
                StartCoroutine(UnloadAdditive(sceneName));

            if (sceneOperation == SceneOperation.LoadAdditive)
                StartCoroutine(LoadAdditive(sceneName));
        }


        IEnumerator LoadAdditive(string sceneName)
        {
            isInTransition = true;

            yield return fadeInOut.FadeIn();

            if (mode == NetworkManagerMode.ClientOnly)
            {
                loadingSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                while (loadingSceneAsync != null && !loadingSceneAsync.isDone)
                {
                    yield return null;
                }
            }


            NetworkClient.isLoadingScene = false;
            isInTransition = false;


            OnClientSceneChanged();
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));

            yield return fadeInOut.FadeOut();

        }


        IEnumerator UnloadAdditive(string sceneName)
        {
            isInTransition = true;

            yield return fadeInOut.FadeIn();


            if (mode == NetworkManagerMode.ClientOnly)
            {
                yield return SceneManager.UnloadSceneAsync(sceneName);
                yield return Resources.UnloadUnusedAssets();
            }


            NetworkClient.isLoadingScene = false;
            isInTransition = false;

            OnClientSceneChanged();

        }

        #endregion

        #region Start & Stop Callbacks

        public override void OnStartClient()
        {

            matchController.StartClient();

        }
        public override void OnStartServer()
        {
          ///  mainCanvas.SetActive(false);
            matchController.StartServer();
         
        }
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            matchController.OnConnectionToClient();
           
        }
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            matchController.OnClientDisconnect();
            
        }
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            matchController.OnServerDisconnect(conn);
          
        }
    
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
                   
        }
          

      
        #endregion
    }

}
