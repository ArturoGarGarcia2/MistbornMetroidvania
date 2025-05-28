using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Consumible{
    void Consume();
    string GetName();
    string GetDescription();
    int GetStock();
    Sprite GetSprite();
    
    bool EqualsConsumible(Consumible other);
}
