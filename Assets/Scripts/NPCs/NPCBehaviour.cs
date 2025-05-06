using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour {
    public NPCData npcData;
    private NPC npc;

    PlayerScript playerScript;
    DialogManager dialogManager;
    public string estadoActual = "inicio";

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

        if (playerScript.inNPC && playerScript.nearNPC == this.gameObject && playerScript.nextFrase){
            if (!dialogManager.IsActive()){
                dialogManager.StartDialog(npc.GetFrases(estadoActual),npcData);
            } else {
                dialogManager.DisplayNextFrase();
            }
        }
    }

    public void StartDialogue(){
        Debug.Log("Iniciando diálogo del NPC");
        DialogManager.instance.StartDialog(npcData.estados[0].frases,npcData);
        if(npcData.eventName != null){
            GameEvents.NPCSpoken(npcData.eventName);
        }
    }
}