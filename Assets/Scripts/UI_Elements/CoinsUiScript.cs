using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsUiScript : MonoBehaviour
{
    int Coins;
    public Text CoinsText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Coins = PlayerPrefs.GetInt("Coins");
        CoinsText.text = Coins.ToString()+" C";
    }
}
