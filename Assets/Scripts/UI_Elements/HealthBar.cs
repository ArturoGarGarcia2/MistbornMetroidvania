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

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        if (playerScript == null) {
            Debug.LogError("No se encontró el PlayerScript en la escena.");
            return;
        }
        maxHealth = (float)DatabaseManager.Instance.GetInt("SELECT salud_maxima FROM archivo WHERE id = 1;");
        currentHealth = PlayerPrefs.GetInt("Health");
    }

    void Update(){
        vialesVida = playerScript.vialesVida;

        foreach(var vial in vialesVida.Keys){
            int estadoVial = vialesVida[vial];
            if(estadoVial == -1){
                viales[vial-1].gameObject.SetActive(false);
            }else if(estadoVial == 0){
                viales[vial-1].gameObject.SetActive(true);
                viales[vial-1].sprite = vialesSprites[0];
            }else if(estadoVial == 1){
                viales[vial-1].gameObject.SetActive(true);
                viales[vial-1].sprite = vialesSprites[1];
            }
        }

        maxHealth = (float)DatabaseManager.Instance.GetInt("SELECT salud_maxima FROM archivo WHERE id = 1;");
        currentHealth = playerScript.actual_lifes;

        float fillAmount = currentHealth / maxHealth;

        if (healthFill != null)
            healthFill.fillAmount = fillAmount;

        if (healthEase != null && Mathf.Abs(healthEase.fillAmount - fillAmount) > 0.001f)
            healthEase.fillAmount = Mathf.Lerp(healthEase.fillAmount, fillAmount, lerpSpeed);
    }
}
