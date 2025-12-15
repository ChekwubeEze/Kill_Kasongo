using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(NetworkObject))]
public class PlayerSpawner : NetworkBehaviour
{
    
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        // Only the Server/Host should be listening to this event and perform spawning.
        if (IsServer)
        {
          
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        }
    }

    public override void OnNetworkDespawn()
    {
      
        if (NetworkManager.Singleton != null && IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length-1)];

      
        GameObject playerInstance =
            Instantiate(playerPrefab, spawn.position, spawn.rotation);

       
        playerInstance.GetComponent<NetworkObject>()
            .SpawnAsPlayerObject(clientId);
    }
}