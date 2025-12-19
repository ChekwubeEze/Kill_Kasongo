using Unity.Netcode;
using UnityEngine;

public class OwnedCubeNetcode : NetworkBehaviour
{
    [Header("Ownership")]
    public ulong ownerClientId;        

    [Header("Spawning")]
    public GameObject cubePrefab;      
    public Vector3 minSpawnPos;
    public Vector3 maxSpawnPos;

    private void OnTriggerEnter(Collider other)
    {
        // Run this only on the server to keep authority
        if (!IsServer) return;

        // Check if the other collider belongs to a player
        var player = other.GetComponent<PlayerIdNetcode>();
        if (player == null) return;

        if (player.ClientId != ownerClientId) return;

        SpawnNewCubeForSameOwner();
        NetworkObject.Despawn(true); 
    }

    private void SpawnNewCubeForSameOwner()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(minSpawnPos.x, maxSpawnPos.x),
            Random.Range(minSpawnPos.y, maxSpawnPos.y),
            Random.Range(minSpawnPos.z, maxSpawnPos.z)
        );

        // Instantiate on the server
        GameObject newCubeObj = Instantiate(cubePrefab, randomPos, Quaternion.identity);

        var newCubeNetObj = newCubeObj.GetComponent<NetworkObject>();
        var newCubeScript = newCubeObj.GetComponent<OwnedCubeNetcode>();

        // Set owner id and spawn settings before Spawn
        newCubeScript.ownerClientId = ownerClientId;
        newCubeScript.cubePrefab = cubePrefab;
        newCubeScript.minSpawnPos = minSpawnPos;
        newCubeScript.maxSpawnPos = maxSpawnPos;

        
        newCubeNetObj.Spawn();
    }
}
