using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class PlayerHealth : MonoBehaviourPun
{
    public Image bar;
    private int health = 20;
    public int maxHealth = 20;
    public GameObject explosion;
    void Update()
    {
        bar.fillAmount = (float) health / (float) maxHealth;
    }
    [PunRPC]
    public void DecreaseHealth(int damage, Vector3 pos)
    {
        health -= damage;
        
        GameObject effect = Instantiate(explosion, pos, Quaternion.identity);
        Destroy(effect, 5f);
    }
}
