using UnityEngine;

// Attaches to the Main Camera. Smoothly follows the local player.
// We find the local player at runtime since it's spawned dynamically by Photon.
public class CameraFollow : MonoBehaviour
{
    // How quickly the camera catches up to the player.
    // Higher = snappier, lower = more floaty/cinematic.
    [SerializeField] private float smoothSpeed = 8f;

    // The target transform to follow (set at runtime when local player spawns).
    private Transform target;

    // Called by PlayerController when the local player is spawned.
    // This is a simple pattern: the player finds the camera and tells it "follow me."
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void LateUpdate()
    {
        // LateUpdate runs AFTER Update — so the player moves first,
        // then the camera follows. This prevents jittery movement.
        if (target == null) return;

        // Keep the camera's Z position at -10 (default for 2D).
        // In 2D, the camera needs to be "behind" the scene to see it.
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10f);

        // Lerp = Linear Interpolation. Smoothly moves from current position
        // toward the target. smoothSpeed * Time.deltaTime controls the rate.
        // It's like easing in CSS transitions.
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
