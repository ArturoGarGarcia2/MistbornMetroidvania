using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevoLaudano", menuName = "Buyable/Piedra")]
public class ProjectilePiedraData : Buyable{

    public override void Buy(PlayerData pd){
        pd.AddProjectilePiedra();
    }

    public override string ToString(){
        return "Piedra: "+price;
    }
}
