using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SimpleBrumousEnemy : MonoBehaviour {
    // Stats básicos
    public int baseDamage = 10;
    public int damage;

    public float baseAttackRange = 2f;
    public float attackRange;

    public float attackCooldown = 1.5f;
    public float attackDelay = .5f;

    protected float lastAttackTime;
    public float baseMoveSpeed = 7f;
    public float moveSpeed;
    public int baseHealth = 10;
    public int health;
    public bool physicalHurted = false;
    public bool invulnerable = false;

    public bool dead = false;

    protected Transform player;
    public Image lifeBack;
    public Image lifeFill;

    // Variables propias
    public Metal[] currentMetals;

    public BroncePulseManager bpm;
    public GameObject copperCloud;

    public GameObject mentalIndicator;
    public float allomanticEmotionRange = 20f;
    public bool mentalSelected;
    public bool canMentalInterfiere = true;
    public bool dampened;
    public bool inflamed;
    public bool mentalDuralumined;

    bool inCopperCloud = false;

    bool chromed = false;
    bool nicrosiled = false;
    bool leechedDuralumined = false;
    bool canGetChromed = true;

    bool isAttacking;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Collider2D c2d;

    Vector3 initialPosition;
    Vector3 previousPosition;

    PlayerScript playerScript;
    PlayerData pd;
    CadmiumBendalloyManager CyBManager;

    bool inCadBubble = false;
    bool inBenBubble = false;

    bool burningElectrum = false;
    bool burningAtium = false;

    public float allomanticRange = 10f;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        c2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        initialPosition = transform.position;
        previousPosition = transform.position;

        player = GameObject.FindWithTag("Player")?.transform;
        playerScript = FindObjectOfType<PlayerScript>();
        CyBManager = FindObjectOfType<CadmiumBendalloyManager>();

        moveSpeed = baseMoveSpeed;
        attackRange = baseAttackRange;
        health = baseHealth;

        // bpm = allomanticPulse.GetComponent<BroncePulseManager>();

        if (bpm != null) {
            bpm.startingMetals = currentMetals;
        }
        
        if (copperCloud != null) {
            copperCloud.SetActive(false);
        }
        mentalIndicator.SetActive(false);

        foreach(Metal m in currentMetals){
            if(m == Metal.ATIUM) burningAtium = true;
            if(m == Metal.ELECTRUM) burningElectrum = true;
        }
        canMentalInterfiere = true;
        canGetChromed = true;
    }

    void Update() {
        if (dead) {
            if (lifeBack != null) lifeBack.gameObject.SetActive(false);
            if (lifeFill != null) lifeFill.gameObject.SetActive(false);
            GetComponent<Animator>().SetBool("Dead",true);
            rb.bodyType = RigidbodyType2D.Static;
            c2d.isTrigger = true;
            return;
        }
        
        if(CyBManager.slowedDownTime && !inBenBubble){
            moveSpeed = baseMoveSpeed * .1f;
            attackRange = baseAttackRange * 1f;

        }else if(CyBManager.speededUpTime && !inCadBubble){
            moveSpeed = baseMoveSpeed * 10f;
            attackRange = baseAttackRange * 1f;

        }else if(CyBManager.stoppedTime){
            moveSpeed = baseMoveSpeed * 0f;
            attackRange = baseAttackRange * 0f;

        }else if(CyBManager.infinitedTime){
            moveSpeed = baseMoveSpeed * 100f;
            attackRange = baseAttackRange * 1f;

        }else{
            moveSpeed = baseMoveSpeed;
            attackRange = baseAttackRange * 1f;
        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if(distanceToPlayer > 30f) return;

        pd = playerScript.pd;

        moveSpeed = baseMoveSpeed;
        damage = baseDamage;

        // Manejo simple de movimiento: solo mirar al jugador
        if (player != null) {
            float distToPlayer = Vector2.Distance(transform.position, player.position);
            if (distToPlayer <= attackRange) {
                spriteRenderer.flipX = player.position.x < transform.position.x;
            } else {
                // patrullar (simple)
                float deltaX = transform.position.x - previousPosition.x;
                if (deltaX > 0.01f) spriteRenderer.flipX = false;
                else if (deltaX < -0.01f) spriteRenderer.flipX = true;
            }
            previousPosition = transform.position;
        }

        // Vida e indicador (simplificado)
        HemaMetal hmBro = pd.GetHemaMetalIfEquipped((int)Metal.BRONZE);
        if (hmBro != null) {
            if (lifeBack != null) lifeBack.gameObject.SetActive(true);
            if (lifeFill != null) {
                lifeFill.gameObject.SetActive(true);
                lifeFill.fillAmount = (float)health / baseHealth;
            }
        } else {
            if (lifeBack != null) lifeBack.gameObject.SetActive(false);
            if (lifeFill != null) lifeFill.gameObject.SetActive(false);
        }

        AloMetal amBro = pd.GetAloMetalIfEquipped((int)Metal.BRONZE);
        AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);
        if(amBro != null && amBro.IsBurning()){
            if(inCopperCloud){
                if(amDur != null && amDur.IsBurning()){
                    bpm.show = true;
                    if(!bpm.isPulsing) StartCoroutine(bpm.PlayPulses());
                }else{
                    bpm.show = false;
                    StopCoroutine(bpm.PlayPulses());
                    bpm.isPulsing = false;
                }
            }else{
                bpm.show = true;
                if(!bpm.isPulsing) StartCoroutine(bpm.PlayPulses());
            }
        }else{
            bpm.show = false;
            StopCoroutine(bpm.PlayPulses());
            bpm.isPulsing = false;
        }

        AloMetal amZin = pd.GetAloMetalIfEquipped((int)Metal.ZINC);
        AloMetal amBra = pd.GetAloMetalIfEquipped((int)Metal.BRASS);

        if(((amZin != null && amZin.IsBurning()) || (amBra != null && amBra.IsBurning())) && distanceToPlayer < allomanticEmotionRange){
            mentalIndicator.SetActive(true);
            if(!playerScript.nearEmotionObjects.Contains(gameObject)){
                playerScript.nearEmotionObjects.Add(gameObject);
            }
        }else{
            mentalIndicator.SetActive(false);
            if(playerScript.nearEmotionObjects.Contains(gameObject)){
                playerScript.nearEmotionObjects.Remove(gameObject);
            }
        }

        if(canMentalInterfiere){
            if(mentalSelected){
                mentalIndicator.GetComponent<SpriteRenderer>().color = Color.yellow;
            }else{
                mentalIndicator.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }else{
            mentalIndicator.GetComponent<SpriteRenderer>().color = Color.gray;
        }

        HandleLeeching(distanceToPlayer);
        HandleBrass();
        HandleMentalState();

        UseMistAbility();

        TryAttack();
    }

    float timeSinceLastChromiumRemoval = 0f;
    float chromiumRemovalInterval = 0.5f;

    void HandleBrass(){

    }

    void HandleLeeching(float distanceToPlayer){
        if (!currentMetals.Contains(Metal.CHROMIUM)) return;
        timeSinceLastChromiumRemoval += Time.deltaTime;

        if (timeSinceLastChromiumRemoval >= chromiumRemovalInterval && !chromed) {
            timeSinceLastChromiumRemoval = 0f;
            if (distanceToPlayer < allomanticRange) {
                foreach (AloMetal am in pd.GetAloMetals()) {
                    am.RemoveAmount(15);
                }
            }
        }
    }

    void HandleMentalState(){
        if(dampened){
            moveSpeed = (int)(baseMoveSpeed*0.7f);
            damage = (int)(baseDamage*0.7f);
        }
        if(inflamed){
            moveSpeed = (int)(baseMoveSpeed*1.3f);
            damage = (int)(baseDamage*1.3f);
        }

        if((dampened || inflamed) && canMentalInterfiere){
            canMentalInterfiere = false;
            StartCoroutine(MentalCooldown());
        }
    }

    IEnumerator MentalCooldown(){
        yield return new WaitForSeconds(mentalDuralumined ? 10f : 5f);
        mentalDuralumined = false;
        dampened = false;
        inflamed = false;
        yield return new WaitForSeconds(10f);
        canMentalInterfiere = true;
    }

    void FixedUpdate() {
        if (dead) return;
        if(pd == null) return;

        Hurted();
    }

    IEnumerator HandleNicrosil(){
        yield return new WaitForSeconds(leechedDuralumined ? 7f : 4f);
        nicrosiled = false;
        StartCoroutine(HandleChromium());
    }

    IEnumerator HandleChromium(){
        yield return new WaitForSeconds(leechedDuralumined ? 7f : 4f);
        chromed = false;
        leechedDuralumined = false;
        yield return new WaitForSeconds(5f);
        canGetChromed = true;
    }

    void Hurted() {
        if (burningAtium) {
            AloMetal amAti = pd.GetAloMetalIfEquipped((int)Metal.ATIUM);
            AloMetal amEle = pd.GetAloMetalIfEquipped((int)Metal.ELECTRUM);
            if ((amAti == null || !amAti.IsBurning()) && (amEle == null || !amEle.IsBurning())) {
                return;
            }
        }
        if (physicalHurted && !invulnerable) {
            physicalHurted = false;
            invulnerable = true;
            health -= playerScript.pd.GetDamage();
            StartCoroutine(InvulnerabilityCooldown());

            HandleLeecher();

            if (health <= 0) {
                dead = true;
                pd.AddCoins(7);
            }
        }
    }

    void HandleLeecher(){
        AloMetal amChr = pd.GetAloMetalIfEquipped((int)Metal.CHROMIUM);
        AloMetal amNic = pd.GetAloMetalIfEquipped((int)Metal.NICROSIL);
        AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);
        if(amChr != null && amChr.IsBurning() && canGetChromed){
            canGetChromed = false;
            amChr.Burn(300);
            chromed = true;
            if(amDur != null && amDur.IsBurning()){
                leechedDuralumined = true;
            }
            StartCoroutine(HandleChromium());
        }
        if(amNic != null && amNic.IsBurning() && canGetChromed){
            canGetChromed = false;
            amNic.Burn(400);
            nicrosiled = true;
            chromed = true;
            if(amDur != null && amDur.IsBurning()){
                leechedDuralumined = true;
            }
            StartCoroutine(HandleNicrosil());
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

    IEnumerator InvulnerabilityCooldown() {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(.5f);
        GetComponent<SpriteRenderer>().color = Color.white;
        invulnerable = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player_Attack")) {
            physicalHurted = true;
        }
        if (other.CompareTag("CopperCloud")) {
            inCopperCloud = true;
        }

        if(other.tag=="CadmiumBubble"){
            inCadBubble = true;
        }
        if(other.tag == "BendalloyBubble"){
            inBenBubble = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("CopperCloud")) {
            inCopperCloud = false;
        }
        
        if(other.tag=="CadmiumBubble"){
            inCadBubble = false;
        }
        if(other.tag == "BendalloyBubble"){
            inBenBubble = false;
        }
    }

    void UseMistAbility() {
        if (nicrosiled) {
            damage = 0;
            moveSpeed = 0f;
            attackRange = 0f;
            if (copperCloud != null) copperCloud.SetActive(false);
        } else if (chromed) {
            damage = baseDamage;
            moveSpeed = baseMoveSpeed;
            attackRange = baseAttackRange;
            if (copperCloud != null) copperCloud.SetActive(false);
        } else {
            foreach (Metal currentMetal in currentMetals) {
                if (currentMetal == Metal.PEWTER) {
                    damage = baseDamage*2;
                }
                if (currentMetal == Metal.COPPER) {
                    if (copperCloud != null) copperCloud.SetActive(true);
                    inCopperCloud = true;
                }
            }
        }
    }

    public void TryAttack() {
        if (player == null || dead || isAttacking) return;

        float dist = Vector2.Distance(player.position, transform.position);

        if (Time.time - lastAttackTime >= attackCooldown && dist <= attackRange) {
            lastAttackTime = Time.time;
            StartCoroutine(AttackWithDelay());
        }
    }

    IEnumerator AttackWithDelay() {
        isAttacking = true;

        if (GetComponent<Animator>() != null) {
            GetComponent<Animator>().SetTrigger("Attack");
        }
        yield return new WaitForSeconds(attackDelay);

        if (player != null && !dead && Vector2.Distance(player.position, transform.position) <= attackRange) {
            Attack();
        }

        yield return new WaitForSeconds(attackCooldown - attackDelay);
        isAttacking = false;
    }

    void Attack() {
        if (playerScript == null) return;
        if (pd == null) return;
        AloMetal amAti = pd.GetAloMetalIfEquipped((int)Metal.ATIUM);
        if((amAti != null && amAti.IsBurning()) && !burningElectrum ) return;

        ParryManager parryManager = FindObjectOfType<ParryManager>();
        if (parryManager != null && !parryManager.canParry) {
            playerScript.GetHurt(damage);
        }
    }

    public void ResetEnemy() {
        transform.position = initialPosition;
        health = baseHealth;
        dead = false;
        invulnerable = false;
        chromed = false;
        nicrosiled = false;
        canGetChromed = true;
        isAttacking = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        c2d.isTrigger = false;
        gameObject.SetActive(true);
        GetComponent<Animator>().SetBool("Dead",false);

        if (copperCloud != null) copperCloud.SetActive(false);
    }
}
