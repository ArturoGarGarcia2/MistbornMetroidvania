using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tin : MonoBehaviour{
    PlayerScript playerScript;
    PlayerData pd;
    public Image mistImage;
    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
    }

    void Update(){
        pd = playerScript.pd;

        // AloMetal amTin = pd.GetAloMetalIfEquipped((int)Metal.TIN);
        // Color c = new Color(45/255f,45/255f,45/255f,100/255f);
        // if(amTin != null && amTin.IsBurning()){
        //     Debug.Log($"QUEMANDO ESTAÑO DESDE TinSenses");
        //     c = new Color(45/255f,45/255f,45/255f,50/255f);
        // }
        // // else{
        // //     Debug.Log($"NO QUEMANDO ESTAÑO DESDE TinSenses");
        // //     c = new Color(45/255f,45/255f,45/255f,100/255f);
        // // }
        // mistImage.color = c;
    }
}
