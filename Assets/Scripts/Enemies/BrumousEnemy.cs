using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrumousEnemy : EnemyBase, IBrumous {
    public Metal[] currentMetals;
    int damage;

    PlayerScript playerScript;
    ParryManager parryManager;
    public GameObject alomanticPulse;
    BroncePulseManager bpm;
    bool inCopperCloud = false;
    public GameObject copperCloud;

    protected override void Start() {
        base.Start();
        playerScript = FindObjectOfType<PlayerScript>();

        bpm = alomanticPulse.GetComponent<BroncePulseManager>();
        bpm.startingMetals = currentMetals;
        copperCloud.gameObject.SetActive(false);
    }
    protected override void Update(){ 
        base.Update();
    }

    void FixedUpdate() {
        if(dead){
            gameObject.SetActive(false);
            return;
        }
        parryManager = FindObjectOfType<ParryManager>();

        TryAttack();
        UseMistAbility();
        Hurted();

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

    void Hurted(){
        if(hurted && !invulnerable){
            invulnerable = true;
            health -= playerScript.pd.GetDamage();
            if(health <= 0){
                dead = true;
                pd.AddCoins(7);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other){
        if (other.tag == "CopperCloud") {
            inCopperCloud = true;
        }
        if (other.tag == "Player_Attack") {
            hurted = true;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if (other.tag == "CopperCloud") {
            inCopperCloud = false;
        }
        if (other.tag == "Player_Attack") {
            invulnerable = false;
        }
    }

    protected override void OnAttackStart() {
        StartCoroutine(AttackWithDelay());
    }

    private IEnumerator AttackWithDelay() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) {
            sr.color = Color.yellow;
        }

        yield return new WaitForSeconds(attackDelay);

        if (player != null && !dead && Vector2.Distance(player.position, transform.position) <= attackRange) {
            Attack();
        }

        if (sr != null) {
            sr.color = Color.white;
        }
    }


    protected override void Attack() {
        if(!parryManager.canParry){
            playerScript.pd.Hurt(damage);
        }
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
