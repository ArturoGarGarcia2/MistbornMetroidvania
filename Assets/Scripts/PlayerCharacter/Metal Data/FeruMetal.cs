using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeruMetal{
    Metal metal;
    int amount;
    int capacity;
    int storingRate;
    int tappingRate;
    int status = 0;

    public FeruMetal(Metal m, int a, int c, int sr, int tr){
        this.metal = m;
        this.amount = a;
        this.capacity = c;
        this.storingRate = sr;
        this.tappingRate = tr;
    }

    public Metal GetMetal() => metal;
    public int GetAmount() => amount;
    public int GetCapacity() => capacity;
    public int GetStoringRate() => storingRate;
    public int GetTappingRate() => tappingRate;
    public int GetStatus() => status;
    public bool IsStoring() => status == 1;
    public bool IsTapping() => status == -1;

    public void SetAmount(int a){this.amount = a;}
    public void SetCapacity(int c){this.capacity = c;}
    public void SetStoringRate(int sr){this.storingRate = sr;}
    public void SetTappingRate(int tr){this.tappingRate = tr;}
    public void SetStatus(int s){this.status = s;}
    public void NextStatus(){
        status++;
        if(status >1){
            status=-1;
        }
    }

    public void UseMetalmind(){
        if(status == 1){
            amount+=storingRate;
            if(amount>capacity){
                amount=capacity;
                SetStatus(0);
            }
            Debug.Log($"Guardando en {this.GetMetal()}");
        }else if(status == -1){
            amount-=tappingRate;
            if(amount<0){
                amount=0;
                SetStatus(0);
            }
            Debug.Log($"Decantando en {this.GetMetal()}");
        }
    }

    public override string ToString(){
        return $"(FERU) {metal}: {amount}/{capacity}, SR: {storingRate}, TR: {tappingRate}";
    }
}
