using Unity.Netcode;
using UnityEngine;

public class NetCodeTestUI : MonoBehaviour
{
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        DeactivateButtons();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        DeactivateButtons();
    }
    private void DeactivateButtons()
    {
        Destroy(gameObject);
    }
}
