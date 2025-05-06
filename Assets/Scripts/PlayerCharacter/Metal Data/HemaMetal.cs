using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HemaMetal{
    Metal metal;
    bool active = true;

    public HemaMetal(Metal metal){
        this.metal = metal;
    }

    public Metal GetMetal() => metal;
    public bool IsActive() => active;

    public void Activate() => active = true;
    public void Deactivate() => active = false;
    public void SwitchIt() => active = !active;
    
    public override string ToString(){
        return $"(HEMA) {metal}";
    }
}