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
    PlayerData pd;

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
        pd = playerScript.pd;
        for(int i = 1; i<=4; i++){
            int status = pd.GetFeruStatusInSlot(i);
            if (status == 1){
                slotsRellenar[i-1].color = Color.red;
            } else if (status == -1){
                slotsRellenar[i-1].color = Color.blue;
            } else {
                slotsRellenar[i-1].color = new Color(
                    156f/255f,
                    149f/255f,
                    173f/255f,
                    1f
                    );
            }
        }
        UpdateRueda();
    }

    void UpdateRueda(){
        for(int i = 1; i <= 4; i++){
            FeruMetal fm = pd.GetFeruMetalInSlot(i);
            if(fm!=null){
                slots[i-1].gameObject.SetActive(true);
                slotsRellenar[i-1].gameObject.SetActive(true);
                slots[i-1].sprite = spritesF[(int)fm.GetMetal()-1];
                slotsRellenar[i-1].sprite = spritesFRellenar[(int)fm.GetMetal()-1];
                slotsRellenar[i-1].fillAmount = (float)fm.GetAmount()/fm.GetCapacity();
            }else{
                slots[i-1].gameObject.SetActive(false);
                slotsRellenar[i-1].gameObject.SetActive(false);
            }
        }
    }
}
