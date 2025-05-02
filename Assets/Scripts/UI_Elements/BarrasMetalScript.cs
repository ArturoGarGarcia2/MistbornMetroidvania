using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrasMetalScript : MonoBehaviour{

    PlayerScript playerScript;
    public Dictionary<int, int> slotMetalAlo = new Dictionary<int, int>();
    public Dictionary<string,int> cantidadesAlo = new Dictionary<string, int>();
    public Dictionary<string,int> capacidadesAlo = new Dictionary<string, int>();

    public Sprite[] spritesBarrasMetales;
    public GameObject[] barrasMetales;
    
    PlayerData pd;
    
    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        if (playerScript == null) {
            Debug.LogError("No se encontró el PlayerScript en la escena.");
            return;
        }
    }

    void Update(){
        slotMetalAlo = playerScript.slotMetalAlo;
        cantidadesAlo = playerScript.cantidadesAlo;
        capacidadesAlo = playerScript.capacidadesAlo;
        pd = playerScript.pd;
        SetBarrasAlo();
    }

    private void SetBarrasAlo(){
        for(int i = 1; i <= pd.GetAloSlots().Length; i++){
            AloMetal metal = pd.GetAloMetalInSlot(i);

            if(metal == null){
                barrasMetales[i-1].SetActive(false);
                continue;
            }
            barrasMetales[i-1].SetActive(true);

            Image cantMetal = barrasMetales[i - 1].transform.GetChild(0).GetComponent<Image>();
            Image capMetal = barrasMetales[i - 1].transform.GetChild(1).GetComponent<Image>();

            capMetal.sprite = spritesBarrasMetales[(int)metal.GetMetal() - 1];
            cantMetal.fillAmount = (float)metal.GetAmount()/metal.GetCapacity();
        }
    }
}
