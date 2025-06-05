using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyBase : MonoBehaviour{
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

    bool inCadBubble = false;
    bool inBenBubble = false;

    public bool dead = false;
    public float distance;

    protected Transform player;
    public Image lifeBack;
    public Image lifeFill;
    
    CadmiumBendalloyManager CyBManager;

    protected PlayerScript playerScript;
    public PlayerData pd;
    
    public GameObject mentalIndicator;
    public float mentalIndicatorsRange = 20f;

    public bool isAttacking;
    public bool selected;
    public bool inflamed;
    public bool dampened;
    public bool canMentalInterfiere = true;
    public bool duralumined = false;

    private Coroutine inflamedRoutine;
    private Coroutine dampenedRoutine;

    protected Vector3 initialPosition;
    private Vector3 previousPosition;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Collider2D c2d;

    protected virtual void Start(){
        previousPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        c2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
        if (player == null){
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null){
                player = playerObj.transform;
            }
        }
        CyBManager = FindObjectOfType<CadmiumBendalloyManager>();

        playerScript = FindObjectOfType<PlayerScript>();
        pd = playerScript.pd;
        player = GameObject.FindWithTag("Player")?.transform;

        moveSpeed = baseMoveSpeed;
        attackRange = baseAttackRange;
        health = baseHealth;
    }

    protected virtual void Update(){
        if(dead){
            rb.bodyType = RigidbodyType2D.Static;
            c2d.isTrigger = true;
            return;
        }
        moveSpeed = baseMoveSpeed;
        damage = baseDamage;
        
        CheckBubbles();
        CheckMentalStates();

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

        if (spriteRenderer != null) {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange) {
                // Mira al jugador
                spriteRenderer.flipX = player.position.x < transform.position.x;
            } else {
                // Patrulla normalmente
                float deltaX = transform.position.x - previousPosition.x;

                if (deltaX > 0.01f) {
                    spriteRenderer.flipX = false;
                } else if (deltaX < -0.01f) {
                    spriteRenderer.flipX = true;
                }
            }

            previousPosition = transform.position;
        }

        HemaMetal hm = playerScript.pd.GetHemaMetalIfEquipped((int)Metal.BRONZE);
        if (hm != null){
            lifeBack.gameObject.SetActive(true);
            lifeFill.gameObject.SetActive(true);

            float ratio = (float)health / (float)baseHealth;
            lifeFill.fillAmount = ratio;
        }else{
            lifeBack.gameObject.SetActive(false);
            lifeFill.gameObject.SetActive(false);
        }

        AloMetal amZin = pd.GetAloMetalIfEquipped((int)Metal.ZINC);
        AloMetal amBra = pd.GetAloMetalIfEquipped((int)Metal.BRASS);

        if(
            (amZin != null && amZin.IsBurning()) ||
            (amBra != null && amBra.IsBurning())
        ){
            ShowMentalIndicator();
        }else{
            mentalIndicator.gameObject.SetActive(false);
            if(playerScript.nearMetalObjects.Contains(gameObject)){
                playerScript.nearMetalObjects.Remove(gameObject);
            }
        }

    }

    protected void CheckMentalStates(){
        if (inflamed && inflamedRoutine == null){
            inflamedRoutine = StartCoroutine(HandleMentalState(() => inflamed, val => inflamed = val, "Inflamed"));
        }

        if (dampened && dampenedRoutine == null){
            dampenedRoutine = StartCoroutine(HandleMentalState(() => dampened, val => dampened = val, "Dampened"));
        }
    }
    private IEnumerator HandleMentalState(System.Func<bool> getState, System.Action<bool> setState, string label){
        canMentalInterfiere = false;
        yield return new WaitForSeconds(duralumined ? 20f : 10f);
        setState(false);

        yield return new WaitForSeconds(10f);
        canMentalInterfiere = true;
        duralumined = true;

        if(label == "Inflamed") inflamedRoutine = null;
        if(label == "Dampened") dampenedRoutine = null;
    }
    void ShowMentalIndicator(){
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(selected){
            mentalIndicator.GetComponent<SpriteRenderer>().color = Color.yellow;
        }else{
            mentalIndicator.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (distanceToPlayer < mentalIndicatorsRange){
            mentalIndicator.gameObject.SetActive(true);
            if(!playerScript.nearEmotionObjects.Contains(gameObject)){
                playerScript.nearEmotionObjects.Add(gameObject);
            }
        }else{
            mentalIndicator.gameObject.SetActive(false);
            if(playerScript.nearEmotionObjects.Contains(gameObject)){
                playerScript.nearEmotionObjects.Remove(gameObject);
            }
        }
    }

    public virtual void TryAttack(){
        if (player == null || dead) return;

        distance = Vector2.Distance(player.position, transform.position);
        if (Time.time - lastAttackTime >= attackCooldown && distance <= attackRange){
            lastAttackTime = Time.time;
            OnAttackStart();
        }
    }

    protected virtual void OnAttackStart(){}
    protected virtual void Attack(){}

    protected virtual void Hurt(){}

    void CheckBubbles(){
        inCadBubble = false;
        inBenBubble = false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (var hit in hits){
            if (hit.CompareTag("CadmiumBubble"))
                inCadBubble = true;
            if (hit.CompareTag("BendalloyBubble"))
                inBenBubble = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag=="CadmiumBubble"){
            inCadBubble = true;
        }
        
        if(other.tag == "BendalloyBubble"){
            inBenBubble = true;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.tag=="CadmiumBubble"){
            inCadBubble = false;
        }
        
        if(other.tag == "BendalloyBubble"){
            inBenBubble = false;
        }
    }

    public virtual void ResetEnemy() {
        transform.position = initialPosition;
        health = baseHealth;
        dead = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        c2d.isTrigger = false;
        gameObject.SetActive(true);
    }
}
