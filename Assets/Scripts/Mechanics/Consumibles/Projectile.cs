using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, Consumible{
    int stock;
    string name;
    string description;
    Sprite sprite;
    GameObject projectilePrefab;

    PlayerScript playerScript;

    public Projectile(int s, string n, string d, Sprite sp, GameObject prefab){
        stock = s;
        name = n;
        description = d;
        sprite = sp;
        projectilePrefab = prefab;

        playerScript = GameObject.FindObjectOfType<PlayerScript>();
    }

    public void SetPrefab(GameObject pf){
        projectilePrefab = pf;
    }

    public void Consume(){
        if (stock <= 0) return;
        if(name == "Moneda"){
            AloMetal amSte = playerScript.pd.GetAloMetalIfEquipped((int)Metal.STEEL);
            if(playerScript.pd.GetCoins()>=1 && amSte != null && amSte.IsBurning()){
                GameObject newProj = GameObject.Instantiate(
                    projectilePrefab,
                    playerScript.transform.position,
                    playerScript.transform.rotation
                );

                ProjectileBehaviour behaviour = newProj.GetComponent<ProjectileBehaviour>();
                behaviour.SetDirection(playerScript.facingRight ? Vector2.right : Vector2.left);

                playerScript.pd.BuySomething(1);
            }
        }else{
            GameObject newProj = GameObject.Instantiate(
                projectilePrefab,
                playerScript.transform.position,
                playerScript.transform.rotation
            );

            ProjectileBehaviour behaviour = newProj.GetComponent<ProjectileBehaviour>();
            behaviour.SetDirection(playerScript.facingRight ? Vector2.right : Vector2.left);

            stock--;
        }

    }

    public void AddStock() {
        stock += 1;
    }

    public string GetName() => name;
    public string GetDescription() => description;
    public int GetStock() => stock;
    public Sprite GetSprite() => sprite;

    public bool EqualsConsumible(Consumible other){
        if (other is not Projectile otherItem) return false;
        return name == otherItem.name && stock == otherItem.stock;
    }
}