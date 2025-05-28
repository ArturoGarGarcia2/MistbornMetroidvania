using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyingSpot : MonoBehaviour{

    public Buyable buyingThing;
    public GameObject display;
    
    void Start(){
        display.GetComponent<SpriteRenderer>().sprite = buyingThing.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string ToString(){
        return buyingThing.ToString();
    }
}
