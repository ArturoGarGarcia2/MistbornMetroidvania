using System.Collections.Generic;

[System.Serializable]
public class NPC {
    public string name;
    public Dictionary<string, List<string>> frasesPorEstado;

    public NPC(string name){
        this.name = name;
        frasesPorEstado = new Dictionary<string, List<string>>();
    }

    public void AddFrases(string estado, List<string> frases){
        frasesPorEstado[estado] = frases;
    }

    public List<string> GetFrases(string estadoActual){
        if (frasesPorEstado.ContainsKey(estadoActual)){
            return frasesPorEstado[estadoActual];
        }
        return new List<string>() { "..." }; // Frase por defecto si no hay para ese estado
    }
}
