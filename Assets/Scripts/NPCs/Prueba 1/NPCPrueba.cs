using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCPrueba : MonoBehaviour {
    public NPC npc;
    PlayerScript playerScript;
    DialogManager dialogManager;
    public bool inNPC;

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        dialogManager = FindObjectOfType<DialogManager>();

        npc = new NPC("Pruebus");
        npc.AddFrases("inicio", new List<string>{
            "Esto es una frase de prueba",
            "Y esta es otra"
        });
    }

    void Update(){
        inNPC = playerScript.inNPC;
        bool nextFrase = playerScript.nextFrase;

        if (inNPC){
            playerScript.speed = 0;

            if (nextFrase){
                if (!dialogManager.IsActive()){
                    dialogManager.StartDialog(npc.GetFrases("inicio"));
                } else {
                    dialogManager.DisplayNextFrase();
                }
            }
        } else {
            playerScript.speed = 5;
        }
    }
}