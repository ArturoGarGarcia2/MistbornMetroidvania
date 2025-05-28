using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinsUiScript : MonoBehaviour{
    public TextMeshProUGUI CoinsText;
    
    PlayerScript playerScript;
    PlayerData pd;

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
    }

    void Update(){
        pd = playerScript.pd;
        CoinsText.text = pd.GetCoins()+" €";
    }
}
