using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Manager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject player1Prefab;
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();

    }

    void SpawnPlayer()
    {
        if (PhotonNetwork.PlayerList.Length > 1)
            player = PhotonNetwork.Instantiate(player1Prefab.name, new Vector3(15.07f, -2.01f, 0), player1Prefab.transform.rotation);
        else
            player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(-14.63f, -2.01f, 0), playerPrefab.transform.rotation);
        if (!player.GetPhotonView().IsMine) return;
        Camera.main.GetComponent<CameraMovement>().SetTarget(player.transform);
    }

    private void Update()
    {
        
    }

    
}
