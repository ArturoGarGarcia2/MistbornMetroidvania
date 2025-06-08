using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevoLaudano", menuName = "Buyable/Flecha")]
public class ProjectileFlechaData : Buyable{

    public override void Buy(PlayerData pd){
        pd.AddProjectileFlecha();
    }

    public override string ToString(){
        return "Flecha: "+price;
    }
}
