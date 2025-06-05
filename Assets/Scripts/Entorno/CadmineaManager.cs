using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CadmineaManager : MonoBehaviour{
    Collider2D c2d;
    public SpriteRenderer[] srs;
    public Transform player;
    int phase = 0;
    public Sprite[] states;
    float timeBetweenStates = 2f;
    float timeCounter = 0f;

    PlayerScript ps;
    PlayerData pd;
    void Start(){
        c2d = GetComponent<Collider2D>();
        ps = FindObjectOfType<PlayerScript>();
    }

    // Update is called once per frame
    void Update(){
        pd = ps.pd;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        AloMetal amCad = pd.GetAloMetalIfEquipped((int)Metal.CADMIUM);
        if(phase <= 5 && phase >= 0){
            foreach(SpriteRenderer sr in srs){
                sr.sprite = states[phase];
            }
        }

        timeCounter += Time.deltaTime;
        if(timeCounter > timeBetweenStates){
            timeCounter = 0f;
            if(distanceToPlayer < 7f && amCad != null && amCad.IsBurning()){
                if(phase < 5){
                    phase++;
                }
            }else{
                if(phase > 0){
                    phase--;
                }
            }
        }

        if(phase == 5){
            c2d.isTrigger = true;
            foreach(SpriteRenderer sr in srs){
                sr.color = new Color(1f,1f,1f,.5f);
            }
        }else{
            foreach(SpriteRenderer sr in srs){
                sr.color = new Color(1f,1f,1f,1f);
            }
            c2d.isTrigger = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.CompareTag("Player")) {
            pd.Hurt(2);
        }
    }
}
