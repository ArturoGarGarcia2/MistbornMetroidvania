using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Hurt : MonoBehaviour
{

    public bool hurted = false;
    public int damage = 0;

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag=="Player_Attack"){
            hurted = true;
            damage = (int)(DatabaseManager.Instance.GetInt($"SELECT damage FROM archivo WHERE id = 1;") * (!HasClavo("Peltre") ? 1 : (HasClavo("Electro") ? 1.5 : 2)));
        }
    }

    bool HasClavo(string metal){
        return DatabaseManager.Instance.GetInt($"SELECT slot_h FROM metal_archivo WHERE archivo_id = 1 AND nombre = '{metal}';") != 0;
    }
}
