using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float followSpeed = 1f;
    public float focusZ = -0.5f;
    public float unFocusZ = -1.2f;
    public float focusY = 0.2f;
    public float unFocusY = 0f;
    public Camera cam;

    public Transform player;

    bool isFocusing = false;
    float focusX = 0;
    bool isFollowing = false;
    Transform followTarget;

    private void Awake() {

        cam = GetComponent<Camera>();

    }

    private void Update() {

        // If player is in a conversation.
        if (isFocusing) {

            if (focusX < -7.5f)
                focusX = -7.5f;
            else if (focusX > 7.5f)
                focusX = 7.5f;

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(focusX, focusY, focusZ), Time.deltaTime * followSpeed);
        // If player is scanning an NPC.    
        } else if (isFollowing) {

            float xval = followTarget.position.x;
            if (xval < -7.5f)
                xval = -7.5f;
            else if (xval > 7.5f)
                xval = 7.5f;

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(xval, unFocusY, focusZ), Time.deltaTime * followSpeed);
        // Regular camera movement.
        } else {

            float xval = player.position.x;
            if (xval < -7.5f)
                xval = -7.5f;
            else if (xval > 7.5f)
                xval = 7.5f;

            transform.position = Vector3.Lerp(transform.position, new Vector3(xval, unFocusY, unFocusZ), Time.deltaTime * followSpeed);

        }

    }

    public void Focus (float x) {

        focusX = x;
        isFocusing = true;

    }

    public void UnFocus() {

        isFocusing = false;

    }

    public void Follow (Transform target) {

        followTarget = target;
        isFollowing = true;

    }

    public void UnFollow () {

        followTarget = null;
        isFollowing = false;

    }

}
