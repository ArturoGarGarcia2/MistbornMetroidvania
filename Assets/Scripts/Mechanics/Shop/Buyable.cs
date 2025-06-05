using UnityEngine;

public abstract class Buyable : ScriptableObject {
    public Sprite sprite;
    public int price;
    public bool renovable;
    public bool unlockable;
    public MetalArt metalArt;
    public int amount;
    public string name;
    public string description;

    public abstract void Buy(PlayerData pd);
}
