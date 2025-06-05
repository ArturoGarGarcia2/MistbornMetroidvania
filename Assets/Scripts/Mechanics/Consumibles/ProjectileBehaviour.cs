using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour {
    public float speed;
    public int damage;

    private Vector2 direction = Vector2.right;

    public void SetDirection(Vector2 dir){
        direction = dir.normalized;

        // Solo voltea el sprite visual (si está en un hijo)
        if (dir.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    void Update(){
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Enemy")){
            other.GetComponent<SimpleBrumousEnemy>()?.ProjectileHurted(damage);
            Destroy(gameObject);
        }
        if (other.CompareTag("Koloss")){
            other.GetComponent<KolossBoss>()?.GetHurt(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Floor")){
            Destroy(gameObject);
        }
    }
}
