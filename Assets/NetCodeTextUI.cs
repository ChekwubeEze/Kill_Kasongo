using Unity.Netcode;
using UnityEngine;

public class NetCodeTextUI : MonoBehaviour
{
   public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        DeactivateButton();
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        DeactivateButton();
    }
    private void DeactivateButton()
    {
        Destroy(gameObject);

    }
}