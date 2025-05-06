using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonBrumousEnemy : EnemyBase {
    void Update() {
        TryAttack();
    }
}
