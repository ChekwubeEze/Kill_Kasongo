using Unity.Netcode;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private Transform[] spawnPoints;

    void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.L))
        {
            SpawnObjectServerRpc();
        }
    }

    [ServerRpc]
    private void SpawnObjectServerRpc(ServerRpcParams rpcParams = default)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject spawned = Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);
        spawned.GetComponent<NetworkObject>().Spawn();
    }
}
