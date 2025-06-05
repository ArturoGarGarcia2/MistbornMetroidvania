using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayerInZone : MonoBehaviour {
    public float parallaxFactor = 0.3f;
    private Transform cam;
    private Vector3 lastCamPosition;
    private bool isPlayerInside = false;

    void Start() {
        cam = Camera.main.transform;
        lastCamPosition = cam.position;
    }

    void LateUpdate() {
        if (!isPlayerInside) return;

        Vector3 delta = cam.position - lastCamPosition;
        transform.position += new Vector3(delta.x * parallaxFactor, 0, 0);
        lastCamPosition = cam.position;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayerInside = true;
            lastCamPosition = cam.position;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayerInside = false;
        }
    }
}
