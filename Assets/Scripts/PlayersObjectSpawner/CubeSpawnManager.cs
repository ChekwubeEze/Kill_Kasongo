using Unity.Netcode;
using UnityEngine;

public class CubeSpawnManager : NetworkBehaviour
{
    public GameObject cubePrefab;
    //public Vector3 minSpawnPos;
    //public Vector3 maxSpawnPos;
    public Transform[] spawnPoint;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDestroy()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"SpawnCubeForClient(clientId) {clientId}");
        SpawnCubeForClient(clientId);
    }

    private void SpawnCubeForClient(ulong clientId)
    {
        //Vector3 randomPos = new Vector3(
        //    //Random.Range(minSpawnPos.x, maxSpawnPos.x),
        //    //Random.Range(minSpawnPos.y, maxSpawnPos.y),
        //    //Random.Range(minSpawnPos.z, maxSpawnPos.z)
        //    Random.Range(0, -2),
        //    Random.Range(0, -2),
        //    Random.Range(0, -2)
        //);
        Transform SpawnPoint = spawnPoint[Random.Range(0, spawnPoint.Length -1)];   

        GameObject cubeObj = Instantiate(cubePrefab, SpawnPoint.position, SpawnPoint.rotation);

        var cubeNetObj = cubeObj.GetComponent<NetworkObject>();
        var cubeScript = cubeObj.GetComponent<OwnedCubeNetcode>();

        cubeScript.ownerClientId = clientId;
        cubeScript.cubePrefab = cubePrefab;
        //cubeScript.minSpawnPos = minSpawnPos;
        //cubeScript.maxSpawnPos = maxSpawnPos;

        // Optionally give ownership to the client if you want:
        // cubeNetObj.SpawnWithOwnership(clientId);
        cubeNetObj.Spawn();
    }
}
