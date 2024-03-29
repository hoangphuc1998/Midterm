﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPun
{
    public float speed = 10f;

    public float destroyTime = 2f;

    public GameObject explosion;

    IEnumerator destroyBullet()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(this.gameObject);
    }

    void Start()
    {
        Physics2D.IgnoreLayerCollision(8, 9);
        Physics2D.IgnoreLayerCollision(9, 9);
        StartCoroutine("destroyBullet");
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject otherGO = other.gameObject;
        if (otherGO.tag.Equals("Player"))
        {
            if (otherGO.GetComponent<PhotonView>().IsMine)
            {
                other.gameObject.GetPhotonView().RPC("DecreaseHealth", RpcTarget.AllBuffered, 1);
            }
            Destroy(this.gameObject);
            GameObject effect = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
        else if (otherGO.tag.Equals("Walls"))
        {
            Destroy(this.gameObject);
            GameObject effect = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }
    
}
