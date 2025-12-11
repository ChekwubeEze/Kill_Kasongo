using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkTestingAuthServer : NetworkBehaviour
{
    private Renderer playerRenderer;

    private void Awake()
    {
            playerRenderer = GetComponent<Renderer>();  
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            ChangeAppearanceServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangeAppearanceServerRpc(ServerRpcParams rpcParams = default)
    {
        Color newColor = new Color(Random.value, Random.value, Random.value);

        playerRenderer.material.color = newColor;

        ChangeAppearanceClientRpc(newColor);
    }

    [ClientRpc]
    private void ChangeAppearanceClientRpc(Color newColor)
    {
        playerRenderer.material.color = newColor;
    }

}
