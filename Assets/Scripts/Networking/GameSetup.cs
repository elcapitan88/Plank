using UnityEngine;
using Photon.Pun;

// This script runs when the Game scene loads.
// Each client spawns their own player using Photon's networked instantiation.
// PhotonNetwork.Instantiate creates the object on ALL clients automatically —
// it's like an API call that tells every connected machine "create this object."
public class GameSetup : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Spawn the player at a random position near the center so players
        // don't all stack on top of each other.
        Vector2 randomPos = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));

        // PhotonNetwork.Instantiate works like Unity's Instantiate, but networked.
        // Key differences from regular Instantiate:
        //   1. The prefab MUST be in a "Resources" folder (Photon's requirement)
        //   2. You pass the prefab NAME as a string, not a reference
        //   3. It automatically creates the object on all connected clients
        //   4. It assigns a PhotonView so the object can sync across the network
        PhotonNetwork.Instantiate("Player", randomPos, Quaternion.identity);
    }
}
