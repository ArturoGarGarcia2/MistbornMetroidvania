using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZoneManager : MonoBehaviour {

    public MonoBehaviour bossBehaviour;
    public bool playerInside = false;

    private Boss boss;

    private void Start() {
        // if (bossBehaviour != null) {
        //     if (boss == null) {
        //         Debug.LogError("El componente asignado no implementa la interfaz Boss");
        //     }
        //     // instance = bossBehaviour.GetInstanceID();
        //     Debug.Log($"boss: {boss}");
        // }
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerInside = true;
            boss = bossBehaviour as Boss;
            Debug.Log($"boss: {boss}");
            boss.ShowHealthBar();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerInside = false;
            boss.HideHealthBar();
        }
    }
}
