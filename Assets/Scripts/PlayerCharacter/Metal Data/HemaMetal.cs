using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HemaMetal{
    Metal metal;
    int slot;

    public HemaMetal(Metal metal){
        this.metal = metal;
    }

    public Metal GetMetal() => metal;
    
    public override string ToString(){
        return $"(HEMA) {metal}";
    }
}