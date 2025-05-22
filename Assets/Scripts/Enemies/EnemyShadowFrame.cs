using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShadowFrame {
    public Vector3 position;
    public Quaternion rotation;
    public bool isAttacking;

    public EnemyShadowFrame(Vector3 pos, Quaternion rot, bool attack) {
        position = pos;
        rotation = rot;
        isAttacking = attack;
    }
}
