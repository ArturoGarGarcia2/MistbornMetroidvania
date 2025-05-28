using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform player;
    public float smoothSpeed = 0.1f;
    public float minMoveDistance = 0.05f; // Umbral mínimo para evitar temblores
    public CameraZone CurrentZone { get; private set; }

    private Camera cam;

    private void Start() {
        cam = Camera.main;
    }

    public void SetCurrentZone(CameraZone zone) {
        CurrentZone = zone;
    }

    private void LateUpdate() {
        if (CurrentZone == null || player == null) return;

        Vector3 targetPos = player.position;
        targetPos.z = transform.position.z;

        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        Vector2 desiredCenter = new Vector2(targetPos.x, targetPos.y);
        Vector2 adjustedCenter = ClampCameraInsidePolygon(desiredCenter, horzExtent, vertExtent);

        Vector3 finalTarget = new Vector3(adjustedCenter.x, adjustedCenter.y, targetPos.z);

        // Solo moverse si se supera un umbral mínimo (anti-temblor)
        if ((finalTarget - transform.position).sqrMagnitude > minMoveDistance * minMoveDistance) {
            Vector3 smoothed = Vector3.Lerp(transform.position, finalTarget, smoothSpeed);
            transform.position = smoothed;
        }
    }

    private Vector2 ClampCameraInsidePolygon(Vector2 desiredCenter, float hx, float hy) {
        if (IsCameraInsideZone(desiredCenter, hx, hy)) {
            return desiredCenter;
        }

        const int maxIterations = 30;
        const float stepSize = 0.2f;

        Vector2 bestPosition = transform.position;
        float minDistance = Vector2.Distance(desiredCenter, bestPosition);

        for (float dx = -stepSize * maxIterations; dx <= stepSize * maxIterations; dx += stepSize) {
            for (float dy = -stepSize * maxIterations; dy <= stepSize * maxIterations; dy += stepSize) {
                Vector2 testPos = desiredCenter + new Vector2(dx, dy);
                if (IsCameraInsideZone(testPos, hx, hy)) {
                    float dist = Vector2.Distance(desiredCenter, testPos);
                    if (dist < minDistance) {
                        minDistance = dist;
                        bestPosition = testPos;
                    }
                }
            }
        }

        return bestPosition;
    }

    private bool IsCameraInsideZone(Vector2 camCenter, float hx, float hy) {
        Vector2[] corners = new Vector2[4] {
            camCenter + new Vector2(-hx, hy),
            camCenter + new Vector2(hx, hy),
            camCenter + new Vector2(hx, -hy),
            camCenter + new Vector2(-hx, -hy)
        };

        foreach (var c in corners) {
            if (!CurrentZone.polygonCollider.OverlapPoint(c))
                return false;
        }

        return true;
    }
}
