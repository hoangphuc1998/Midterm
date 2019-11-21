using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using System;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    public PhotonView photonview;
    public float moveSpeed = 5f;
    public float moveSmoothSpeed = 5f;
    public float rotationSmoothSpeed = 5f;
    private Rigidbody2D rb;
    Vector2 movement;
    private GameObject scenceCamera;
    private Camera cam;
    Vector2 mousePos;
    Vector3 smoothMove;
    Quaternion smoothRotation;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 1f;

    private Image playerFill, enemyFill, ammoFill;
    private int playerHealth = 20;
    private int enemyHealth = 20;
    public int maxHealth = 20;

    //Ammo reloading
    public int maxAmmo = 10;
    private int currentAmmo;
    private bool isReloading = false;
    public float reloadTime = 2f;
    
    //Win lose screen
    private GameObject winScreen, loseScreen;
    private GameObject winText, loseText;
    private bool endGame = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerFill = GameObject.Find("PlayerFill").GetComponent<Image>();
        enemyFill = GameObject.Find("EnemyFill").GetComponent<Image>();
        ammoFill = GameObject.Find("AmmoFill").GetComponent<Image>();
        
        if (photonView.IsMine)
        {
            currentAmmo = maxAmmo;
            scenceCamera = GameObject.Find("Main Camera");
            cam = Camera.main;

            winScreen = GameObject.Find("WinScreen");
            loseScreen = GameObject.Find("LoseScreen");
            winText = GameObject.Find("Win");
            loseText = GameObject.Find("Lose");
            winScreen.SetActive(false);
            loseScreen.SetActive(false);
            winText.SetActive(false);
            loseText.SetActive(false);
        }
    }
    [PunRPC]
    void endGameVictory()
    {
        
        Debug.Log(photonView.IsMine.ToString() + "Win");
        if (photonView.IsMine)
        {
            loseScreen.SetActive(false);
            loseText.SetActive(false);
            winScreen.SetActive(true);
            winText.SetActive(true);
        }
        endGame = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (endGame) return;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        
        if (photonView.IsMine)
        {
            if (playerHealth <= 0)
            {
                loseScreen.SetActive(true);
                loseText.SetActive(true);
                winScreen.SetActive(false);
                winText.SetActive(false);
                Debug.Log(this.gameObject.name + photonView.IsMine.ToString());
                if (this.gameObject.name.Equals("Player(Clone)"))
                {
                    Debug.Log("Player 1 Win");
                    GameObject.Find("Player 1(Clone)").GetComponent<PhotonView>().RPC("endGameVictory", RpcTarget.Others);
                }
                else
                {
                    Debug.Log("Player Win");
                    GameObject.Find("Player(Clone)").GetComponent<PhotonView>().RPC("endGameVictory", RpcTarget.Others);
                }
                endGame = true;
            }
            playerFill.fillAmount = (float)playerHealth / (float)maxHealth;
            ammoFill.fillAmount = (float)currentAmmo / (float)maxAmmo;
            ProcessInput();
        }
        else
        {
            enemyFill.fillAmount = (float)enemyHealth / (float)maxHealth;
            SmoothMovement();
        }
    }

    private void ProcessInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
        if (Input.GetButtonDown("Fire1"))
        {
            if (isReloading)
                return;
            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }
            currentAmmo--;
            photonView.RPC("Shoot", RpcTarget.All, firePoint.position, firePoint.rotation, firePoint.up);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    [PunRPC]
    void Shoot(Vector3 pos, Quaternion rot, Vector3 upPos)
    {
        GameObject bullet = Instantiate(bulletPrefab, pos, rot);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(upPos * bulletForce, ForceMode2D.Impulse);
    }


    private void SmoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * moveSmoothSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, smoothRotation, Time.deltaTime * rotationSmoothSpeed);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(playerHealth);
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
            smoothRotation = (Quaternion)stream.ReceiveNext();
            enemyHealth = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void DecreaseHealth(int damage)
    {
        if (playerHealth >= damage)
            playerHealth -= damage;
        else
            playerHealth = 0;
    }
}
