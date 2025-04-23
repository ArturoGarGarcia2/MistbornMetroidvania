using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIScript : MonoBehaviour
{
    int Health;
    public Text HealthText;
    PlayerScript playerScript;
    
    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        if (playerScript == null) {
            Debug.LogError("No se encontró el PlayerScript en la escena.");
            return;
        }
    }

    // Update is called once per frame
    void Update(){
        Health = playerScript.actual_lifes;
        HealthText.text = Health.ToString()+" <3";
    }
}
