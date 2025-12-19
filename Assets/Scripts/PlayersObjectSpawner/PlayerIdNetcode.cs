using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerIdNetcode : NetworkBehaviour
{
    public ulong ClientId => OwnerClientId; 

    // Optional: just to see it in the inspector for debugging
    [SerializeField] private ulong clientIdDebug;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            clientIdDebug = OwnerClientId;
        }
    }
}
