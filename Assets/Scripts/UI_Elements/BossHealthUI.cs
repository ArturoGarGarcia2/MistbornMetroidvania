using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour {
    public Image fillImage;
    private float maxHealth;

    void Start(){
        gameObject.SetActive(false);
    }

    public void Init(float maxHealth) {
        this.maxHealth = maxHealth;
        gameObject.SetActive(true);
    }

    public void UpdateHealth(float currentHealth) {
        if (maxHealth <= 0f) return;
        float fill = currentHealth / maxHealth;
        fillImage.fillAmount = fill;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
