using System.Collections;
using UnityEngine;

public class ParryManager : MonoBehaviour{
    public SpriteRenderer sr;
    private PlayerData pd;
    public bool canParry = false;
    public bool inCooldown = false;

    private Coroutine parryCoroutine;

    private void Start(){
        pd = FindObjectOfType<PlayerScript>()?.pd;
        if (sr == null) sr = FindObjectOfType<PlayerScript>()?.GetComponent<SpriteRenderer>();
    }

    void Update(){
        pd = FindObjectOfType<PlayerScript>()?.pd;
    } 

    public void TryParry(){
        if (canParry || inCooldown) return;

        float parryWindow = pd != null ? pd.GetParryWindow() : 0.3f;
        if (parryCoroutine != null) StopCoroutine(parryCoroutine);
        parryCoroutine = StartCoroutine(ParryRoutine(parryWindow));
    }

    private IEnumerator ParryRoutine(float window){
        canParry = true;
        sr.color = Color.cyan;

        yield return new WaitForSeconds(window);

        canParry = false;
        sr.color = Color.gray;
        inCooldown = true;

        yield return new WaitForSeconds(0.3f);

        inCooldown = false;
        sr.color = Color.white;
    }

    public bool IsParrying(){
        return canParry;
    }

    public void OnSuccessfulParry(){
    }
}
