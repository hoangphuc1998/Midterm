using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CameraMovement : MonoBehaviour
{
    private Transform target;

    public float smoothSpeed = 0.125f;

    private Vector3 offset = new Vector3(0, 0, -1);

    private Camera mainCamera;
    float camVertExtent, camHorExtent;


    float leftBound, rightBound, bottomBound, topBound;
    // Start is called before the first frame update

    public void SetTarget(Transform target)
    {
        this.target = target;
        mainCamera = Camera.main;
        camVertExtent = mainCamera.orthographicSize;
        camHorExtent = mainCamera.aspect * camVertExtent;

        Bounds bounds = new Bounds();
        foreach (SpriteRenderer spriteBounds in GameObject.Find("BackGround").GetComponentsInChildren<SpriteRenderer>())
        {
            bounds.Encapsulate(spriteBounds.bounds);
        }

        leftBound = bounds.min.x + camHorExtent;
        rightBound = bounds.max.x - camHorExtent;
        topBound = bounds.max.y - camVertExtent;
        bottomBound = bounds.min.y + camVertExtent;
        offset = new Vector3(0, 0, -1);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null)
        {
            float camX = Mathf.Clamp(target.position.x + offset.x, leftBound, rightBound);
            float camY = Mathf.Clamp(target.position.y + offset.y, bottomBound, topBound);

            Vector3 desiredPosition = new Vector3(camX, camY, offset.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
