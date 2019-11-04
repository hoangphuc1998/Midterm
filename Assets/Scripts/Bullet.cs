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
        Destroy(this.gameObject);
    }

    void Start()
    {
        StartCoroutine("destroyBullet");
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            other.gameObject.GetPhotonView().RPC("DecreaseHealth", RpcTarget.All, 1, transform.position);
            Destroy(this.gameObject);
        }
    }
    
}
