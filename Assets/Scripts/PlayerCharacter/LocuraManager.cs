using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocuraManager : MonoBehaviour{
    
    PlayerScript ps;

    public float time = 0f;

    int silencio = 90;
    int murmullos = 90+5400;
    int ojos = 90+5400+4500;
    int mundo = 90+5400+4500+3600;
    int dominio = 90+5400+4500+3600+2700;
    int extasis = 90+5400+4500+3600+2700+180;
    int declive = 90+5400+4500+3600+2700+180+300;
    public int maxTime = 0;
    PlayerScript playerScript;
    PlayerData pd;

    
    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        maxTime = declive;
        ps = FindObjectOfType<PlayerScript>();
        if (ps == null) {
            Debug.LogError("No se encontró el ps en la escena.");
            return;
        }
        time = ps.pd.GetPhaseTime();
    }

    public void ConsumeLaudano(){
        time -= 100;
        if(time < 0) time = 0;
    }

    void Update(){
        pd = playerScript.pd;
        float mult = 0f;
        if(time < silencio || time > dominio){
            mult = GetMultiplicadorByNumClavos(pd.GetNailNum()) == -1f ? -1f : 1f;
        }else{
            mult = GetMultiplicadorByNumClavos(pd.GetNailNum());
        }
        
        FeruMetal fm = pd.GetFeruMetalIfEquipped((int)Metal.ALUMINIUM);
        if(fm != null){
            mult = fm.IsStoring() ? (mult<0 ? mult*.5f : mult*2f) : fm.IsTapping() ? -2f : mult*1f;
        }

        time += Time.deltaTime * mult;

        if(time > 0 && time < silencio){
            // ps.faseActual = "silencio";
            ps.pd.SetPhase("silencio");
        }else if(time > silencio && time < murmullos){
            // ps.faseActual = "murmullos";
            ps.pd.SetPhase("murmullos");
        }else if(time > murmullos && time < ojos){
            // ps.faseActual = "ojos";
            ps.pd.SetPhase("ojos");
        }else if(time > ojos && time < mundo){
            // ps.faseActual = "mundo";
            ps.pd.SetPhase("mundo");
        }else if(time > mundo && time < dominio){
            // ps.faseActual = "dominio";
            ps.pd.SetPhase("dominio");
        }else if(time > dominio && time < extasis){
            // ps.faseActual = "extasis";
            ps.pd.SetPhase("extasis");
        }else if(time > extasis && time < declive){
            // ps.faseActual = "declive";
            ps.pd.SetPhase("declive");
        }

        ps.pd.SetPhaseTime((int)time);

        if(time <= 0){
            time = 0;
        }
        if(time >= declive){
            time = ojos;
        }
    }

    
    float GetMultiplicadorByNumClavos(int numClavos){
        return numClavos == 0 ? -1f :
                numClavos == 1 ? 1f :
                numClavos == 2|| numClavos == 3 ? 5f/3f :
                numClavos == 4 ? 2f :
                numClavos == 5|| numClavos == 6 ? 5f :
                10f;
    }
}