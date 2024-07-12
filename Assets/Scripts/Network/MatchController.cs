using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;
using Warbuzz.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Warbuzz.Player;

namespace Warbuzz.Network
{
   
    public struct UpdateMatchesMsg : NetworkMessage
    {
        public List<Match> matches;

        public UpdateMatchesMsg(List<Match> matches)
        {
            this.matches = matches;
        }
    }

    public class MatchController : MonoBehaviour
    {
        public static string[] sceneNames = { "Level2", "Level2" };
        public List<Match> matches = new List<Match>();
        public Match currentMatch = null;
        public Player currentPlayer = null;
        public GameObject playerPrefab;
        public bool inMatch = false;
        public bool isLead = false;

        public GameObject bulletPoolPref;

        private bool subscenesLoaded;

        public List<SubScene> subScenes = new List<SubScene>();
        
        public void StartClient()
        {
       
            NetworkClient.RegisterHandler<ClientMatchMessage>(OnClientMatchMessage);
        }

        public void OnConnectionToClient()
        {

            NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.GetList, match = null });
        }
        public void OnClientDisconnect()
        {

            ResetComponentsDefault();
        }
        [Server]
        public void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            Match match = GetMatch(conn.connectionId);
            if(match != null)
                LeaveMatch(conn, new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Leave, match = match});
        }
        public void StartServer()
        {
            NetworkServer.RegisterHandler<ServerMatchMessage>(OnServerMatchMessage);
        }

        private void ResetComponentsDefault()
        {
            matches.Clear();
            currentMatch = null;
            currentPlayer = null;
            inMatch = false;
            isLead = false;
        }

        #region Create match
        [Client]
        public void RequestCreateMatch(int level, int maxPlayer)
        {
         
            NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Create,match = new Match {levelIndx = level, maxPlayers = maxPlayer }}); 
        }
        [Server]
        public void CreateMatch(NetworkConnectionToClient conn, ServerMatchMessage msg)
        {

            Player player = new Player { isLead = true, connectionId = conn.connectionId };

            Match match = new Match
            {
                matchID = Guid.NewGuid(),
                levelIndx = msg.match.levelIndx,
                maxPlayers = msg.match.maxPlayers,
                players = new List<Player>()
            };

            match.players.Add(player);
            matches.Add(match);

            conn.Send(new ClientMatchMessage(ClientMatchOperation.Created, matches, match, player, "succes"));
            ServerLoadSubScene(match);
            SendMatchesAllPlayer();
           

        }

        [Client]
        public void CreateMatchCallBack(ClientMatchMessage msg)
        {
            currentMatch = msg.match;
            currentPlayer = msg.player;
            inMatch = true;
            isLead = true;
            GameManager.Instance.uiManager.CreatedMatch(msg);
            

        }
        #endregion

        #region Join match
        [Client]
        public void RequestJoinMatch(Match match)
        {
           
            NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Join, match = match });
        }
        [Server]
        public void JoinMatch(NetworkConnectionToClient conn, ServerMatchMessage msg)
        {
            Match match = GetMatch(msg.match.matchID);
            if (match == null)
            {
                conn.Send(new ClientMatchMessage(ClientMatchOperation.Joined, matches, null, null, "not found"));
            }
            else
            {

                if (match.players.Count >= match.maxPlayers)
                {
                    conn.Send(new ClientMatchMessage(ClientMatchOperation.Joined, matches, null, null, "max players"));
                }
                else
                {
                    Player player = new Player{isLead = false, connectionId = conn.connectionId};
                   
                    match.players.Add(player);
                    conn.Send(new ClientMatchMessage(ClientMatchOperation.Joined, matches, match, player, "succes"));

                    SendMatchesAllPlayer();
                }
            }


        }
        [Client]
        public void JoinMatchCallBack(ClientMatchMessage msg)
        {

            if (msg.result == "succes")
            {
                currentPlayer = msg.player;
                currentMatch = msg.match;
                inMatch = true;
                isLead = false;
                GameManager.Instance.uiManager.JoinedMatch();
            }
            else
            {
              //  GameManager.Instance.uiManager.btnJoin.interactable = true;
            }


        }

        #endregion
        #region Leave match
        [Client]
        public void RequestLeaveMatch()
        {
            
            NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Leave, match = currentMatch});
        }

      
        [Server]
        public void LeaveMatch(NetworkConnectionToClient conn, ServerMatchMessage msg)
        {

            Match match = GetMatch(msg.match.matchID);
            
            Player player_ = match.GetPlayer(msg.match.players[0].connectionId);
            match.players.Remove(player_);
            conn.Send(new ClientMatchMessage(ClientMatchOperation.Departed, matches, match, player_, "succes"));

            if (match.players.Count <= 0)
            {
                matches.Remove(match);
                return;
            }
              

            if (player_.isLead)
            {
                matches.Remove(match);
               
               
            }
            SendMatchesAllPlayer();


        }
        [Client]
        public void LeaveMatchCallBack(ClientMatchMessage msg)
        {

            if (msg.result == "succes")
            {
                currentPlayer = null;
                currentMatch = null;
                inMatch = false;
                isLead = false;
                GameManager.Instance.uiManager.LeaveMatch();
              
            }
            else
            {
               
            }


        }
        #endregion
        #region Start match
        [Client]
        public void RequestStartMatch()
        {
           
            NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Start, match = currentMatch});
        }
        [Server]
        public void StartMatch(NetworkConnectionToClient conn, ServerMatchMessage msg)
        {

            Match match = GetMatch(msg.match.matchID);
            Player player_ = match.GetPlayer(conn.connectionId);
        
            if (player_.isLead )//&& match.players.Count == match.maxPlayers)
            {
               foreach(Player player in match.players)
                {
                    NetworkConnectionToClient connectionToClient = GetConnection(player.connectionId);
                    connectionToClient.Send(new ClientMatchMessage(ClientMatchOperation.Started, matches, match, player_, "succes"));

                  

                }

            }
            SubScene subScene = GetSubScene(conn.connectionId);
            foreach(Player mPlayer in subScene.match.players)
            {
                StartCoroutine(OnServerAddPlayerDelayed(GetConnection(mPlayer.connectionId)));
            }
            
            SendMatchesAllPlayer();

        }

        IEnumerator OnServerAddPlayerDelayed(NetworkConnectionToClient conn)
        {

            SubScene subScene = GetSubScene(conn.connectionId);

            GameObject player = Instantiate(playerPrefab);
            player.transform.SetParent(null);
            player.GetComponent<PlayerController>().currentScene = subScene;

            SceneManager.MoveGameObjectToScene(player, subScene.scene);
            player.GetComponent<WeaponController>().InitPool();
            conn.Send(new SceneMessage { sceneName = "Level2", sceneOperation = SceneOperation.LoadAdditive, customHandling = true });

            yield return new WaitForEndOfFrame();
        
            Transform startPos = NetworkManager.singleton.GetStartPosition();
            player.transform.position = startPos.position;

           
            NetworkServer.AddPlayerForConnection(conn, player);


        }
      
        [Client]
        public void StartMatchCallBack(ClientMatchMessage msg)
        {
           
            if (msg.result == "succes")
            {
                        
                GameManager.Instance.uiManager.StartMatch();
               
            }
        }

        public SubScene GetSubScene(Guid matchId)
        {
            foreach (SubScene subScene in subScenes)
            {
                if (subScene.match.matchID.Equals(matchId))
                    return subScene;
            }
            return null;
        }
        public SubScene GetSubScene(int connId)
        {
            foreach (SubScene subScene in subScenes)
            {
               
                foreach (Player player in subScene.match.players)
                {
                    if (player.connectionId == connId)
                        return subScene;
                }

            }
            return null;
        }
     
        public void ServerLoadSubScene(Match match)
        {
            StartCoroutine(IEServerLoadSubScene(match));
        }

        public IEnumerator IEServerLoadSubScene(Match match)
        {
            string sceneName = sceneNames[match.levelIndx];

            yield return SceneManager.LoadSceneAsync(sceneName, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics2D });
            Scene scene = SceneManager.GetSceneByName(sceneName);

            print(scene.name + " - " + sceneNames[match.levelIndx] + "   " + SceneManager.sceneCount);
            SubScene newScene = new SubScene(match, scene);
            newScene.match = match;
            subScenes.Add(newScene);

           // GameObject reward = Instantiate(bulletPoolPref, Vector3.zero, Quaternion.identity);
           // SceneManager.MoveGameObjectToScene(reward, scene);
           // NetworkServer.Spawn(reward);

            subscenesLoaded = true;

        }
        #endregion
        [Server]
        private void OnServerMatchMessage(NetworkConnectionToClient conn, ServerMatchMessage msg)
        {
            switch (msg.serverMatchOperation)
            {
                case ServerMatchOperation.None:
                    {
                        Debug.LogWarning("Missing ServerMatchOperation");
                        break;
                    }
                case ServerMatchOperation.Create:
                    {
                        CreateMatch(conn, msg);
                        break;
                    }
                case ServerMatchOperation.Cancel:
                    {
                      //  OnServerCancelMatch(conn);
                        break;
                    }
                case ServerMatchOperation.Join:
                    {
                        JoinMatch(conn, msg);
                        break;
                    }
                case ServerMatchOperation.Leave:
                    {
                        LeaveMatch(conn, msg);
                        break;
                    }
                case ServerMatchOperation.Ready:
                    {
                       // OnServerPlayerReady(conn, msg.matchId);
                        break;
                    }
                case ServerMatchOperation.Start:
                    {
                        StartMatch(conn, msg);

                        break;
                    }
                case ServerMatchOperation.GetList:
                    {
                        conn.Send(new ClientMatchMessage(ClientMatchOperation.UpdateList, matches, null, null, "succes"));

                        break;
                    }
            }
        }
        [Client]
        private void OnClientMatchMessage(ClientMatchMessage msg)
        {
            switch (msg.clientMatchOperation)
            {
                case ClientMatchOperation.None:
                    {
                        Debug.LogWarning("Missing ClientMatchOperation");
                        break;
                    }
                case ClientMatchOperation.UpdateList:
                    {
                        UpdateMatches(msg);

                        break;
                    }
                case ClientMatchOperation.Created:
                    {
                      
                        CreateMatchCallBack(msg);
                        
                        break;
                    }
                case ClientMatchOperation.Cancelled:
                    {
                       
                        break;
                    }
                case ClientMatchOperation.Joined:
                    {
                        JoinMatchCallBack(msg);
                        break;
                    }
                case ClientMatchOperation.Departed:
                    {
                        LeaveMatchCallBack(msg);
                        UpdateMatches(msg);
                        break;
                    }
                case ClientMatchOperation.UpdateRoom:
                    {
                      
                        break;
                    }
                case ClientMatchOperation.Started:
                    {
                        StartMatchCallBack(msg);
                        break;
                    }
            }
        }

        [Server]
        public void SendMatchesAllPlayer()
        {
            foreach (KeyValuePair<int, NetworkConnectionToClient> networkConnection in NetworkServer.connections)
            {
                networkConnection.Value.Send(new ClientMatchMessage(ClientMatchOperation.UpdateList, matches, null, null, "succes"));
            }
        }
        [Client]
        public void UpdateMatches(ClientMatchMessage msg)
        {
           // GameManager.Instance.uiManager.ClearMatchItems();

            foreach (Match match in msg.matches)
            {
                                   
                GameManager.Instance.uiManager.AddMatchItems(match);

            }
        }

    
        [Server]
        private Match GetMatch(Guid matchId)
        {
            foreach (Match match in matches)
            {
                if (match.matchID.Equals(matchId))
                {
                    return match;
                }
            }
                      
            return null;
        }
        [Server]
        private Match GetMatch(int connectionId)
        {
            foreach (Match match in matches)
            {
                foreach (Player player in match.players)
                {
                    if (connectionId == player.connectionId)
                        return match;
                }
               
            }

            return null;
        }
        [Server]
        private NetworkConnectionToClient GetConnection(int id)
        {
            foreach (KeyValuePair<int, NetworkConnectionToClient> networkConnection in NetworkServer.connections)
            {
                if (networkConnection.Value.connectionId == id)
                    return networkConnection.Value;
            }
            return null;
        }
      

    }
}

