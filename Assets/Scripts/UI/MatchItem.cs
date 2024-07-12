using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Warbuzz.Network;
using UnityEngine.UI;
using System;
using Mirror;

namespace Warbuzz.UI
{
    public class MatchItem : MonoBehaviour, IPointerClickHandler
    {
        public TMP_Text playersText;
        public Image imageSelected;
        public Image imageInMatch;

        public Action<MatchItem> pointerClick;

        public Match match;

        private bool _isSelected = false;

        public bool isSelected
        {
            get 
            { 
                return _isSelected; 
            }
            set 
            {
                _isSelected = value;
                imageSelected.gameObject.SetActive(_isSelected);

            }
        }
        private bool _inMatch = false;

        public bool inMatch
        {
            get
            {
                return _inMatch;
            }
            set
            {
                
                _inMatch = value;
                imageInMatch.gameObject.SetActive(_inMatch);

            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            pointerClick?.Invoke(this);
        }
    }
}

