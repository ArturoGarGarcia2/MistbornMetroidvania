using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevoNPC", menuName = "NPC/NPCData")]
public class NPCData : ScriptableObject {
    public string npcName;
    public List<NPCEstado> estados;
}

[System.Serializable]
public class NPCEstado {
    public string estado;
    [TextArea(2, 5)]
    public List<string> frases;
}