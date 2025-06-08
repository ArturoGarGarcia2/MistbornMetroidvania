using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KolossBoss : MonoBehaviour, Boss{
    public GameObject bossGameObject;
    public int maxHealth = 150;
    public int damage = 20;
    private int health;

    Vector2 initialPosition;
    public Transform player;
    float detectionRange = 15f;
    float baseAttackRange = 5f;
    float attackRange;
    public float baseMoveSpeed = 2f;
    public float moveSpeed;
    public string eventName;

    public GameObject cierre;

    public GameObject kolossAttack;

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

    public BossHealthUI bossHealthUI;

    private void Awake(){
        animator = GetComponentInChildren<Animator>();
        health = maxHealth;
    }

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        CyBManager = FindObjectOfType<CadmiumBendalloyManager>();
        initialPosition = transform.position;
        cierre.SetActive(false);
        pd = playerScript.pd;
        if(pd.IsEventAchievedByName(eventName)){
            Destroy(bossGameObject);
            return;
        }
        kolossAttack.SetActive(false);
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        
        moveSpeed = baseMoveSpeed;
        attackRange = baseAttackRange;
    }

    private void Update(){
        if(!bzm.playerInside) return;
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

        if(health<=0){
            cierre.SetActive(false);
        }

        if(hurted){
            GetComponent<SpriteRenderer>().color = Color.red;
        }else{
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        if(bossHealthUI != null){
            bossHealthUI.UpdateHealth(health);
        }
        pd = playerScript.pd;
        if (isAttacking || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange){
            if(canAttack)
                ChooseAttack();
        } else if (distance <= detectionRange){
            MoveTowardsPlayer();
        } else {
            animator.SetBool("Walking", false);
        }
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

        // Voltear sprite si hace falta
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
        kolossAttack.SetActive(true);
        yield return new WaitForSeconds(.1f);
        kolossAttack.SetActive(false);
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
        if(pd.IsEventAchievedByName(eventName)) return;
        transform.position = initialPosition;
        health = maxHealth;
        HideHealthBar();
    }

    public void ShowHealthBar(){
        bossHealthUI.Init(maxHealth);
        cierre.SetActive(true);
    }
    public void HideHealthBar(){
        bossHealthUI.Hide();
    }
}