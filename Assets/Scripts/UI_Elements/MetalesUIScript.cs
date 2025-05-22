using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalesUIScript : MonoBehaviour
{
    Dictionary<string, string> metalesChiqui = new Dictionary<string, string>(){
        {"Hierro","Fe"},
        {"Acero","Fe+"},
        {"Estaño","Sn"},
        {"Peltre","Sn+"},
        {"Zinc","Zn"},
        {"Latón","Zn+"},
        {"Cobre","Cu"},
        {"Bronce","Cu+"},
        {"Cadmio","Cd"},
        {"Bendaleo","Cd+"},
        {"Oro","Au"},
        {"Electro","Au+"},
        {"Cromo","Cr"},
        {"Nicrosil","Cr+"},
        {"Aluminio","Al"},
        {"Duralumín","Al+"},
        {"Atium","At"},
    };

    public Text MetalesText;
    public Text DBText;

    PlayerScript playerScript;

    Dictionary<string, string> coloresMetales = new Dictionary<string, string>(){
        {"Hierro", "#4B4B4B"},
        {"Acero", "#A9A9A9"},
        {"Estaño", "#708090"},
        {"Peltre", "#B0C4DE"},
        {"Zinc", "#808000"},
        {"Latón", "#DAA520"},
        {"Cobre", "#B87333"},
        {"Bronce", "#CD7F32"},
        {"Cadmio", "#6A5ACD"},
        {"Bendaleo", "#9370DB"},
        {"Oro", "#FFD700"},
        {"Electro", "#FFB700"},
        {"Cromo", "#C0C0C0"},
        {"Nicrosil", "#D3D3D3"},
        {"Aluminio", "#ADD8E6"},
        {"Duralumín", "#4682B4"},
        {"Atium", "#5EF2DC"},
    };
    
    LocuraManager locuraManager;

    // Start is called before the first frame update
    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        if (playerScript == null) {
            Debug.LogError("No se encontró el PlayerScript en la escena.");
            return;
        }
        locuraManager = FindObjectOfType<LocuraManager>();
        if (locuraManager == null) {
            Debug.LogError("No se encontró el LocuraManager en la escena.");
            return;
        }
    }

    void Update(){
        // ActualizarMetales();
        ActualizarTexto();
    }

    void ActualizarTexto(){
        MetalesText.text = $"{(int)locuraManager.time}/{locuraManager.maxTime} \n{playerScript.pd.GetPhase()}";
        // MetalesText.text = $"Peso actual: {playerScript.pd.GetWeight()}";
    }
}
