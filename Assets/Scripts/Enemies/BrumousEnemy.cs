using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrumousEnemy : EnemyBase, IBrumous {
    public Metal[] currentMetals;

    ParryManager parryManager;
    public GameObject alomanticPulse;
    BroncePulseManager bpm;
    bool inCopperCloud = false;
    public GameObject copperCloud;
    bool chromed = false;
    bool nicrosiled = false;
    bool duralumined = false;
    bool canGetChromed = true;

    bool burningElectrum = false;
    bool burningAtium = false;

    protected override void Start() {
        base.Start();
        playerScript = FindObjectOfType<PlayerScript>();

        bpm = alomanticPulse.GetComponent<BroncePulseManager>();
        bpm.startingMetals = currentMetals;
        parryManager = FindObjectOfType<ParryManager>();
        copperCloud.gameObject.SetActive(false);
    }

    protected override void Update(){ 
        base.Update();
        if (inflamed){
            moveSpeed += baseMoveSpeed * .4f;
            damage += (int)(damage * .4f);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(242f/255f, 120f/255f, 111f/255f, 1f);
            if(duralumined)
                gameObject.GetComponent<SpriteRenderer>().color = new Color(242f/255f, 100f/255f, 100f/255f, 1f);

        }else if (dampened){
            moveSpeed -= baseMoveSpeed * .4f;
            damage -= (int)(damage * .4f);
            gameObject.GetComponent<SpriteRenderer>().color = new Color(135f/255f, 236f/255f, 245f/255f, 1f);
            if(duralumined)
                gameObject.GetComponent<SpriteRenderer>().color = new Color(100f/255f, 250f/255f, 250f/255f, 1f);

        }else{
            // gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        UseMistAbility();
    }

    void FixedUpdate() {
        if(dead){
            GetComponent<Animator>().SetBool("Dead",true);
            return;
        }

        TryAttack();
        Hurted();

        PlayerData pd = playerScript.pd;
        AloMetal playerBronze = pd.GetAloMetalIfEquipped((int)Metal.BRONZE);
        AloMetal playerDuralumin = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);

        if(playerBronze != null && !chromed && !nicrosiled){
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

    // void LateUpdate(){
    // }

    public void Hurted(){
        if (burningAtium) {
            AloMetal amAti = pd.GetAloMetalIfEquipped((int)Metal.ATIUM);
            AloMetal amEle = pd.GetAloMetalIfEquipped((int)Metal.ELECTRUM);
            if ((amAti == null || !amAti.IsBurning()) && (amEle == null || !amEle.IsBurning())) {
                return;
            }
        }
        
        if(physicalHurted && !invulnerable){
            physicalHurted = false;
            invulnerable = true;
            health -= playerScript.pd.GetDamage();
            StartCoroutine(InvulnerabilityCooldown());
            if(health <= 0){
                dead = true;
                pd.AddCoins(7);
            }
            AloMetal amChr = pd.GetAloMetalIfEquipped((int)Metal.CHROMIUM);
            AloMetal amNic = pd.GetAloMetalIfEquipped((int)Metal.NICROSIL);
            AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);
            if(canGetChromed && amChr != null && amChr.IsBurning() && amChr.GetAmount() >= 300){
                amChr.RemoveAmount(300);
                chromed = true;
                canGetChromed = false;
                if(amDur != null && amDur.IsBurning()){
                    duralumined = true;
                }
                StartCoroutine(HandleChromium());
            }
            if(canGetChromed && amNic != null && amNic.IsBurning() && amNic.GetAmount() >= 450){
                amNic.RemoveAmount(450);
                nicrosiled = true;
                canGetChromed = false;
                if(amDur != null && amDur.IsBurning()){
                    duralumined = true;
                }
                StartCoroutine(HandleNicrosil());
            }
        }
    }
    
    public void ProjectileHurted(int damage){
        if (burningAtium) {
            AloMetal amAti = pd.GetAloMetalIfEquipped((int)Metal.ATIUM);
            AloMetal amEle = pd.GetAloMetalIfEquipped((int)Metal.ELECTRUM);
            if ((amAti == null || !amAti.IsBurning()) && (amEle == null || !amEle.IsBurning())) {
                return;
            }
        }
        
        if(!invulnerable){
            invulnerable = true;
            health -= damage;
            StartCoroutine(InvulnerabilityCooldown());
            if(health <= 0){
                dead = true;
                pd.AddCoins(7);
            }
        }
    }

    IEnumerator InvulnerabilityCooldown(){
        yield return new WaitForSeconds(.5f);
        invulnerable = false;
    }

    IEnumerator HandleNicrosil(){
        yield return new WaitForSeconds(duralumined ? 5f : 3f);
        nicrosiled = false;
        chromed = true;
        StartCoroutine(HandleChromium());
    }
    IEnumerator HandleChromium(){
        yield return new WaitForSeconds(duralumined ? 8f : 5f);
        chromed = false;
        duralumined = false;
        StartCoroutine(HandleCanGetChromium());
    }
    IEnumerator HandleCanGetChromium(){
        yield return new WaitForSeconds(5f);
        canGetChromed = true;
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Player_Attack") {
            physicalHurted = true;
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

    protected override void OnAttackStart() {
        if (isAttacking) return;
        StartCoroutine(AttackWithDelay());
    }

    private IEnumerator AttackWithDelay() {
        isAttacking = true;

        GetComponent<Animator>().SetTrigger("Attack");
        yield return new WaitForSeconds(attackDelay);

        if (player != null && !dead && Vector2.Distance(player.position, transform.position) <= attackRange) {
            Attack();
        }

        yield return new WaitForSeconds(attackCooldown - attackDelay);
        isAttacking = false;
    }

    protected override void Attack() {
        AloMetal amAti = pd.GetAloMetalIfEquipped((int)Metal.ATIUM);
        if(amAti != null && amAti.IsBurning() && !(burningElectrum || burningAtium)) return;
        if(!parryManager.canParry){
            playerScript.GetHurt(damage);
        }
    }

    public void UseMistAbility() {
        if(nicrosiled){
            damage = 0;
            moveSpeed = 0f;
            attackRange = 0f;
            copperCloud.gameObject.SetActive(false);
            burningElectrum = false;
            burningAtium = false;
        }else if(chromed){
            damage = base.baseDamage;
            copperCloud.gameObject.SetActive(false);
            burningElectrum = false;
            burningAtium = false;
        }else{
            foreach(Metal currentMetal in currentMetals){
                if(currentMetal == Metal.PEWTER){
                    damage *= 2;
                }
                if(currentMetal == Metal.COPPER){        
                    copperCloud.gameObject.SetActive(true);
                    inCopperCloud = true;
                }
                if(currentMetal == Metal.ELECTRUM){
                    burningElectrum = true;
                }
                if(currentMetal == Metal.ATIUM){
                    burningAtium = true;
                }
            }
        }
    }

    public override void ResetEnemy() {
        base.ResetEnemy();
        GetComponent<Animator>().SetBool("Dead",false);
        invulnerable = false;
        chromed = false;
        nicrosiled = false;
        duralumined = false;
        canGetChromed = true;
        copperCloud?.SetActive(false);
    }
}
