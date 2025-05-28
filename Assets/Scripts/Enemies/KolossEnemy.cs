using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KolossEnemy : EnemyBase {
    protected override void Attack() {
        // Debug.Log($"{gameObject.name} hace un ataque brutal como Koloss.");
        base.Attack();
    }
}
