using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour{
    public Image healthFill;
    public Image healthEase;
    public float currentHealth;
    public float maxHealth;
    private float lerpSpeed = 0.01f;
    public Image[] viales;
    public Sprite[] vialesSprites;
    PlayerScript playerScript;
    public Dictionary<int, int> vialesVida = new Dictionary<int, int>();

    string faseActual;
    public Sprite[] fasesLocuraSprites;
    public Image healthBar;
    PlayerData pd;

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        if (playerScript == null) {
            Debug.LogError("No se encontró el PlayerScript en la escena.");
            return;
        }
        maxHealth = (float)DatabaseManager.Instance.GetInt("SELECT max_health FROM file WHERE id = 1;");
        currentHealth = PlayerPrefs.GetInt("Health");
    }

    void Update(){
        pd = playerScript.pd;
        vialesVida = playerScript.vialesVida;
        faseActual = pd.GetPhase();

        for (int i = 1; i <= viales.Length; i++){
            Debug.Log($"viales.Length: {viales.Length}");
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

        maxHealth = (float)DatabaseManager.Instance.GetInt("SELECT max_health FROM file WHERE id = 1;");
        currentHealth = playerScript.actual_lifes;

        float fillAmount = pd.GetHealth() / pd.GetMaxHealth();

        if (healthFill != null)
            healthFill.fillAmount = fillAmount;

        if (healthEase != null && Mathf.Abs(healthEase.fillAmount - fillAmount) > 0.001f)
            healthEase.fillAmount = Mathf.Lerp(healthEase.fillAmount, fillAmount, lerpSpeed);

        switch(faseActual){
            case "silencio":
                healthBar.color = Color.white;
                healthBar.sprite = fasesLocuraSprites[0];
                break;
            case "murmullos":
                healthBar.color = Color.white;
                healthBar.sprite = fasesLocuraSprites[1];
                break;
            case "ojos":
                healthBar.color = Color.white;
                healthBar.sprite = fasesLocuraSprites[2];
                break;
            case "mundo":
                healthBar.color = Color.white;
                healthBar.sprite = fasesLocuraSprites[3];
                break;
            case "dominio":
                healthBar.color = Color.white;
                healthBar.sprite = fasesLocuraSprites[4];
                break;
            case "extasis":
                healthBar.color = Color.white;
                healthBar.sprite = fasesLocuraSprites[5];
                break;
            case "declive":
                healthBar.color = Color.white;
                healthBar.color = Color.red;
                healthBar.sprite = fasesLocuraSprites[5];
                break;
            default:
                healthBar.color = Color.green;
                healthBar.sprite = fasesLocuraSprites[0];
                break;
        }
    }
}
