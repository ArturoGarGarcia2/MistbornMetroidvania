using UnityEngine;
using System.Collections.Generic;

public class ParallaxManager : MonoBehaviour {
    public Transform player;
    private ParallaxZone currentZone;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("ParallaxZone")) {
            ParallaxZone newZone = other.GetComponentInChildren<ParallaxZone>();
            if (newZone != null && newZone != currentZone) {
                Debug.Log($"currentZone: {currentZone} / newZone: {newZone}");
                if (currentZone != null) currentZone.Deactivate();
                newZone.Activate();
                currentZone = newZone;
            }
        }
    }
}
