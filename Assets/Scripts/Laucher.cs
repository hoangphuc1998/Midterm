using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Laucher : MonoBehaviourPunCallbacks
{
    public GameObject connectedScreen;
    public GameObject disconnectedScreen;
    public GameObject connectButton;
    public GameObject quitButton;

    private void Start()
    {

    }
    public void OnClick_Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        connectButton.SetActive(false);
        quitButton.SetActive(false);
        disconnectedScreen.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        connectButton.SetActive(false);
        quitButton.SetActive(false);
        if (disconnectedScreen.activeSelf)
        {
            disconnectedScreen.SetActive(false);
        }
        connectedScreen.SetActive(true);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
