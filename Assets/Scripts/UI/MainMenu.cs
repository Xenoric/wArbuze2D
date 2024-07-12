using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Button btnStart;

    public void OnBtnStartClick()
    {
        btnStart.interactable = false;
        GameManager.Instance.networkManager.StartClient();
    }
}
