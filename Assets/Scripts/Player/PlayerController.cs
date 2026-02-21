using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

// Now inherits from MonoBehaviourPun instead of MonoBehaviour.
// MonoBehaviourPun gives us access to this.photonView — a reference
// to the PhotonView component on this GameObject. We use it to check
// if this player belongs to us (the local machine) or someone else.
public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput;

    // Reference to the PlayerInput component so we can disable it
    // on remote players (we don't want to process input for other people).
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            // This is OUR player — tell the camera to follow us.
            // FindFirstObjectByType searches the scene for a CameraFollow component.
            CameraFollow cam = FindFirstObjectByType<CameraFollow>();
            if (cam != null)
                cam.SetTarget(transform);
        }
        else
        {
            // This is someone else's player — disable input so our
            // keyboard doesn't move their character.
            if (playerInput != null)
                playerInput.enabled = false;
        }
    }

    public void OnMove(InputValue value)
    {
        // Only process input if this is our local player.
        if (!photonView.IsMine) return;

        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        // Only move if this is our local player.
        // Remote players get their position synced via PhotonTransformView.
        if (!photonView.IsMine) return;

        Vector2 direction = moveInput.normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }
}
