using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevoLaudano", menuName = "Buyable/Laudano")]
public class LaudanoData : Buyable{
    
    public override void Buy(PlayerData pd){
        pd.AddLaudanoBottle();
    }

    public override string ToString(){
        return "Láudano: "+price;
    }
}
