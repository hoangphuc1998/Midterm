using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCrapWalk : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float start = 0.0f;
    public float end = 0.0f;

    public bool faceLeft = false;

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.position.y);
        if(faceLeft) {
            if(transform.position.x > start) {
                transform.position = transform.position - new Vector3(movementSpeed * Time.deltaTime, 0, 0);
            }
            else {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                faceLeft = false;
            }
        }
        else {
            if(transform.position.x < end) {
                transform.position = transform.position + new Vector3(movementSpeed * Time.deltaTime, 0, 0);
            }
            else {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                faceLeft = true;
            }
        }
    }
}
