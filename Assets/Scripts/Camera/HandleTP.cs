using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class HandleTP : MonoBehaviour {
    public Transform reciever;
    public Transform cameraReciever;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PlayerScript playerScript = other.GetComponent<PlayerScript>();
            if (playerScript != null && !playerScript.tping) {
                playerScript.StartTP(reciever, cameraReciever);
            }
        }
    }
}
