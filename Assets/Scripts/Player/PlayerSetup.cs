using UnityEngine;
using TMPro;
using Photon.Pun;

// Handles player identity — assigns a unique color and displays the player's name.
// Runs once when the player is spawned.
public class PlayerSetup : MonoBehaviourPun
{
    // Reference to the name label (a TextMeshPro object above the player's head).
    // We'll create this as a child of the Player prefab in the Editor.
    [SerializeField] private TextMeshPro nameLabel;

    // Array of colors — each player gets a different one based on their ActorNumber.
    // ActorNumber is Photon's unique ID for each player in a room (1, 2, 3, etc.).
    // Think of it like a primary key in a database.
    private static readonly Color[] playerColors = new Color[]
    {
        new Color(0.2f, 0.8f, 0.2f),   // Green
        new Color(0.2f, 0.4f, 1.0f),   // Blue
        new Color(1.0f, 0.3f, 0.3f),   // Red
        new Color(1.0f, 0.8f, 0.0f),   // Yellow
        new Color(1.0f, 0.5f, 0.0f),   // Orange
        new Color(0.8f, 0.2f, 0.8f),   // Purple
        new Color(0.0f, 0.9f, 0.9f),   // Cyan
        new Color(1.0f, 0.4f, 0.7f),   // Pink
        new Color(0.6f, 0.4f, 0.2f),   // Brown
        new Color(0.9f, 0.9f, 0.9f),   // White
    };

    void Start()
    {
        // Get this player's color based on their ActorNumber.
        // The % (modulo) wraps around if there are more players than colors.
        int colorIndex = (photonView.Owner.ActorNumber - 1) % playerColors.Length;
        Color myColor = playerColors[colorIndex];

        // Apply the color to the sprite.
        // GetComponent<T>() finds a component on this GameObject — like
        // accessing an attribute: self.sprite_renderer in Python.
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            sprite.color = myColor;

        // Set the name label to show the player's Photon nickname.
        // photonView.Owner.NickName is the name they typed in the lobby.
        if (nameLabel != null)
            nameLabel.text = photonView.Owner.NickName;
    }
}
