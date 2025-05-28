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

    private Coroutine infiniteTimeCoroutine;
    private bool infiniteTimeCoroutineRunning = false;

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

                    if (!infiniteTimeCoroutineRunning){
                        infiniteTimeCoroutine = StartCoroutine(InfinitedTimeCoroutine());
                    }
                }else{
                    // QUEMANDO CADMIO EL TIEMPO SE ACELERA Y SE CREA LA BURBUJA
                    CadmiumBubble.SetActive(true);
                    inCadBubble = true;
                    SetEverythingFalse();
                    speededUpTime = true;

                    if (infiniteTimeCoroutineRunning){
                        StopCoroutine(infiniteTimeCoroutine);
                        infiniteTimeCoroutineRunning = false;
                    }

                }
            }else{
                CadmiumBubble.SetActive(false);
                CadmiumBubble.transform.position = player.position;
                if (infiniteTimeCoroutineRunning){
                    StopCoroutine(infiniteTimeCoroutine);
                    infiniteTimeCoroutineRunning = false;
                }
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
            if (infiniteTimeCoroutineRunning){
                StopCoroutine(infiniteTimeCoroutine);
                infiniteTimeCoroutineRunning = false;
            }
        }
    }

    IEnumerator InfinitedTimeCoroutine(){
        infiniteTimeCoroutineRunning = true;

        while (true){
            AloMetal amCad = pd.GetAloMetalIfEquipped((int)Metal.CADMIUM);
            AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);

            if (amCad == null || !amCad.IsBurning() || amDur == null || !amDur.IsBurning()){
                infiniteTimeCoroutineRunning = false;
                yield break;
            }

            pd.ModifyFeruReservesInInfinited();
            yield return new WaitForSeconds(0.2f);
        }
    }


    void SetEverythingFalse(){
        speededUpTime = false;
        slowedDownTime = false;
        infinitedTime = false;
        stoppedTime = false;
    }
}
