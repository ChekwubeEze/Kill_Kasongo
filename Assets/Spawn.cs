using UnityEngine;
using Unity.Netcode;

public class Spawn : NetworkBehaviour
{
    public GameObject Spawnable;

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SpawnServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership =false)]
    void SpawnServerRpc()
    {
        GameObject obj = Instantiate(Spawnable , transform.position + Vector3.forward, Quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn(true);
    }


}