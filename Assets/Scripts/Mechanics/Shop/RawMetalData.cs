using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevoRawMetal", menuName = "Buyable/RawMetal")]
public class RawMetalData : Buyable {
    public Metal metal;

    public override void Buy(PlayerData pd){
        FeruMetal fmDur = pd.GetFeruMetalIfEquipped((int)Metal.DURALUMIN);
        if(fmDur != null && fmDur.IsStoring()){
            pd.BuyRawMetal((int)metal,amount,(int)(price*1.4f));
        }else if(fmDur != null && fmDur.IsTapping()){
            pd.BuyRawMetal((int)metal,amount,(int)(price*.8f));
        }else{
            pd.BuyRawMetal((int)metal,amount,price);
        }
    }

    public override string ToString(){
        return amount+"*"+metal+" => "+price;
    }
}
