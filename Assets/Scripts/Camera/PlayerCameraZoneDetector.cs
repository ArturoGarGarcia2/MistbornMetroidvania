using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraZoneDetector : MonoBehaviour {
    public CameraController cameraController;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out CameraZone zone)) {
            cameraController.SetCurrentZone(zone);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.TryGetComponent(out CameraZone zone) && cameraController.CurrentZone == zone) {
            cameraController.SetCurrentZone(null);
        }
    }
}
