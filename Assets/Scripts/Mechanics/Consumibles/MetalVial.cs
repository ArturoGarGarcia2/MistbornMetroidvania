using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MetalVial : Consumible {
    public bool roto;

    private List<MetalVialContent> _content = new List<MetalVialContent>();
    public List<MetalVialContent> content {
        get => _content;
        set {
            _content = value;
            UpdateDescription();
        }
    }

    string name;
    string description;
    Sprite spriteLleno;
    Sprite spriteVacio;
    Sprite spriteRoto;
    
    PlayerData pd;

    readonly string[] metales = {
        "Hierro", "Acero", "Estaño", "Peltre", "Zinc", "Latón",
        "Cobre", "Bronce", "Cadmio", "Bendaleo", "Oro", "Electro",
        "Cromo", "Nicrosil", "Aluminio", "Duralumín", "Atium"
    };

    public MetalVial(List<MetalVialContent> c, string n, string d, bool r, Sprite spL, Sprite spV, Sprite spR, PlayerData pdata){
        name = n;
        roto = r;

        spriteLleno = spL;
        spriteVacio = spV;
        spriteRoto = spR;

        pd = pdata;

        content = c;
    }

    private void UpdateDescription(){
        List<string> partes = new List<string>();
        foreach (MetalVialContent mvc in content){
            partes.Add($"{metales[((int)mvc.metal) - 1]}: {mvc.amount}");
        }
        description = string.Join(", ", partes);
    }

    public void Add(MetalVialContent mvc){
        content.Add(mvc);
        UpdateDescription();
    }

    public override string ToString(){
        string result = $"Vial {(roto ? "ROTO" : "")}[";
        foreach (MetalVialContent mvc in content){
            result += mvc + ", ";
        }
        result += "]";
        return result;
    }

    public void Consume(){
        pd.ConsumeVial(content);
        content.Clear();
        UpdateDescription();
    }

    public string GetName() => name;

    public string GetDescription() => description;
    
    public int GetStock() => -1;

    public Sprite GetSprite() => roto ? spriteRoto : content.Count == 0 ? spriteVacio : spriteLleno;

    public bool EqualsConsumible(Consumible other){
        if (other is not MetalVial otherVial) return false;
        if (content.Count != otherVial.content.Count) return false;

        for (int i = 0; i < content.Count; i++){
            if (content[i].metal != otherVial.content[i].metal || content[i].amount != otherVial.content[i].amount)
                return false;
        }

        return true;
    }
}

[System.Serializable]
public class MetalVialContent {
    public Metal metal;
    public int amount;

    public MetalVialContent(Metal m, int a){
        metal = m;
        amount = a;
    }

    public override string ToString(){
        return $"({metal}-{amount})";
    }
}