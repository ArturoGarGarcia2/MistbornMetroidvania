using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AloMetal{
    Metal metal;
    int amount;
    int capacity;
    int burningRate;
    bool burning = false;

    public AloMetal(Metal m, int a, int c, int vq){
        this.metal = m;
        this.amount = a;
        this.capacity = c;
        this.burningRate = vq;
    }

    public Metal GetMetal() => metal;
    public int GetAmount() => amount;
    public int GetCapacity() => capacity;
    public int GetBurningRate() => burningRate;
    public bool IsBurning() => burning;

    public void SetAmount(int a){this.amount = a;}
    public void SetCapacity(int c){this.capacity = c;}
    public void SetBurningRate(int vq){this.burningRate = vq;}
    public void SetBurning(bool b){this.burning = b;}
    public void AlterBurning(){
        burning = !burning;
    }

    public void Burn(){
        if(!burning)return;
        if(amount>0){
            amount-=burningRate;
        }else{
            amount=0;
        }
    }

    public override string ToString(){
        return $"(ALO) {metal}: {amount}/{capacity}, BR: {burningRate}";
    }
}