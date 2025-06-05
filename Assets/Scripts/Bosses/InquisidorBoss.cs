using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InquisidorBoss : MonoBehaviour, Boss{
    int maxHealth = 200;
    private int health;
    public int baseDamage = 30;
    public int damage;
    public string eventName;

    float detectionRange = 15f;
    float baseAttackRange = 3f;
    float attackRange;
    float baseMoveSpeed = 2f;
    float moveSpeed;
    
    public GameObject cierre;

    Vector2 initialPosition;
    public Transform player;
    public GameObject inquisidorAttack;
    
    public BossHealthUI bossHealthUI;

    public Sprite[] usableMetalsSymbols;
    public SpriteRenderer symbol;
    int index = 0;
    Metal[] usableMetals = {
        //SIN HACHA
        // Metal.STEEL,
        Metal.CHROMIUM,
        Metal.GOLD,

        //CON HACHA
        Metal.PEWTER,
        Metal.BRASS,
        Metal.ELECTRUM,
    };
    Metal previousMetal = Metal.NULL;
    public Metal currentMetal = Metal.PEWTER;
    float timeWithMetal = 5f;
    float timeWithMetalCounter = 0f;
    
    bool choosingMetal = false;

    public bool chromed;

    float healthSpan = 2f;
    float healthTime = 0f;

    private Animator animator;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool hurted = false;

    PlayerScript playerScript;
    PlayerData pd;
    CadmiumBendalloyManager CyBManager;
    public BossZoneManager bzm;

    bool inCadBubble = false;
    bool inBenBubble = false;

    void Awake(){
        var others = FindObjectsOfType<InquisidorBoss>();
        if (others.Length > 1){
            Debug.LogWarning("Destruyendo instancia duplicada del jefe");
            Destroy(gameObject);
            return;
        }
        animator = GetComponentInChildren<Animator>();
        health = maxHealth;
        Debug.Log($"[INQUISIDOR] Instancia creada: {GetInstanceID()}");
    }

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        CyBManager = FindObjectOfType<CadmiumBendalloyManager>();
        initialPosition = transform.position;
        pd = playerScript.pd;

        inquisidorAttack.SetActive(false);
        
        symbol.color = new Color(1f,1f,1f,0f);
        damage = baseDamage;
        moveSpeed = baseMoveSpeed;
        attackRange = baseAttackRange;
    }

    private void Update(){
        if(!bzm.playerInside) return;
        damage = baseDamage;
        moveSpeed = baseMoveSpeed;
        attackRange = baseAttackRange;
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

        symbol.sprite = usableMetalsSymbols[index];

        if(hurted){
            GetComponent<SpriteRenderer>().color = Color.red;
        }else{
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        if(bossHealthUI != null){
            Debug.Log($"[INQUISIDOR {GetInstanceID()}] {health}/{maxHealth}");
            bossHealthUI.UpdateHealth(health);
        }
        pd = playerScript.pd;
        if (isAttacking || player == null) return;

        if(!chromed && currentMetal == Metal.PEWTER){
            damage = baseDamage*3;
        }

        if(!chromed && currentMetal == Metal.GOLD){
            healthTime += Time.deltaTime;
            if(healthTime >= healthSpan){
                healthTime = 0f;
                health += 5;
                if(health > maxHealth){
                    health = maxHealth;
                }
            }
        }else{
            healthTime = 0f;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        
        if(!choosingMetal) timeWithMetalCounter += Time.deltaTime;

        if(!isAttacking && timeWithMetalCounter >= timeWithMetal){
            timeWithMetalCounter = 0f;
            ChooseMetal();
        }
        if(!choosingMetal){
            if (distance <= attackRange){
                if(canAttack)
                    ChooseAttack();
            } else if (distance <= detectionRange){
                MoveTowardsPlayer();
            } else {
                animator.SetBool("Walking", false);
            }
        }
    }

    void ChooseMetal(){
        choosingMetal = true;
        canAttack = true;
        Metal newMetal;
        do {
            newMetal = usableMetals[UnityEngine.Random.Range(0, usableMetals.Length)];
        } while (newMetal == currentMetal && usableMetals.Length > 1);

        currentMetal = newMetal;
        Metal metalToShow = previousMetal != Metal.NULL ? previousMetal : usableMetals[0];
        int prevIndex = Array.IndexOf(usableMetals, metalToShow);
        if (prevIndex >= 0){
            symbol.sprite = usableMetalsSymbols[prevIndex];
        }
        symbol.color = new Color(1f, 1f, 1f, 1f);
        animator.SetTrigger("ChooseMetal");
    }

    public void TriggerChooseMetal(){
        if (previousMetal != currentMetal){
            index = Array.IndexOf(usableMetals, currentMetal);
            symbol.sprite = usableMetalsSymbols[index];

            previousMetal = currentMetal;
            StartCoroutine(ChoosingMetalCooldown());
        } else {
            choosingMetal = false;
            previousMetal = currentMetal;
            symbol.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    IEnumerator ChoosingMetalCooldown(){
        yield return new WaitForSeconds(.5f);
        choosingMetal = false;
        animator.SetTrigger("ChooseMetal");
        symbol.color = new Color(1f, 1f, 1f, 0f);
    }
    
    public void GetHurt(){
        if(!hurted){
            hurted = true;
            TakeDamage(pd.GetDamage());
            if (health > 0){
                StartCoroutine(IT());
            }
        }
    }

    public void GetHurt(int damage){
        if(!hurted){
            hurted = true;
            TakeDamage(damage);
            if (health > 0){
                StartCoroutine(IT());
            }
        }
    }

    IEnumerator IT(){
        yield return new WaitForSeconds(.7f);
        hurted = false;
    }

    private void MoveTowardsPlayer(){
        animator.SetBool("Walking", true);

        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += new Vector3(direction.x, 0, 0) * moveSpeed * Time.deltaTime;

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void ChooseAttack(){
        isAttacking = true;
        animator.SetTrigger("Attack");
    }

    public void TriggerAttack(){
        canAttack = false;
        isAttacking = false;
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown(){
        inquisidorAttack.SetActive(true);
        yield return new WaitForSeconds(.1f);
        inquisidorAttack.SetActive(false);
        yield return new WaitForSeconds(2f);
        canAttack = true;
    }

    public void TakeDamage(int amount){
        health -= amount;
        if (health <= 0){
            Die();
        }
    }

    private void Die(){
        HideHealthBar();
        animator.SetBool("Dead",true);
        cierre.SetActive(false);
        pd.AchieveEventByName(eventName);
        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Player_Attack"){
            GetHurt();
        }

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

    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.CompareTag("Player")) {
            pd.Hurt(2);
        }
    }

    public void Reset(){
        HideHealthBar();
        cierre.SetActive(false);
        if(pd.IsEventAchievedByName(eventName)) return;
        transform.position = initialPosition;
        health = maxHealth;
        HideHealthBar();
    }

    public void ShowHealthBar(){
        Debug.Log("Mostrando la vida desde Inquisidor");
        bossHealthUI.Init(maxHealth);
        cierre.SetActive(true);
    }
    public void HideHealthBar(){
        bossHealthUI.Hide();
    }
}
