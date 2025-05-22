using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour {
    public ShopData shopData;
    public GameObject shopItemUIPrefab;
    public Transform shopUIContainer;
    public PlayerData pd; // o tu sistema de base de datos

    void Start() {
        LoadShop();
    }

    void LoadShop() {
        // foreach (var item in shopData.items) {
        //     var go = Instantiate(shopItemUIPrefab, shopUIContainer);
        //     var ui = go.GetComponent<ShopItemUI>();
        //     ui.Setup(item, () => TryBuyItem(item));
        // }
    }

    void TryBuyItem(ShopItemData item) {
        if (pd.GetCoins() >= item.price) {
            pd.Buy(item.price);
            // ApplyItemEffect(item);
            // Feedback: sonido, animación, refrescar UI, etc.
        } else {
            // Mostrar mensaje "No tienes suficiente oro"
        }
    }

    // void ApplyItemEffect(ShopItemData item) {
    //     switch (item.type) {
    //         case ShopItemType.MetalAlomantico:
    //             pd.UnlockMetal(item.referenceId);
    //             break;
    //         case ShopItemType.MenteFeruquica:
    //             pd.UnlockMente(item.referenceId);
    //             break;
    //         case ShopItemType.Vial:
    //             pd.AddVials(item.referenceId, item.quantity);
    //             break;
    //         case ShopItemType.Proyectil:
    //             pd.AddAmmo(item.referenceId, item.quantity);
    //             break;
    //     }
    // }
}
