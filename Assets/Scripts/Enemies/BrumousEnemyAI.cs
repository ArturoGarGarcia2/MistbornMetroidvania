using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrumousEnemyAI : MonoBehaviour {
    public Transform player;
    public float chaseRange = 5f;
    public float wanderRange = 5f;
    
    private BrumousEnemy enemy;
    private Vector2 initialPos;

    private bool movingRight = true;

    private void Start(){
        enemy = GetComponent<BrumousEnemy>();
        initialPos = transform.position;

        // Autoasignar jugador si no se asigna desde el Inspector
        if (player == null){
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null){
                player = playerObj.transform;
            }
        }
    }

    private void Update(){
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

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

        // Mover hacia la derecha o izquierda
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
