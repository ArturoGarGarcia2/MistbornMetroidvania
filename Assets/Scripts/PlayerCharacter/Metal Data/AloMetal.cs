using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AloMetal{
    Metal metal;
    int amount;
    int capacity;
    int burningRate;
    bool burning = false;

    public AloMetal(Metal m, int a, int c, int br){
        this.metal = m;
        this.amount = a;
        this.capacity = c;
        this.burningRate = br;
    }

    public Metal GetMetal() => metal;
    public int GetAmount() => amount;
    public int GetCapacity() => capacity;
    public int GetBurningRate() => burningRate;
    public bool IsBurning() => burning;

    public void SetAmount(int a){this.amount = a;}
    public void SetCapacity(int c){this.capacity = c;}
    public void SetBurningRate(int br){this.burningRate = br;}
    public void SetBurning(bool b){this.burning = b;}
    public void AlterBurning(){
        burning = !burning;
    }

    public void Recharge(int rechargeAmount){
        amount += rechargeAmount;
        if(amount > 1000){
            amount = 1000;
        }
    }

    public void RemoveAmount(int removeAmount){
        amount-=removeAmount;
        if(amount<0){
            amount=0;
        }
    }

    public void Burn(){
        if(!burning)return;
        if(amount>0){
            amount-=burningRate;
        }else{
            amount=0;
            burning=false;
        }
    }

    public void Burn(int newBurningRate){
        if(!burning)return;
        if(amount>0){
            amount-=newBurningRate;
        }else{
            amount=0;
            burning=false;
        }
    }

    public void AluminiumBurn(){
        if(amount>0){
            amount-=100;
        }else{
            amount=0;
            burning=false;
        }
    }

    public override string ToString(){
        return $"(ALO) {metal}: {amount}/{capacity}, BR: {burningRate}";
    }
}