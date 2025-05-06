using System.Collections;
using UnityEngine;
using System.Linq;

public class BroncePulseManager : MonoBehaviour {
    Animator anim;
    SpriteRenderer sr;
    Color transparent = Color.white;
    float timeBtwAnims = .7f;
    
    public Metal[] startingMetals;

    string fisico = "#3A6EA5";
    string mental = "#B03A48";
    string temporal = "#D4AF37";
    string espiritual = "#6A4FB6";
    string atium = "#00CFC8";

    public bool show = false; 

    void Start() {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        transparent.a = 0f;
        StartCoroutine(PlayPulses());
    }

    void Update(){
        if(!show){
            sr.color = transparent;
        }
    }

    IEnumerator PlayPulses() {
        while (true) {
            if(startingMetals.Contains(Metal.IRON))
                yield return StartCoroutine(PlayIron());
                
            if(startingMetals.Contains(Metal.STEEL))
                yield return StartCoroutine(PlaySteel());

            if(startingMetals.Contains(Metal.TIN))
                yield return StartCoroutine(PlayTin());

            if(startingMetals.Contains(Metal.PEWTER))
                yield return StartCoroutine(PlayPewter());

            if(startingMetals.Contains(Metal.ZINC))
                yield return StartCoroutine(PlayZinc());

            if(startingMetals.Contains(Metal.BRASS))
                yield return StartCoroutine(PlayBrass());

            if(startingMetals.Contains(Metal.COPPER))
                yield return StartCoroutine(PlayCopper());

            if(startingMetals.Contains(Metal.BRONZE))
                yield return StartCoroutine(PlayBronze());

            if(startingMetals.Contains(Metal.CADMIUM))
                yield return StartCoroutine(PlayCadmium());

            if(startingMetals.Contains(Metal.BENDALLOY))
                yield return StartCoroutine(PlayBendalloy());

            if(startingMetals.Contains(Metal.GOLD))
                yield return StartCoroutine(PlayGold());

            if(startingMetals.Contains(Metal.ELECTRUM))
                yield return StartCoroutine(PlayElectrum());

            if(startingMetals.Contains(Metal.CHROMIUM))
                yield return StartCoroutine(PlayChromium());

            if(startingMetals.Contains(Metal.NICROSIL))
                yield return StartCoroutine(PlayNicrosil());

            if(startingMetals.Contains(Metal.ALUMINIUM))
                yield return StartCoroutine(PlayAluminium());

            if(startingMetals.Contains(Metal.DURALUMIN))
                yield return StartCoroutine(PlayDuralumin());

            if(startingMetals.Contains(Metal.ATIUM))
                yield return StartCoroutine(PlayAtium());
        }
    }

    IEnumerator PlayIron(){
        Color color;
        if (ColorUtility.TryParseHtmlString(fisico, out color)) {
            yield return StartCoroutine(PlayExternalPushPulse(color));
        }
    }
    IEnumerator PlaySteel(){
        Color color;
        if (ColorUtility.TryParseHtmlString(fisico, out color)) {
            yield return StartCoroutine(PlayExternalPullPulse(color));
        }
    }
    IEnumerator PlayTin(){
        Color color;
        if (ColorUtility.TryParseHtmlString(fisico, out color)) {
            yield return StartCoroutine(PlayInternalPushPulse(color));
        }
    }
    IEnumerator PlayPewter(){
        Color color;
        if (ColorUtility.TryParseHtmlString(fisico, out color)) {
            yield return StartCoroutine(PlayInternalPullPulse(color));
        }
    }

    IEnumerator PlayZinc(){
        Color color;
        if (ColorUtility.TryParseHtmlString(mental, out color)) {
            yield return StartCoroutine(PlayExternalPushPulse(color));
        }
    }
    IEnumerator PlayBrass(){
        Color color;
        if (ColorUtility.TryParseHtmlString(mental, out color)) {
            yield return StartCoroutine(PlayExternalPullPulse(color));
        }
    }
    IEnumerator PlayCopper(){
        Color color;
        if (ColorUtility.TryParseHtmlString(mental, out color)) {
            yield return StartCoroutine(PlayInternalPushPulse(color));
        }
    }
    IEnumerator PlayBronze(){
        Color color;
        if (ColorUtility.TryParseHtmlString(mental, out color)) {
            yield return StartCoroutine(PlayInternalPullPulse(color));
        }
    }

