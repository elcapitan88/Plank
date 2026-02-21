using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

// Enum for the two roles in the game.
public enum Role
{
    None,      // Not assigned yet
    Crew,      // Good guys — do tasks, find the mutineer
    Mutineer   // Bad guys — kill crew, sabotage the ship
}

// Handles role assignment and storage for each player.
// Roles are stored in Photon Player Custom Properties — each player has
// their own properties dict that syncs across the network.
// The role is stored as an int so other clients CAN read it, but we only
// SHOW the role to the local player (and to mutineers seeing other mutineers).
public class PlayerRole : MonoBehaviourPunCallbacks
{
    // The key used in Photon Player Custom Properties.
    private const string ROLE_KEY = "Role";
    private const string ALIVE_KEY = "IsAlive";

    // Cached role for quick access.
    public Role MyRole { get; private set; } = Role.None;
    public bool IsAlive { get; private set; } = true;

    void Start()
    {
        // Read the role from Photon properties (it may already be set
        // if the host assigned roles before this client's Start() ran).
        ReadRoleFromProperties();
    }

    // --- ROLE ASSIGNMENT (called by GameManager on host only) ---

    // Static method that assigns roles to ALL players in the room.
    // Only the MasterClient (host) runs this.
    // Static = belongs to the class itself, not an instance. Like a @staticmethod in Python.
    public static void AssignRoles()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        int playerCount = players.Length;

        // Determine how many mutineers based on player count.
        // 4-6 players = 1 mutineer, 7-10 players = 2 mutineers.
        int mutineerCount = playerCount >= 7 ? 2 : 1;

        // Create a shuffled list of indices to randomly pick mutineers.
        // This is like random.sample() in Python.
        int[] indices = new int[playerCount];
        for (int i = 0; i < playerCount; i++) indices[i] = i;

        // Fisher-Yates shuffle — the standard way to randomize an array.
        for (int i = playerCount - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = indices[i];
            indices[i] = indices[j];
            indices[j] = temp;
        }

        // Assign roles: first N shuffled indices are mutineers, rest are crew.
        for (int i = 0; i < playerCount; i++)
        {
            Role role = (i < mutineerCount) ? Role.Mutineer : Role.Crew;
            Photon.Realtime.Player player = players[indices[i]];

            // Set the role and alive status in that player's Custom Properties.
            Hashtable props = new Hashtable
            {
                { ROLE_KEY, (int)role },
                { ALIVE_KEY, true }
            };
            player.SetCustomProperties(props);

            Debug.Log($"[Roles] {player.NickName} → {role}");
        }
    }

    // --- READING ROLE ---

    private void ReadRoleFromProperties()
    {
        object roleObj;
        if (photonView.Owner.CustomProperties.TryGetValue(ROLE_KEY, out roleObj))
        {
            MyRole = (Role)(int)roleObj;
        }

        object aliveObj;
        if (photonView.Owner.CustomProperties.TryGetValue(ALIVE_KEY, out aliveObj))
        {
            IsAlive = (bool)aliveObj;
        }
    }

    // Photon callback — fires when ANY player's custom properties change.
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        // Only update if the changed properties belong to this player object's owner.
        if (targetPlayer != photonView.Owner) return;

        if (changedProps.ContainsKey(ROLE_KEY))
        {
            MyRole = (Role)(int)changedProps[ROLE_KEY];
        }

        if (changedProps.ContainsKey(ALIVE_KEY))
        {
            IsAlive = (bool)changedProps[ALIVE_KEY];
        }
    }

    // --- UTILITY ---

    // Marks this player as dead. Called by the kill system.
    public void SetDead()
    {
        Hashtable props = new Hashtable { { ALIVE_KEY, false } };
        photonView.Owner.SetCustomProperties(props);
    }

    // Check if a specific Photon player is a mutineer.
    // Used by mutineers to see who their teammates are.
    public static bool IsMutineer(Photon.Realtime.Player player)
    {
        object roleObj;
        if (player.CustomProperties.TryGetValue(ROLE_KEY, out roleObj))
        {
            return (Role)(int)roleObj == Role.Mutineer;
        }
        return false;
    }
}
