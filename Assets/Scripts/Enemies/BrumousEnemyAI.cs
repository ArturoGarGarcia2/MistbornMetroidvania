using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrumousEnemyAI : MonoBehaviour {
    public Transform player;
    public float baseChaseRange = 5f;
    public float chaseRange;
    public float wanderRange = 5f;
    
    private SimpleBrumousEnemy enemy;
    private Vector2 initialPos;

    private bool movingRight = true;

    protected PlayerScript playerScript;
    public PlayerData pd;

    private void Start(){
        enemy = GetComponent<SimpleBrumousEnemy>();
        initialPos = transform.position;
        chaseRange = baseChaseRange;

        playerScript = FindObjectOfType<PlayerScript>();

        if (player == null){
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null){
                player = playerObj.transform;
            }
        }
    }

    private void Update(){
        if(gameObject.GetComponent<SimpleBrumousEnemy>().dead) return;
        pd = playerScript.pd;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(distanceToPlayer > 30f) return;

        AloMetal amCop = pd.GetAloMetalIfEquipped((int)Metal.COPPER);
        AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);
        if(amCop != null && amCop.IsBurning()){
            if(amDur != null && amDur.IsBurning()){
                chaseRange = baseChaseRange/4;
            }else{
                chaseRange = baseChaseRange/2;
            }
        }else{
            chaseRange = baseChaseRange;
        }

        if (distanceToPlayer < chaseRange){
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * enemy.moveSpeed * Time.deltaTime);
            enemy.TryAttack();
        } else {
            Patrol();
        }
    }

    private void Patrol(){
        float patrolSpeed = enemy.moveSpeed * 0.5f;
        Vector2 newPos = transform.position;

        if (movingRight){
            newPos += Vector2.right * patrolSpeed * Time.deltaTime;
            if (newPos.x > initialPos.x + wanderRange){
                movingRight = false;
            }
        } else {
            newPos += Vector2.left * patrolSpeed * Time.deltaTime;
            if (newPos.x < initialPos.x - wanderRange){
                movingRight = true;
            }
        }

        transform.position = newPos;
    }
}
