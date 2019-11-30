using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GemProcess : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject otherGO = other.gameObject;
        Debug.Log("GetGem");
        if (other.gameObject.tag.Equals("Player"))
        {
            if (otherGO.GetComponent<PhotonView>().IsMine)
            {
                otherGO.GetPhotonView().RPC("getGem", RpcTarget.AllBuffered);
            }
        }
    }

}
