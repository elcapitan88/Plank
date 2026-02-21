using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput;
    private PlayerInput playerInput;

    // Rigidbody2D handles physics — collisions, gravity, etc.
    // By moving through the Rigidbody instead of transform.position,
    // Unity's physics engine will stop us when we hit a wall collider.
    // Think of it like the difference between teleporting vs physically walking.
    private Rigidbody2D rb;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            CameraFollow cam = FindFirstObjectByType<CameraFollow>();
            if (cam != null)
                cam.SetTarget(transform);
        }
        else
        {
            if (playerInput != null)
                playerInput.enabled = false;
        }
    }

    public void OnMove(InputValue value)
    {
        if (!photonView.IsMine) return;
        moveInput = value.Get<Vector2>();
    }

    // FixedUpdate runs at a fixed interval (default 50 times/sec) and is
    // used for physics. Regular Update runs every frame (variable rate).
    // Physics should ALWAYS go in FixedUpdate for consistent behavior.
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Vector2 direction = moveInput.normalized;

        // MovePosition tells the Rigidbody to move to a new position
        // while respecting collisions. If a wall is in the way, it stops.
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }
}
