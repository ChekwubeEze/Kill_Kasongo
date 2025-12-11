using Unity.Netcode;
using UnityEngine;

public class SimpleServerAuthmechanics : NetworkBehaviour
{
    [SerializeField] private GameObject ObjectToDestroy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        if(Input.GetKeyDown(KeyCode.A) )
            DestroyObject_ServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyObject_ServerRPC()
    {
        Destroy(gameObject);
    }
}
