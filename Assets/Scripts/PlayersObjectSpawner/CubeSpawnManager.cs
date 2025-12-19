using Unity.Netcode;
using UnityEngine;

public class CubeSpawnManager : NetworkBehaviour
{
    public GameObject cubePrefab;
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
        Transform SpawnPoint = spawnPoint[Random.Range(0, spawnPoint.Length -1)];   

        GameObject cubeObj = Instantiate(cubePrefab, SpawnPoint.position, SpawnPoint.rotation);

        var cubeNetObj = cubeObj.GetComponent<NetworkObject>();
        var cubeScript = cubeObj.GetComponent<OwnedCubeNetcode>();

        cubeScript.ownerClientId = clientId;
        cubeScript.cubePrefab = cubePrefab;
        cubeNetObj.Spawn();
    }
}
