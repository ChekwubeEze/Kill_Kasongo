using Unity.Netcode;
using UnityEngine;

public class JumpAction : NetworkBehaviour
{
    public Transform Transform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Transform.Translate(transform.position.x, 4, 4);
        }
    }
}
