using System;
using System.Collections.Generic;
using Mirror;

namespace Warbuzz.Network
{
    [System.Serializable]
    public class Player
    {
        public string playerName;
        public bool isLead;
        public int connectionId;

    }
    [System.Serializable]
    public class Match 
    {
        public Guid matchID;
        public int levelIndx;
        public int maxPlayers;
        public List<Player> players;

        public Match()
        {
        }

        public Match(Guid matchID, int levelIndx, int maxPlayers, List<Player> players)
        {
            this.matchID = matchID;
            this.levelIndx = levelIndx;
            this.maxPlayers = maxPlayers;
            this.players = players;
        }

        public Player GetPlayer(int connId)
        {
            foreach (Player player_ in players)
            {
                if (player_.connectionId == connId) return player_;
            }
            return null;
        }
      
    }
    /// <summary>
    /// Match message to be sent to the server
    /// </summary>
    public struct ServerMatchMessage : NetworkMessage
    {
        public ServerMatchOperation serverMatchOperation;
        public Match match;

    }

    /// <summary>
    /// Match message to be sent to the client
    /// </summary>
    public struct ClientMatchMessage : NetworkMessage
    {
        public ClientMatchOperation clientMatchOperation;
      
        public List<Match> matches;
        public Match match;
        public Player player;
        public string result;

        public ClientMatchMessage(ClientMatchOperation clientMatchOperation, List<Match> matches, Match match, Player player, string result)
        {
            this.clientMatchOperation = clientMatchOperation;
            this.matches = matches;
            this.match = match;
            this.player = player;
            this.result = result;
        }
    }

    /// <summary>
    /// Match operation to execute on the server
    /// </summary>
    public enum ServerMatchOperation : byte
    {
        None,
        Create,
        Cancel,
        Start,
        Join,
        Leave,
        Ready,
        GetList
    }

    /// <summary>
    /// Match operation to execute on the client
    /// </summary>
    public enum ClientMatchOperation : byte
    {
        None,
        UpdateList,
        Created,
        Cancelled,
        Joined,
        Departed,
        UpdateRoom,
        Started
    }

   
}
