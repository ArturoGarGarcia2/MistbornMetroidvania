using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour{
    public Image healthFill;
    public Image healthEase;
    public float currentHealth = 0f;
    public float maxHealth;
    private float lerpSpeed = 0.02f;
    public Image[] viales;
    public Sprite[] vialesSprites;
    PlayerScript playerScript;
    public Dictionary<int, int> vialesVida = new Dictionary<int, int>();

    string faseActual;
    public Sprite[] healthBars;
    public Sprite[] feruGoldHealthBars;
    public Sprite[] brumas;
    public Image healthBar;
    public Image brumasImage;
    PlayerData pd;

    int maxMaxHealth = 120;

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        if (playerScript == null) {
            return;
        }
    }

    void Update(){
        pd = playerScript.pd;
        faseActual = pd.GetPhase();

        for (int i = 1; i <= viales.Length; i++){
            int vials = pd.GetVials();
            int remainingVials = pd.GetRemainingVials();

            if( i <= vials && i <= remainingVials ){
                viales[i-1].gameObject.SetActive(true);
                viales[i-1].sprite = vialesSprites[1];
            }else if( i <= vials && i > remainingVials ){
                viales[i-1].gameObject.SetActive(true);
                viales[i-1].sprite = vialesSprites[0];
            }else{
                viales[i-1].gameObject.SetActive(false);
            }
        }

        maxHealth = (float)pd.GetMaxHealth();
        currentHealth = (float)pd.GetHealth();
        
        FeruMetal fmGol = pd.GetFeruMetalIfEquipped((int)Metal.GOLD);
        if(fmGol != null && fmGol.IsStoring()){
            PutFeruGoldHealthBar();
        }else{
            PutHealthBar();
        }

        float fillAmount = currentHealth / maxMaxHealth;

        if (healthFill != null)
            healthFill.fillAmount = fillAmount;

        if (healthEase != null && Mathf.Abs(healthEase.fillAmount - fillAmount) > 0.001f)
            healthEase.fillAmount = Mathf.Lerp(healthEase.fillAmount, fillAmount, lerpSpeed);

        switch(faseActual){
            case "silencio":
                healthBar.color = Color.white;
                brumasImage.color = new Color(0f,0f,0f,0f);
                break;
            case "murmullos":
                healthBar.color = Color.white;
                brumasImage.color = Color.white;
                brumasImage.sprite = brumas[0];
                break;
            case "ojos":
                healthBar.color = Color.white;
                brumasImage.color = Color.white;
                brumasImage.sprite = brumas[1];
                break;
            case "mundo":
                healthBar.color = Color.white;
                brumasImage.color = Color.white;
                brumasImage.sprite = brumas[2];
                break;
            case "dominio":
                healthBar.color = Color.white;
                brumasImage.color = Color.white;
                brumasImage.sprite = brumas[3];
                break;
            case "extasis":
                healthBar.color = Color.white;
                brumasImage.color = Color.white;
                brumasImage.sprite = brumas[4];
                break;
            case "declive":
                brumasImage.color = Color.black;
                brumasImage.sprite = brumas[5];
                break;
            default:
                brumasImage.color = Color.green;
                brumasImage.sprite = brumas[0];
                break;
        }
    }

    void PutHealthBar(){
        switch(pd.GetMaxHealth()){
            case 30:
                healthBar.sprite = healthBars[0];
                break;
            case 60:
                healthBar.sprite = healthBars[1];
                break;
            case 90:
                healthBar.sprite = healthBars[2];
                break;
            case 120:
                healthBar.sprite = healthBars[3];
                break;
            default:
                break;
        }
    }

    void PutFeruGoldHealthBar(){
        switch(pd.GetMaxHealth()){
            case 10:
                healthBar.sprite = feruGoldHealthBars[0];
                break;
            case 20:
                healthBar.sprite = feruGoldHealthBars[1];
                break;
            case 30:
                healthBar.sprite = feruGoldHealthBars[2];
                break;
            case 40:
                healthBar.sprite = feruGoldHealthBars[3];
                break;
            default:
                break;
        }
    }
}
