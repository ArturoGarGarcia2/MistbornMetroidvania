using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour {
    public NPCData npcData;
    private NPC npc;

    PlayerScript playerScript;
    DialogManager dialogManager;
    public string estadoActual = "inicio";

    public bool requireGold = false;

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        dialogManager = FindObjectOfType<DialogManager>();

        npc = new NPC(npcData.npcName);

        foreach (var estado in npcData.estados){
            npc.AddFrases(estado.estado, estado.frases);
        }
    }

    void Update(){
        if (playerScript == null || dialogManager == null) return;
        AloMetal amGol = playerScript.pd.GetAloMetalIfEquipped((int)Metal.GOLD);
        if(requireGold && amGol != null && !amGol.IsBurning()){
            return;
        }

        if (playerScript.inNPC && playerScript.nearNPC == this.gameObject && playerScript.nextFrase){
            if (!dialogManager.IsActive()){
                dialogManager.StartDialog(npc.GetFrases(estadoActual),npcData,requireGold);
            } else {
                dialogManager.DisplayNextFrase();
            }
        }
    }

    public void StartDialogue(){
        AloMetal amGol = playerScript.pd.GetAloMetalIfEquipped((int)Metal.GOLD);
        if(requireGold && amGol != null && !amGol.IsBurning()){
            return;
        }
        DialogManager.instance.StartDialog(npcData.estados[0].frases,npcData,requireGold);
        if(npcData.eventName != null){
            GameEvents.NPCSpoken(npcData.eventName);
        }
    }
}