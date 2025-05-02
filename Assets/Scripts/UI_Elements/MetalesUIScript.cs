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
        {"Duraluminio","Al+"},
        {"Atium","At"},
    };

    public Text MetalesText;
    public Text DBText;

    PlayerScript playerScript;
    public Dictionary<string,int> capacidadesAlo = new Dictionary<string, int>();
    public Dictionary<string,int> cantidadesAlo = new Dictionary<string, int>();
    public Dictionary<string,int> capacidadesFeru = new Dictionary<string, int>();
    public Dictionary<string,int> cantidadesFeru = new Dictionary<string, int>();

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
        {"Duraluminio", "#4682B4"},
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
        capacidadesAlo = playerScript.capacidadesAlo;
        cantidadesAlo = playerScript.cantidadesAlo;
        capacidadesFeru = playerScript.capacidadesFeru;
        cantidadesFeru = playerScript.cantidadesFeru;
        ActualizarMetales();
    }

    void ActualizarMetales(){
        string result = "";
        result += "<b>Met:  Alo  Feru</b>\n";

        foreach(var metal in cantidadesFeru.Keys){
            string nombre = metalesChiqui[metal].PadRight(4);
            string alo = cantidadesAlo[metal].ToString().PadLeft(4);
            string feru = cantidadesFeru[metal].ToString().PadLeft(4);

            string color = coloresMetales[metal];
            result += $"<color={color}>{nombre}</color>: {alo} - {feru}\n";
        }

        MetalesText.text = result;
        MetalesText.text = $"{(int)locuraManager.time}/{locuraManager.maxTime} \n{playerScript.faseActual}";
    }
}
