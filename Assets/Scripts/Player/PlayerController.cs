using UnityEngine;
using UnityEngine.InputSystem;

// This script handles basic player movement using Unity's new Input System.
// The new Input System uses "actions" instead of polling keys directly.
// Think of it like event-driven input — you define actions (Move, Jump, etc.)
// and Unity maps them to keys/controllers for you.
public class PlayerController : MonoBehaviour
{
    // [SerializeField] exposes this variable in the Unity Inspector
    // so you can tweak the speed without changing code.
    [SerializeField] private float moveSpeed = 5f;

    // This stores the current movement direction from player input.
    // It gets updated whenever the player presses/releases WASD.
    private Vector2 moveInput;

    // Called by the Player Input component via "Send Messages" whenever
    // the "Move" action fires. The method name must be "OnMove" to match
    // the action name "Move". Unity sends an InputValue object containing
    // the input data — similar to how Flask/FastAPI passes request data
    // to your route handler.
    public void OnMove(InputValue value)
    {
        // Get<Vector2>() reads the direction from the input:
        //   W = (0,1), S = (0,-1), A = (-1,0), D = (1,0)
        // When keys are released, it sends (0,0) automatically.
        moveInput = value.Get<Vector2>();
    }

    // Update() is called once per frame by Unity (like a game loop).
    void Update()
    {
        // Normalize so diagonal movement isn't faster than cardinal.
        Vector2 direction = moveInput.normalized;

        // Move the GameObject by shifting its position.
        // Time.deltaTime = time since last frame — makes movement frame-rate independent.
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }
}
