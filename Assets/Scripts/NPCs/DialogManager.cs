using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour {
    public static DialogManager instance; 

    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI npcNameText;
    public Image npcImage;

    private Queue<string> frases;
    private bool isActive = false;
    public float cooldownEntreFrases = .3f;
    public float cooldownEntreDialogos = 3.0f;
    private bool puedeMostrarFrase = true;
    private bool puedeHablar = true;

    private CanvasGroup canvasGroup;

    PlayerScript playerScript;

    private void Awake(){
        // Asegura que solo haya una instancia de DialogManager
        if (instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        frases = new Queue<string>();
        canvasGroup = dialogPanel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        dialogPanel.SetActive(false);

    }

    public void StartDialog(List<string> frasesList, NPCData npcData, bool requireGold){
        AloMetal amGol = playerScript.pd.GetAloMetalIfEquipped((int)Metal.GOLD);
        if(requireGold && amGol != null && !amGol.IsBurning()){
            return;
        }
        if(!puedeHablar) return;

        frases.Clear();
        foreach (string f in frasesList){
            frases.Enqueue(f);
        }

        npcNameText.text = npcData.npcName;

        dialogPanel.SetActive(true);
        playerScript.HideHUD();
        StartCoroutine(IniciarDialogoConFade());
    }

    private IEnumerator IniciarDialogoConFade(){
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 0.5f));
        DisplayNextFrase();
        isActive = true;
    }

    public void DisplayNextFrase(){
        if (!puedeMostrarFrase) return;

        if (frases.Count == 0){
            EndDialog();
            return;
        }

        StartCoroutine(FadeEntreFrases(frases.Dequeue()));
    }

    private IEnumerator FadeEntreFrases(string nuevaFrase){
        puedeMostrarFrase = false;

        // Fade out del texto
        yield return StartCoroutine(FadeText(dialogText, 1f, 0f, 0.2f));

        dialogText.text = nuevaFrase;

        // Fade in del texto
        yield return StartCoroutine(FadeText(dialogText, 0f, 1f, 0.2f));

        yield return new WaitForSeconds(cooldownEntreFrases);
        puedeMostrarFrase = true;
    }

    private IEnumerator CooldownCoroutine(){
        puedeMostrarFrase = false;
        yield return new WaitForSeconds(cooldownEntreFrases);
        puedeMostrarFrase = true;
    }

    public void EndDialog(){
        StartCoroutine(CerrarDialogoConFade());
        playerScript.inNPC = false;
        playerScript.ShowHUD();
    }

    private IEnumerator CerrarDialogoConFade(){
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, 0.5f));
        dialogPanel.SetActive(false);
        dialogText.text = "";
        isActive = false;
        playerScript.speed = 5;
        StartCoroutine(CooldownPostDialogCoroutine());
    }

    private IEnumerator CooldownPostDialogCoroutine(){
        puedeHablar = false;
        yield return new WaitForSeconds(cooldownEntreDialogos);
        puedeHablar = true;
    }

    public bool IsActive(){
        return isActive;
    }

    public void ShowDialog(List<string> frases){
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration){
        float elapsed = 0f;
        while (elapsed < duration){
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
    }

    private IEnumerator FadeText(TextMeshProUGUI tmp, float start, float end, float duration){
        float elapsed = 0f;
        while (elapsed < duration){
            elapsed += Time.deltaTime;
            tmp.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        tmp.alpha = end;
    }
}
