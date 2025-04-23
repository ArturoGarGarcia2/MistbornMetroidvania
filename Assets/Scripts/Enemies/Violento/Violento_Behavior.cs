using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Violento_Behavior : MonoBehaviour, IAttacker
{

    [Header("Bordes Movimiento")]
    [SerializeField] GameObject Edge1;
    [SerializeField] GameObject Edge2;

    [SerializeField] GameObject Detection_Edge1;
    [SerializeField] GameObject Detection_Edge2;

    [Header("Movimiento")]
    [SerializeField] GameObject Enemy;
    [SerializeField] [Range(.5f,5)] float speed;
    GameObject player;
    PlayerScript p;

    Rigidbody2D rb;
    [SerializeField] bool right = false;
    //bool player_inside;

    bool right_check;
    //bool left_check;


    [Header("Vida")]
    [SerializeField] [Range(1,5)] int lifes = 5;
    int actual_lifes;
    //bool hurted = false;
    [SerializeField] [Range(0,5)] float hurted_delay = 1;
    Enemy_Hurt EH;
    float targetTime;
    SpriteRenderer sr;
    bool hurt_start = false;
    bool dead = false;
    [SerializeField] [Range(5,25)] float hurt_thrust;
    float short_term_thrust;

    [Header("Attacks")]
    [SerializeField] [Range(0,5)] float attack_range = 1;
    [SerializeField] [Range(0,5)] float attack_delay = 1;
    [SerializeField] [Range(0,10)] float attack_cooldown = 1;
    [SerializeField] int attack_damage = 4;
    float attack_duration;
    float cooldown;
    [SerializeField] GameObject AttackBox;
    float player_distance;
    bool ready_to_attack = true;

    public int GetDamage(){
        return attack_damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        sr = Enemy.GetComponent<SpriteRenderer>();
        EH = Enemy.GetComponent<Enemy_Hurt>();

        p = player.GetComponent<PlayerScript>();
        rb = Enemy.GetComponent<Rigidbody2D>();

        actual_lifes = lifes;
    }

    void Attack(){

        if(
            Enemy.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Violento_Attack") &&
            Enemy.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.75f && 
            Enemy.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1f
        ){
            AttackBox.SetActive(true);
        }else{
            AttackBox.SetActive(false);
        }

        if(attack_duration>0){
            attack_duration -= Time.deltaTime;
        }else{
            AttackBox.SetActive(false);
        }

        if(cooldown>0 && attack_duration < 0){
            cooldown -= Time.deltaTime;
        }else if(cooldown <= 0){
            ready_to_attack = true;
        }

        player_distance = Mathf.Abs(player.transform.position.x - Enemy.transform.position.x);

        if(attack_range > player_distance && ready_to_attack){
            Enemy.GetComponent<Animator>().SetTrigger("Attack");
            attack_duration = attack_delay;
            cooldown = attack_cooldown;
            ready_to_attack = false;
        }
    }

    void Hurt(){
        if(EH.hurted){
            if(!hurt_start){
                GetComponent<AudioSource>().Play();
                hurt_start=true;
                actual_lifes-=EH.damage;
                if(actual_lifes<=0){
                    dead = true;
                    Enemy.GetComponent<Animator>().SetBool("Dead",true);
                }
                if(p.facingRight){short_term_thrust=hurt_thrust;}
                else{short_term_thrust=-hurt_thrust;}
            }
            sr.color = Color.red;
            targetTime += Time.deltaTime;
            if(targetTime>hurted_delay){EH.hurted=false;}
        }else{
            targetTime = 0;
            sr.color = Color.white;
            hurt_start = false;
        }

        if(dead){short_term_thrust=0;}//else{Enemy.GetComponent<Animator>().SetBool("Dead",false);}
    }
    void FixedUpdate()
    {
        AttackBox.transform.position = Enemy.transform.position;

        if(!dead){

            Hurt();
            Attack();

            if(player.transform.position.x > Detection_Edge1.transform.position.x && player.transform.position.x < Detection_Edge2.transform.position.x){
                if(player.transform.position.x > Enemy.transform.position.x){
                    rb.velocity = new Vector2(1*speed + short_term_thrust*4, rb.velocity.y);
                    right = true;
                }else{
                    rb.velocity = new Vector2(-1*speed + short_term_thrust*4, rb.velocity.y);
                    right = false;
                }
            }else{
                if(Enemy.transform.position.x < Edge1.transform.position.x){right = true;}
                else if(Enemy.transform.position.x > Edge2.transform.position.x){right = false;}
                
                if(right){rb.velocity = new Vector2(1*speed + short_term_thrust*4, rb.velocity.y);}
                else{rb.velocity = new Vector2(-1*speed + short_term_thrust*4, rb.velocity.y);} 
            }

            if(!right && !right_check){
                Enemy.transform.Rotate(0,180,0);
                right_check = true;
                //left_check = false;
            }else if(right && right_check){
                Enemy.transform.Rotate(0,180,0);
                right_check = false;
                //left_check = true;
            }

            if(short_term_thrust != 0){short_term_thrust *= 0.8f;}
        }else{
            sr.color = Color.white;
            rb.velocity = Vector2.zero;
        }
    }
}
