using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

namespace Warbuzz.Network
{
   
    public class MyNetworkManager : NetworkManager
    {

        public MatchController matchController;
        public FadeInOutScreen fadeInOut;


        private string[] sceneNames = { "Level1", "Level1" };
        private bool subscenesLoaded;

        private readonly List<Scene> subScenes = new List<Scene>();

        private bool isInTransition;
        private bool firstSceneLoaded;



        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);


            fadeInOut.ShowScreenNoDelay();


            if (sceneName == onlineScene)
            {
                StartCoroutine(ServerLoadSubScenes("Level1"));
            }
        }

        public override void OnClientSceneChanged()
        {
            if (isInTransition == false)
            {
                base.OnClientSceneChanged();
            }
        }


        IEnumerator ServerLoadSubScenes(string name)
        {

            yield return SceneManager.LoadSceneAsync(name, new LoadSceneParameters
            {
                loadSceneMode = LoadSceneMode.Additive,
                localPhysicsMode = LocalPhysicsMode.Physics2D
            });

            subscenesLoaded = true;
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


            if (firstSceneLoaded == false)
            {
                firstSceneLoaded = true;

                yield return new WaitForSeconds(0.6f);
            }
            else
            {
                firstSceneLoaded = true;

                yield return new WaitForSeconds(0.5f);
            }


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




        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);
            print("Ready .........");
            if (conn.identity == null)
                StartCoroutine(AddPlayerDelayed(conn));
        }

        IEnumerator AddPlayerDelayed(NetworkConnectionToClient conn)
        {
            while (subscenesLoaded == false)
                yield return null;


            NetworkIdentity[] allObjsWithANetworkIdentity = FindObjectsOfType<NetworkIdentity>();

            foreach (var item in allObjsWithANetworkIdentity)
            {
                item.enabled = true;
            }

            firstSceneLoaded = false;


            conn.Send(new SceneMessage { sceneName = sceneNames[0], sceneOperation = SceneOperation.LoadAdditive, customHandling = true });

            Transform startPos = GetStartPosition();


            GameObject player = Instantiate(playerPrefab, startPos);
            player.transform.SetParent(null);


            yield return new WaitForEndOfFrame();


            SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(sceneNames[0]));


            NetworkServer.AddPlayerForConnection(conn, player);

        }


    }
}
