using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyBase : MonoBehaviour
{
    public int baseDamage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float attackDelay = .5f;
    protected float lastAttackTime;
    public float baseMoveSpeed = 7f;
    public float moveSpeed;
    public int baseHealth = 10;
    public int health;
    public bool hurted = false;
    public bool invulnerable = false;

    bool inCadBubble = false;
    bool inBenBubble = false;

    public bool dead = false;
    public float distance;

    protected Transform player;
    public Image lifeBack;
    public Image lifeFill;
    
    CadmiumBendalloyManager CyBManager;

    PlayerScript playerScript;
    public PlayerData pd;

    // public Queue<EnemyShadowFrame> shadowFrames = new Queue<EnemyShadowFrame>();
    // public float recordInterval = 0.05f; // 20 fps
    // private float recordTimer = 0f;

    public bool isAttacking;

    protected virtual void Start(){
        CyBManager = FindObjectOfType<CadmiumBendalloyManager>();

        playerScript = FindObjectOfType<PlayerScript>();
        pd = playerScript.pd;
        player = GameObject.FindWithTag("Player")?.transform;

        moveSpeed = baseMoveSpeed;
        health = baseHealth;
    }

    protected virtual void Update(){
        CheckBubbles();

        if(CyBManager.slowedDownTime && !inBenBubble){
            moveSpeed = baseMoveSpeed * .1f;
        }else if(CyBManager.speededUpTime && !inCadBubble){
            moveSpeed = baseMoveSpeed * 10f;
        }else if(CyBManager.stoppedTime){
            moveSpeed = baseMoveSpeed * 0f;
        }else if(CyBManager.infinitedTime){
            moveSpeed = baseMoveSpeed * 100f;
        }else{
            moveSpeed = baseMoveSpeed;
        }

        // AloMetal amAti = playerScript.pd.GetAloMetalIfEquipped((int)Metal.ATIUM);
        // if (amAti != null && amAti.IsBurning()) {
        //     recordTimer += Time.deltaTime;
        //     if (recordTimer >= recordInterval) {
        //         recordTimer = 0f;
        //         shadowFrames.Enqueue(new EnemyShadowFrame(transform.position, transform.rotation, isAttacking));
        //         if (shadowFrames.Count > 1000) shadowFrames.Dequeue();
        //     }
        // }

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
}
