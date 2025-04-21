using UnityEngine;
using Unity.Netcode;

public class OnEnteredEndBlock : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!IsOwner) return; // Only the owner can trigger this

            // Notify the server that the player has reached the end block
            NotifyEndBlockReachedServerRpc();
        }
    }

    [ServerRpc]
    private void NotifyEndBlockReachedServerRpc()
    {
        Debug.Log($"Player {OwnerClientId} has reached the end block.");
        // Add logic here to handle the end of the game or level
    }
}