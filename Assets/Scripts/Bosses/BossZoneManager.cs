using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZoneManager : MonoBehaviour {

    public MonoBehaviour bossBehaviour;
    public bool playerInside = false;

    private Boss boss;

    public GameObject salida;
    public GameObject cierre;

    private void Start() {
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerInside = true;
            boss = bossBehaviour as Boss;
            boss.ShowHealthBar();
            cierre.SetActive(true);
            salida.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerInside = false;
            boss.HideHealthBar();
            cierre.SetActive(false);
            salida.SetActive(true);
        }
    }
}
