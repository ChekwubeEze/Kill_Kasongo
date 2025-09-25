using UnityEngine;
using Unity.Netcode;

public class Spawn : NetworkBehaviour
{
    public GameObject Spawnable;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Spawnable.SetActive(true);
            }
        }
    }
}
