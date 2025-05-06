using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyBase : MonoBehaviour
{
    public int baseDamage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    protected float lastAttackTime;
    public float moveSpeed = 5f;
    public int health = 10;
    public bool hurted = false;
    public float distance;
    
    public Text MetalesText;

    protected Transform player;


    protected virtual void Start(){
        player = GameObject.FindWithTag("Player")?.transform;
    }

    public virtual void TryAttack(){
        if (player == null) return;

        distance = Vector2.Distance(player.position, transform.position);
        MetalesText.text = $"distance: {distance}";
        MetalesText.text += $"\nattackRange: {attackRange}";
        if (Time.time - lastAttackTime >= attackCooldown && distance <= attackRange){
            lastAttackTime = Time.time;
            Attack();
        }
    }

    protected virtual void Attack(){}
}