    IEnumerator PlayCadmium(){
        Color color;
        if (ColorUtility.TryParseHtmlString(temporal, out color)) {
            yield return StartCoroutine(PlayExternalPushPulse(color));
        }
    }
    IEnumerator PlayBendalloy(){
        Color color;
        if (ColorUtility.TryParseHtmlString(temporal, out color)) {
            yield return StartCoroutine(PlayExternalPullPulse(color));
        }
    }
    IEnumerator PlayGold(){
        Color color;
        if (ColorUtility.TryParseHtmlString(temporal, out color)) {
            yield return StartCoroutine(PlayInternalPushPulse(color));
        }
    }
    IEnumerator PlayElectrum(){
        Color color;
        if (ColorUtility.TryParseHtmlString(temporal, out color)) {
            yield return StartCoroutine(PlayInternalPullPulse(color));
        }
    }

    IEnumerator PlayChromium(){
        Color color;
        if (ColorUtility.TryParseHtmlString(espiritual, out color)) {
            yield return StartCoroutine(PlayExternalPushPulse(color));
        }
    }
    IEnumerator PlayNicrosil(){
        Color color;
        if (ColorUtility.TryParseHtmlString(espiritual, out color)) {
            yield return StartCoroutine(PlayExternalPullPulse(color));
        }
    }
    IEnumerator PlayAluminium(){
        Color color;
        if (ColorUtility.TryParseHtmlString(espiritual, out color)) {
            yield return StartCoroutine(PlayInternalPushPulse(color));
        }
    }
    IEnumerator PlayDuralumin(){
        Color color;
        if (ColorUtility.TryParseHtmlString(espiritual, out color)) {
            yield return StartCoroutine(PlayInternalPullPulse(color));
        }
    }

    IEnumerator PlayAtium(){
        Color color;
        if (ColorUtility.TryParseHtmlString(atium, out color)) {
            yield return StartCoroutine(PlayAtiumPulse(color));
        }
    }



    IEnumerator PlayInternalPushPulse(Color color) {
        yield return StartCoroutine(PlayPulse("Internal", "InternalPulse", 0.7f, color));
    }
    IEnumerator PlayExternalPushPulse(Color color) {
        yield return StartCoroutine(PlayPulse("External", "ExternalPulse", 0.7f, color));
    }
    IEnumerator PlayInternalPullPulse(Color color) {
        yield return StartCoroutine(PlayPulse("Internal", "InternalPulse", 1.5f, color));
        yield return StartCoroutine(PlayPulse("Internal", "InternalPulse", 1.5f, color));
        yield return new WaitForSeconds(timeBtwAnims);
    }
    IEnumerator PlayExternalPullPulse(Color color) {
        yield return StartCoroutine(PlayPulse("External", "ExternalPulse", 1.5f, color));
        yield return StartCoroutine(PlayPulse("External", "ExternalPulse", 1.5f, color));
        yield return new WaitForSeconds(timeBtwAnims);
    }
    IEnumerator PlayAtiumPulse(Color color) {
        yield return StartCoroutine(PlayPulse("External", "ExternalPulse", 1.5f, color));
        yield return StartCoroutine(PlayPulse("Internal", "InternalPulse", 1.5f, color));
        yield return new WaitForSeconds(timeBtwAnims);
    }

    IEnumerator PlayPulse(string triggerName, string animStateName, float speedMultiplier, Color color) {
        sr.color = color;
        anim.speed = speedMultiplier;
        anim.SetTrigger(triggerName);

        yield return null; // Espera 1 frame para que el trigger se registre

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float duration = stateInfo.length / speedMultiplier;

        yield return new WaitForSeconds(duration);

        sr.color = transparent;
        anim.speed = 1f;
    }
}
