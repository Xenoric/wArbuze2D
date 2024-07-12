using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Warbuzz.Network;
using System;

namespace Warbuzz.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Button btnCreate;
        [SerializeField] private Button btnJoin;
        [SerializeField] private Button btnLeave;
        [SerializeField] private Button btnStart;
        [SerializeField] private Button btnPlay;
        [SerializeField] private Button btnPlayCustom;

        [SerializeField] private TMP_Dropdown ddLevel;
        [SerializeField] private TMP_Dropdown ddPlayers;
        [SerializeField] private Transform matchListContent;
        [SerializeField] private GameObject matchItemPref;

        [SerializeField] private Canvas canvasMain;
        
        public GameMenu gameMenu;
        [SerializeField] private GameObject panelMatchesChouse;
        public MatchItem selectedItem;

        private List<MatchItem> matchItems = new List<MatchItem>();

        #region events
        public void OnBtnPlayClick()
        {
         
        }
        public void OnBtnPlayCustomClick()
        {
            panelMatchesChouse.SetActive(true);
        }
        public void OnBtnMatchesChouseCloseClick()
        {
            panelMatchesChouse.SetActive(false);
        }
        public void OnBtnJoinMatchClick()
        {
            btnJoin.interactable = false;
            GameManager.Instance.networkManager.matchController.RequestJoinMatch(selectedItem.match);
        }

        public void OnBtnCreateMatchClick()
        {
            btnCreate.interactable = false;
            GameManager.Instance.networkManager.matchController.RequestCreateMatch(ddLevel.value + 1, ddPlayers.value + 2);


        }
        public void OnBtnLeaveMatchClick()
        {
            btnLeave.interactable = false;
            GameManager.Instance.networkManager.matchController.RequestLeaveMatch();

        }
        public void OnBtnStartMatchClick()
        {
            btnCreate.interactable = false;
            GameManager.Instance.networkManager.matchController.RequestStartMatch();


        }
        #endregion

        #region Matches
        public void ClearMatchItems()
        {
            foreach (MatchItem item in matchItems)
            {
                Destroy(item.gameObject);
            }
            matchItems.Clear();
        }
        public void ResetSelectedMatchItems()
        {
            foreach (MatchItem item in matchItems)
            {
                if (item.isSelected) item.isSelected = false;
            }
            selectedItem = null;
           
        }
        public void AddMatchItems(Match match)
        {
            MatchItem fItem = FindMatch(match.matchID);
    
            if (fItem)
            {
                if (match.matchID.Equals(fItem.match.matchID))
                {
                    GameManager.Instance.networkManager.matchController.currentMatch = match;
                }
                fItem.playersText.text = match.players.Count + "/" + match.maxPlayers;
                fItem.inMatch = GameManager.Instance.networkManager.matchController.currentMatch != null && GameManager.Instance.networkManager.matchController.currentMatch.matchID.Equals(match.matchID);
                fItem.match = match;
                fItem.gameObject.SetActive(true);

                if (fItem.match.players.Count <= 0)
                {
                    matchItems.Remove(fItem);
                    Destroy(fItem.gameObject);
                    return;
                }

            }
            else
            {
                MatchItem item = Instantiate(matchItemPref, matchListContent).GetComponent<MatchItem>();
                item.playersText.text = match.players.Count + "/" + match.maxPlayers;
                item.match = match;
                item.inMatch = GameManager.Instance.networkManager.matchController.currentMatch.matchID.Equals(match.matchID); ;
                item.pointerClick += OnPointerClick;
                item.gameObject.SetActive(true);
                matchItems.Add(item);
            }

            UpdateBtnsState();
        }
        private MatchItem FindMatch(Guid matchId)
        {
            foreach (MatchItem item in matchItems)
            {
                if (item.match.matchID.Equals(matchId))
                {
                    return item;
                }
            }
            return null;
        }
        public void CreatedMatch(ClientMatchMessage msg)
        {
           
            UpdateBtnsState();
        }
        public void JoinedMatch()
        {
          
            UpdateBtnsState();
        }
        public void LeaveMatch()
        {
          
            ResetSelectedMatchItems();

            UpdateBtnsState();

        }
        public void StartMatch()
        {
         
            UpdateBtnsState();
            canvasMain.gameObject.SetActive(false);
            gameMenu.gameObject.SetActive(true);
        }
        public void OnPointerClick(MatchItem item)
        {
          
            ResetSelectedMatchItems();
            item.isSelected = true;
            selectedItem = item;

            UpdateBtnsState();
        }

        private void UpdateBtnsState()
        {
            Match c_match = GameManager.Instance.networkManager.matchController.currentMatch;
            Network.Player c_player = GameManager.Instance.networkManager.matchController.currentPlayer;
            bool inMatch = GameManager.Instance.networkManager.matchController.inMatch;
            bool isLead = GameManager.Instance.networkManager.matchController.isLead;
       
            btnCreate.interactable = !inMatch;
            btnLeave.interactable = inMatch;
            btnStart.interactable = inMatch && isLead;//&& c_match?.players.Count == c_match?.maxPlayers;
            btnJoin.interactable = selectedItem != null && selectedItem.match.players.Count < selectedItem.match.maxPlayers && !inMatch;
        }
        #endregion
    }
}

