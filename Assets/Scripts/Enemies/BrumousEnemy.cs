using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrumousEnemy : EnemyBase, IBrumous {
    public Metal[] currentMetals;
    int damage;

    PlayerScript playerScript;
    public GameObject alomanticPulse;
    BroncePulseManager bpm;
    bool inCopperCloud = false;
    public GameObject copperCloud;

    protected override void Start() {
        base.Start();
        playerScript = FindObjectOfType<PlayerScript>();
        // alomanticPulse.gameObject.SetActive(false);
        bpm = alomanticPulse.GetComponent<BroncePulseManager>();
        bpm.startingMetals = currentMetals;
        copperCloud.gameObject.SetActive(false);
    }

    void Update() {
        TryAttack();
        UseMistAbility();

        PlayerData pd = playerScript.pd;
        AloMetal playerBronze = pd.GetAloMetalIfEquipped((int)Metal.BRONZE);
        AloMetal playerDuralumin = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);

        if(playerBronze != null){
            if(playerDuralumin != null && playerDuralumin.IsBurning()) {
                    bpm.show = true;
            }else{
                if(playerBronze.IsBurning() && !inCopperCloud){
                    bpm.show = true;
                }else{
                    bpm.show = false;
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D other){
        if (other.tag == "CopperCloud") {
            inCopperCloud = true;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if (other.tag == "CopperCloud") {
            inCopperCloud = false;
        }
    }

    protected override void Attack() {
        playerScript.pd.Hurt(damage);
    }

    public void UseMistAbility() {
        foreach(Metal currentMetal in currentMetals){
            if(currentMetal == Metal.PEWTER){
                damage = base.baseDamage*2;
            }
            if(currentMetal == Metal.COPPER){        
                copperCloud.gameObject.SetActive(true);
                inCopperCloud = true;
            }
        }
    }
}
