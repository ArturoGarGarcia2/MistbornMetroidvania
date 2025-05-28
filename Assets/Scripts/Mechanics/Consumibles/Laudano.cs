using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laudano : MonoBehaviour, Consumible {

    private int _stock;
    public int Stock {
        get => _stock;
        set {
            _stock = value;
            UpdateDescription();
        }
    }

    string name;
    string description;
    string baseDescription = "Vino blanco, azafrán, clavo, canela (y algunas sorpresitas) además de opio, crean un brebaje capaz de calmar las mentes más inquietas e intranquilas";
    Sprite sprite;

    LocuraManager lm;
    PlayerScript playerScript;
    PlayerData pd;

    public Laudano(int s, string n, string d, Sprite sp) {
        name = n;
        baseDescription = d; // usa d como baseDescription por si la pasas personalizada
        sprite = sp;

        lm = FindObjectOfType<LocuraManager>();
        playerScript = FindObjectOfType<PlayerScript>();
        pd = playerScript.pd;

        Stock = s; // usa la propiedad para que actualice la descripción
    }

    private void UpdateDescription() {
        description = $"{baseDescription} (Stock: {Stock})";
    }

    public void AddStock() {
        Stock += 1;
    }

    public void Consume() {
        if (Stock > 0) {
            lm.ConsumeLaudano();
            Stock -= 1;
        }
    }

    public string GetName() => name;
    public string GetDescription() => description;
    public int GetStock() => Stock;
    public Sprite GetSprite() => sprite;

    public bool EqualsConsumible(Consumible other) {
        if (other is not Laudano otherVial) return false;
        return Stock == otherVial.Stock && name == otherVial.name;
    }
}
