using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPun
{
    public float speed = 10f;

    public float destroyTime = 2f;
    IEnumerator destroyBullet()
    {
        yield return new WaitForSeconds(destroyTime);
        if (GetComponent<PhotonView>().IsMine)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        StartCoroutine("destroyBullet");
    }


}
