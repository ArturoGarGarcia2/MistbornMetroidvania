using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CadmiumBendalloyManager : MonoBehaviour
{

    public GameObject CadmiumBubble;
    public GameObject BendalloyBubble;

    public Transform player;
    PlayerScript playerScript;
    PlayerData pd;

    public bool inCadBubble;
    public bool inBenBubble;

    public bool speededUpTime;
    public bool slowedDownTime;
    public bool infinitedTime;
    public bool stoppedTime;

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
    }

    void Update(){
        pd = playerScript.pd;

        AloMetal amCad = pd.GetAloMetalIfEquipped((int)Metal.CADMIUM);
        AloMetal amBen = pd.GetAloMetalIfEquipped((int)Metal.BENDALLOY);
        AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);

        if(amCad != null && amCad.IsBurning() || amBen != null && amBen.IsBurning()){
            if (amCad != null && amCad.IsBurning()){
                if(amDur != null && amDur.IsBurning()){
                    // QUEMANDO CADMIO Y DURALUMINIO EL TIEMPO SE HIPERACELERA SIN BURBUJA
                    CadmiumBubble.SetActive(false);
                    CadmiumBubble.transform.position = player.position;
                    SetEverythingFalse();
                    infinitedTime = true;
                }else{
                    // QUEMANDO CADMIO EL TIEMPO SE ACELERA Y SE CREA LA BURBUJA
                    CadmiumBubble.SetActive(true);
                    inCadBubble = true;
                    SetEverythingFalse();
                    speededUpTime = true;
                }
            }else{
                CadmiumBubble.SetActive(false);
                CadmiumBubble.transform.position = player.position;
            }

            if (amBen != null && amBen.IsBurning()){
                if(amDur != null && amDur.IsBurning()){
                    // QUEMANDO BENDALEO Y DURALUMINIO EL TIEMPO SE DETIENE SIN BURBUJA
                    BendalloyBubble.SetActive(false);
                    BendalloyBubble.transform.position = player.position;
                    SetEverythingFalse();
                    stoppedTime = true;
                }else{
                    // QUEMANDO BENDALEO EL TIEMPO SE RALENTIZA
                    BendalloyBubble.SetActive(true);
                    inBenBubble = true;
                    SetEverythingFalse();
                    slowedDownTime = true;
                }
            }else{
                BendalloyBubble.SetActive(false);
                BendalloyBubble.transform.position = player.position;
            }

            if(amBen != null && amBen.IsBurning() && amCad != null && amCad.IsBurning()){
                SetEverythingFalse();
            }
        }else{
            if(amDur != null && amDur.IsBurning()) return;
            CadmiumBubble.SetActive(false);
            CadmiumBubble.transform.position = player.position;
            BendalloyBubble.SetActive(false);
            BendalloyBubble.transform.position = player.position;
            SetEverythingFalse();
        }
    }

    void SetEverythingFalse(){
        speededUpTime = false;
        slowedDownTime = false;
        infinitedTime = false;
        stoppedTime = false;
    }
}
