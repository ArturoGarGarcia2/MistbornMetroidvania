using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalesUIScript : MonoBehaviour
{
    string[] metales = {
        "Hierro",
        "Acero",
        "Estaño",
        "Peltre",
        "Zinc",
        "Latón",
        "Cobre",
        "Bronce",
        "Cadmio",
        "Bendaleo",
        "Oro",
        "Electro",
        "Cromo",
        "Nicrosil",
        "Aluminio",
        "Duraluminio",
        "Atium",
    };
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
        {"Atium","Atium"},
    };

    int hierro, acero, estaño, peltre, zinc, laton, cobre, bronce, cadmio, bendaleo, oro, electro, cromo, nicrosil, aluminio, duraluminio, atium;

    public Text MetalesText;
    public Text DBText;

    PlayerScript playerScript;
    public Dictionary<string,int> cantidadesFeru = new Dictionary<string, int>();
    public Dictionary<string,int> capacidadesFeru = new Dictionary<string, int>();
    public Dictionary<int,int> estadoSlotFeru = new Dictionary<int, int>();

    // Start is called before the first frame update
    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        if (playerScript == null) {
            Debug.LogError("No se encontró el PlayerScript en la escena.");
            return;
        }
    }

    void Update(){
        cantidadesFeru = playerScript.cantidadesFeru;
        capacidadesFeru = playerScript.capacidadesFeru;
        estadoSlotFeru = playerScript.estadoSlotFeru;
        ActualizarMetales();
    }

    // Coroutine para esperar que la base de datos esté lista
    IEnumerator EsperarBaseDeDatos(){
        // Espera hasta que la base de datos esté inicializada
        while (!DatabaseManager.Instance.IsDatabaseInitialized()){
            yield return null;
        }

        // Cuando la base de datos esté lista, actualizar los metales
        DBText.text = "Base de datos cargada.";
        ActualizarMetales();
    }

    void ActualizarMetales(){
        string result = "";
        int acu = 0;
        foreach(var metal in cantidadesFeru.Keys){
            result += $"{metalesChiqui[metal]}:  {cantidadesFeru[metal]}";
            if (acu % 2 == 1) {
                result += "\n";
            } else {
                result += " - ";
            }
            acu++;
        }

        MetalesText.text = result;
            // "Fe: " + hierro.ToString() + "\n" +
            // "Fe+: " + acero.ToString() + "\n" +
            // "Sn: " + estaño.ToString() + "\n" +
            // "Sn+: " + peltre.ToString() + "\n" +
            // "Cu: " + cobre.ToString() + "\n" +
            // "Cu+: " + bronce.ToString() + "\n" +
            // "Zn: " + zinc.ToString() + "\n" +
            // "Zn+: " + laton.ToString();
    }

    float ObtenerMetal(string nombreMetal){
        string query = "SELECT cantidad FROM metal_archivo WHERE metal_id = (SELECT id FROM metales WHERE nombre = @param0)";
        object resultado = DatabaseManager.Instance.ExecuteScalar(query, nombreMetal);
        return resultado != null ? System.Convert.ToSingle(resultado) : 0;
    }
}
