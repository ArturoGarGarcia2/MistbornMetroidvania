using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, Consumible{
    
    int stock;
    string name;
    string description;
    Sprite sprite;

    PlayerScript playerScript;
    PlayerData pd;

    public Projectile(int s, string n, string d, Sprite sp){
        stock = s;
        name = n;
        description = d;
        sprite = sp;

        playerScript = FindObjectOfType<PlayerScript>();
        pd = playerScript.pd;
    }

    public void Consume(){
    }
    public string GetName(){
        return name;
    }
    public string GetDescription(){
        return description;
    }
    public Sprite GetSprite(){
        return sprite;
    }

    public bool EqualsConsumible(Consumible other){
        if (other is not Projectile otherVial) return false;

        if (stock != otherVial.stock || name != otherVial.name){
            return false;
        }

        return true;
    }
}