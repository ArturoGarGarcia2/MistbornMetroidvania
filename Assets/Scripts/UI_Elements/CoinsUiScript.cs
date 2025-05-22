using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsUiScript : MonoBehaviour{
    public Text CoinsText;
    
    PlayerScript playerScript;
    PlayerData pd;

    // Start is called before the first frame update
    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
    }

    // Update is called once per frame
    void Update(){
        pd = playerScript.pd;
        CoinsText.text = pd.GetCoins()+" €";
    }
}
