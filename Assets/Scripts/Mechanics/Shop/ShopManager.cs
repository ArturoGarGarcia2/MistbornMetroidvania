using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour{
    public GameObject shopPanel;
    bool showingPanel = false;
    Buyable buyingThing;
    int quantity = 1;
    bool canAfford;
    PlayerScript playerScript;

    Image itemSprite;
    TextMeshProUGUI itemTitle;
    TextMeshProUGUI itemDescription;

    TextMeshProUGUI actionQuestion;

    TextMeshProUGUI acceptText;
    Image acceptImage;
    TextMeshProUGUI cancelText;
    Image cancelImage;

    GameObject moreLess;
    TextMeshProUGUI moreText;
    Image moreImage;
    TextMeshProUGUI lessText;
    Image lessImage;

    void Start(){
        itemSprite = shopPanel.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
        itemTitle = shopPanel.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        itemDescription = shopPanel.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();

        acceptText = shopPanel.transform.GetChild(3).GetChild(1).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        cancelText = shopPanel.transform.GetChild(3).GetChild(1).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        acceptImage = shopPanel.transform.GetChild(3).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
        cancelImage = shopPanel.transform.GetChild(3).GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>();

        actionQuestion = shopPanel.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();
        moreLess = shopPanel.transform.GetChild(3).GetChild(2).gameObject;
        moreText = shopPanel.transform.GetChild(3).GetChild(2).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        lessText = shopPanel.transform.GetChild(3).GetChild(2).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        moreImage = shopPanel.transform.GetChild(3).GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>();
        lessImage = shopPanel.transform.GetChild(3).GetChild(2).GetChild(1).GetChild(0).GetComponent<Image>();

        shopPanel.SetActive(false);
        playerScript = FindObjectOfType<PlayerScript>();
    }

    void Update(){
        if(buyingThing != null){
            FeruMetal fmDur = playerScript.pd.GetFeruMetalIfEquipped((int)Metal.DURALUMIN);
            if(fmDur != null && fmDur.IsStoring()){
                canAfford = (buyingThing.price*quantity*1.4f)<=playerScript.pd.GetCoins();
            }else if(fmDur != null && fmDur.IsTapping()){
                canAfford = (buyingThing.price*quantity*.8f)<=playerScript.pd.GetCoins();
            }else{
                canAfford = (buyingThing.price*quantity)<=playerScript.pd.GetCoins();
            }
        }

        if(canAfford){
            acceptText.color = Color.white;
            acceptText.text = " Aceptar";
            acceptImage.color = Color.white;
        }else{
            acceptText.color = Color.red;
            acceptText.text = " Aceptar\n<size=60%><i>saldo insuficiente</i></size>";
            acceptImage.color = Color.red;
        }

        if(quantity > 1){
            lessText.color = Color.white;
            lessImage.color = Color.white;
        }else{
            lessText.color = Color.red;
            lessImage.color = Color.red;
        }
    }

    public void OpenPanel(Buyable thing){
        buyingThing = thing;
        playerScript.HideHUD();
        shopPanel.SetActive(true);

        itemSprite.sprite = buyingThing.sprite;
        itemTitle.text = buyingThing.name;
        itemDescription.text = buyingThing.description;

        if(buyingThing.renovable){
            moreLess.SetActive(true);
            moreText.text = $" Añadir {buyingThing.amount}";
            lessText.text = $" Quitar {buyingThing.amount}";
            FeruMetal fmDur = playerScript.pd.GetFeruMetalIfEquipped((int)Metal.DURALUMIN);
            if(fmDur != null && fmDur.IsStoring()){
                actionQuestion.text = $"¿Comprar {buyingThing.amount*quantity} por {buyingThing.price*quantity*1.4f} €?";
            }else if(fmDur != null && fmDur.IsTapping()){
                actionQuestion.text = $"¿Comprar {buyingThing.amount*quantity} por {buyingThing.price*quantity*.8f} €?";
            }else{
                actionQuestion.text = $"¿Comprar {buyingThing.amount*quantity} por {buyingThing.price*quantity} €?";
            }
        }else{
            moreLess.SetActive(false);
            FeruMetal fmDur = playerScript.pd.GetFeruMetalIfEquipped((int)Metal.DURALUMIN);
            if(fmDur != null && fmDur.IsStoring()){
                actionQuestion.text = $"¿Comprar por {buyingThing.price*1.4f} €?";
            }else if(fmDur != null && fmDur.IsTapping()){
                actionQuestion.text = $"¿Comprar por {buyingThing.price*.8f} €?";
            }else{
                actionQuestion.text = $"¿Comprar por {buyingThing.price} €?";
            }
        }

        showingPanel = true;
        Time.timeScale = 0f;
    }

    // TECLA ENTER
    public void onAcceptBuy(InputAction.CallbackContext context){
        if (context.performed && showingPanel){
            if(canAfford){
                for(int i = 0; i < quantity; i++){
                    buyingThing.Buy(playerScript.pd);
                }
                EndBuy();
            }
        }
    }
    // TECLA X
    public void onCancelBuy(InputAction.CallbackContext context){
        if (context.performed && showingPanel){
            EndBuy();
        }
    }
    // TECLA W
    public void onMoreStock(InputAction.CallbackContext context){
        if (context.performed && showingPanel && buyingThing.renovable){
            quantity++;
            FeruMetal fmDur = playerScript.pd.GetFeruMetalIfEquipped((int)Metal.DURALUMIN);
            if(fmDur != null && fmDur.IsStoring()){
                actionQuestion.text = $"¿Comprar {buyingThing.amount*quantity} por {buyingThing.price*quantity*1.4f} €?";
            }else if(fmDur != null && fmDur.IsTapping()){
                actionQuestion.text = $"¿Comprar {buyingThing.amount*quantity} por {buyingThing.price*quantity*.8f} €?";
            }else{
                actionQuestion.text = $"¿Comprar {buyingThing.amount*quantity} por {buyingThing.price*quantity} €?";
            }
        }
    }
    // TECLA S
    public void onLessStock(InputAction.CallbackContext context){
        if (context.performed && showingPanel && buyingThing.renovable){
            if(quantity != 1){
                quantity--;
                FeruMetal fmDur = playerScript.pd.GetFeruMetalIfEquipped((int)Metal.DURALUMIN);
                if(fmDur != null && fmDur.IsStoring()){
                    actionQuestion.text = $"¿Comprar {buyingThing.amount*quantity} por {buyingThing.price*quantity*1.4f} €?";
                }else if(fmDur != null && fmDur.IsTapping()){
                    actionQuestion.text = $"¿Comprar {buyingThing.amount*quantity} por {buyingThing.price*quantity*.8f} €?";
                }else{
                    actionQuestion.text = $"¿Comprar {buyingThing.amount*quantity} por {buyingThing.price*quantity} €?";
                }
            }
        }
    }

    void EndBuy(){
        shopPanel.SetActive(false);
        Time.timeScale = 1f;
        quantity = 1;
        playerScript.ShowHUD();
        showingPanel = false;
    }
}
