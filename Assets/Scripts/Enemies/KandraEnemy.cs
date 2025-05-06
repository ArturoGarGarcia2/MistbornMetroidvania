using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KandraEnemy : EnemyBase {
    public bool transformed = false;

    public void TransformShape() {
        transformed = true;
        Debug.Log($"{gameObject.name} cambia de forma.");
        // Cambia sprite, ataque, velocidad, etc.
    }

    protected override void Attack() {
        if (transformed) {
            Debug.Log($"{gameObject.name} ataca en forma monstruosa.");
        } else {
            base.Attack();
        }
    }
}
