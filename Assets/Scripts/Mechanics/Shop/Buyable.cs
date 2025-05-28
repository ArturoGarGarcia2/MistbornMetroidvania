using UnityEngine;

public abstract class Buyable : ScriptableObject {
    public Sprite sprite;
    public int price;
    public bool renovable;
    public bool stackeable;
    public int amount;
    public string name;
    public string description;

    public abstract void Buy(PlayerData pd);
}
