using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using System;

public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    public PhotonView photonview;
    public float moveSpeed = 5f;
    public float moveSmoothSpeed = 10f;
    public float rotationSmoothSpeed = 10f;
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

    public Canvas canvas;
    void Start()
    {
        canvas.worldCamera = Camera.main;
        if (photonView.IsMine)
        {
            rb = GetComponent<Rigidbody2D>();
            scenceCamera = GameObject.Find("Main Camera");
            cam = Camera.main;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInput();
        }
        else
        {
            SmoothMovement();
        }
        canvas.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 0));
    }
    private void ProcessInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
        if (Input.GetButtonDown("Fire1"))
        {
            photonView.RPC("Shoot", RpcTarget.All, firePoint.position, firePoint.rotation, firePoint.up);
        }
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
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
            smoothRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
