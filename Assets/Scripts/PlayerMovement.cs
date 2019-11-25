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
    public float bulletForce = 10f;

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
    private bool endGame = false;

    //Gem system
    private int gemCount = 0;
    public GameObject gem;
    private bool isSpawning = false;
    public float respawnTime = 10f;
    private GameObject playerGem1, playerGem2, player1Gem1, player1Gem2;
    // Sprite list
    public Sprite[] sprites;
    [PunRPC]
    void setSprite(int pos)
    {
        GetComponent<SpriteRenderer>().sprite = sprites[pos];
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerFill = GameObject.Find("PlayerFill").GetComponent<Image>();
        enemyFill = GameObject.Find("EnemyFill").GetComponent<Image>();
        ammoFill = GameObject.Find("AmmoFill").GetComponent<Image>();
        if (name.Equals("Player(Clone)"))
        {
            playerGem1 = GameObject.Find("Player Gem 1");
            playerGem2 = GameObject.Find("Player Gem 2");
            playerGem1.SetActive(false);
            playerGem2.SetActive(false);
        }
        else
        {
            player1Gem1 = GameObject.Find("Player 1 Gem 1");
            player1Gem2 = GameObject.Find("Player 1 Gem 2");
            player1Gem1.SetActive(false);
            player1Gem2.SetActive(false);
        }
        if (photonView.IsMine)
        {
            photonView.RPC("setSprite", RpcTarget.AllBuffered, SceneManager.getRobotSprite());
            currentAmmo = maxAmmo;
            scenceCamera = GameObject.Find("Main Camera");
            cam = Camera.main;

            winScreen = GameObject.Find("WinScreen");
            loseScreen = GameObject.Find("LoseScreen");
            winScreen.SetActive(false);
            loseScreen.SetActive(false);
        }
    }
    [PunRPC]
    void endGameVictory()
    {
        
        Debug.Log(photonView.IsMine.ToString() + "Win");
        if (photonView.IsMine)
        {
            loseScreen.SetActive(false);
            winScreen.SetActive(true);
        }
        endGame = true;
    }

    [PunRPC]
    void endGameLoss()
    {

        Debug.Log(photonView.IsMine.ToString() + "Lose");
        if (photonView.IsMine)
        {
            loseScreen.SetActive(true);
            winScreen.SetActive(false);
        }
        endGame = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (endGame) return;

        //Update Gem
        if (name.Equals("Player(Clone)"))
        {
            if (gemCount >= 1)
            {
                playerGem1.SetActive(true);
            }
            if (gemCount >= 2)
            {
                playerGem2.SetActive(true);
            }
        }
        else
        {
            if (gemCount >= 1)
            {
                player1Gem1.SetActive(true);
            }
            if (gemCount >= 2)
            {
                player1Gem2.SetActive(true);
            }
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        
        if (photonView.IsMine)
        {
            //Lose game
            if (playerHealth <= 0)
            {
                loseScreen.SetActive(true);
                winScreen.SetActive(false);
                Debug.Log(this.gameObject.name + photonView.IsMine.ToString());
                if (this.gameObject.name.Equals("Player(Clone)"))
                {
                    GameObject.Find("Player 1(Clone)").GetComponent<PhotonView>().RPC("endGameVictory", RpcTarget.Others);
                }
                else
                {
                    GameObject.Find("Player(Clone)").GetComponent<PhotonView>().RPC("endGameVictory", RpcTarget.Others);
                }
                endGame = true;
            }
            //Win game
            if (gemCount >= 3)
            {
                loseScreen.SetActive(false);
                winScreen.SetActive(true);
                Debug.Log(this.gameObject.name + photonView.IsMine.ToString());
                if (this.gameObject.name.Equals("Player(Clone)"))
                {
                    GameObject.Find("Player 1(Clone)").GetComponent<PhotonView>().RPC("endGameLoss", RpcTarget.Others);
                }
                else
                {
                    GameObject.Find("Player(Clone)").GetComponent<PhotonView>().RPC("endGameLoss", RpcTarget.Others);
                }
                endGame = true;
            }

            playerFill.fillAmount = (float)playerHealth / (float)maxHealth;
            ammoFill.fillAmount = (float)currentAmmo / (float)maxAmmo;
            ProcessInput();
            //Spawn Gem
            if (isSpawning)
            {
                return;
            }
            if (GameObject.Find(gem.name +"(Clone)") == null && GameObject.Find(gem.name) == null && this.gameObject.name.Equals("Player(Clone)"))
            {
                Debug.Log("Spawn Gem");
                StartCoroutine(spawnGem());
            }
        }
        else
        {
            enemyFill.fillAmount = (float)enemyHealth / (float)maxHealth;
            SmoothMovement();
        }
    }

    IEnumerator spawnGem()
    {
        isSpawning = true;
        yield return new WaitForSeconds(respawnTime);
        photonview.RPC("SpawnGemNetwork", RpcTarget.All);
        isSpawning = false;

    }
    [PunRPC]
    public void SpawnGemNetwork()
    {
        Instantiate(gem, gem.transform.position, gem.transform.rotation);
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

        if (isReloading)
            return;
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        {
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

    [PunRPC]
    public void getGem()
    {
        Debug.Log(name + " Gem " + gemCount.ToString());
        gemCount++;
        GameObject gemGO = GameObject.Find(gem.name);
        if (gemGO == null)
            gemGO = GameObject.Find(gem.name + "(Clone)");
        if (gemGO != null)
            Destroy(gemGO);
    }
}
