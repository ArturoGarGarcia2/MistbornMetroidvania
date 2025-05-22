using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Shops/ShopData")]
public class ShopData : ScriptableObject {
    public List<ShopItemData> items;
}

[System.Serializable]
public class ShopItemData {
    public string id;
    public string itemName;
    public string description;
    public Sprite icon;
    public int price;

    public ShopItemType type;
    public string referenceId; // puede ser el metalId, menteId, vialId, etc.

    public int quantity; // para cosas como viales o proyectiles
}

public enum ShopItemType {
    MetalAlomantico,
    MenteFeruquica,
    Vial,
    Proyectil
}
