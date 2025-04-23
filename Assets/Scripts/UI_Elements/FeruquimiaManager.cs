using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeruquimiaManager : MonoBehaviour{

    public Image[] slots;
    public Image[] slotsRellenar;
    public Image[] crucetas;
    public Sprite[] spritesF;
    public Sprite[] spritesFRellenar;
    PlayerScript playerScript;
    public Dictionary<string,int> cantidadesFeru = new Dictionary<string, int>();
    public Dictionary<string,int> capacidadesFeru = new Dictionary<string, int>();
    public Dictionary<int,int> estadoSlotFeru = new Dictionary<int, int>();
    public Dictionary<int,int> slotMetalFeru = new Dictionary<int, int>();

    // Start is called before the first frame update
    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        if (playerScript == null) {
            Debug.LogError("No se encontró el PlayerScript en la escena.");
            return;
        }
    }

    // Update is called once per frame
    void Update(){
        cantidadesFeru = playerScript.cantidadesFeru;
        capacidadesFeru = playerScript.capacidadesFeru;
        estadoSlotFeru = playerScript.estadoSlotFeru;
        slotMetalFeru = playerScript.slotMetalFeru;
        UpdateRueda();
        foreach(var slot in estadoSlotFeru.Keys){
            if(estadoSlotFeru[slot] == 1){
                slotsRellenar[slot-1].color = Color.red;
                // slots[slot-1].color = Color.red;
                // crucetas[slot-1].color = Color.red;
            }else if(estadoSlotFeru[slot] == -1){
                slotsRellenar[slot-1].color = Color.blue;
                // slots[slot-1].color = Color.blue;
                // crucetas[slot-1].color = Color.blue;
            }else{
                slotsRellenar[slot-1].color = Color.white;
                // slots[slot-1].color = Color.white;
                // crucetas[slot-1].color = Color.white;
            }
        }
    }

    void UpdateRueda(){
        for(int i = 1; i <= 4; i++){
            int metalIdSlot = slotMetalFeru[i];
            string metalNombreSlot = DatabaseManager.Instance.GetString($"SELECT nombre FROM metal_archivo WHERE archivo_id = 1 AND metal_id = {metalIdSlot};");
            if(metalIdSlot <= 0){
                slots[i-1].gameObject.SetActive(false);
                slotsRellenar[i-1].gameObject.SetActive(false);
                crucetas[i-1].gameObject.SetActive(false);
            }else{
                slots[i-1].gameObject.SetActive(true);
                slotsRellenar[i-1].gameObject.SetActive(true);
                crucetas[i-1].gameObject.SetActive(true);
                slots[i-1].sprite = spritesF[metalIdSlot - 1];
                slotsRellenar[i-1].sprite = spritesFRellenar[metalIdSlot - 1];
                slotsRellenar[i-1].fillAmount = (float)cantidadesFeru[metalNombreSlot] / capacidadesFeru[metalNombreSlot];
            }
        }
    }
}
