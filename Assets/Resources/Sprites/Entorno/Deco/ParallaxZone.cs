using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxZone : MonoBehaviour {
    public float parallaxFactor = 0.5f;
    private Transform cam;
    private Vector3 initialPosition;
    private Vector3 initialCamPosition;
    private bool active = false;

    void Start() {
        cam = Camera.main.transform;
        initialPosition = transform.position;
    }

    void LateUpdate() {
        if (!active) return;

        Vector3 delta = cam.position - initialCamPosition;
        transform.position = initialPosition + new Vector3(delta.x * parallaxFactor, 0, 0);
    }

    public void Activate() {
        if (active) return;

        active = true;
        initialCamPosition = cam.position;
        initialPosition = transform.position;
    }

    public void Deactivate() {
        active = false;
    }
}
