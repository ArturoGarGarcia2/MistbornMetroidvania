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
            return;
        }
    }

    // Update is called once per frame
    void Update(){
        HealthText.text = playerScript.pd.GetHealth()+" <3";
    }
}
