using UnityEngine;
using Unity.Netcode;

public class net : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        {
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.forward * 4 * Time.deltaTime);
            }
        }
       
    }
}
