using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour {
    public PolygonCollider2D polygonCollider;

    private void Reset() {
        polygonCollider = GetComponent<PolygonCollider2D>();
    }
}