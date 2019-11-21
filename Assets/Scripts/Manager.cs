using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Manager : MonoBehaviour
{
    public GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, playerPrefab.transform.position, playerPrefab.transform.rotation);
        if (!player.GetPhotonView().IsMine) return;
        Camera.main.GetComponent<CameraMovement>().SetTarget(player.transform);
    }
}
