using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay.Models;
using UnityEngine;


public class TestRelay : MonoBehaviour
{
    public static TestRelay Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
        };
    }

    public async Task<string> CreateRelay()
    {
        try
        {
         
            if (RelayService.Instance == null)
            {
               
                return null;
            }

            Debug.Log(" Creating Allocation...");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            if (allocation == null)
            {
               
                return null;
            }

            
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            

           
            
            RelayServerData relayServerData = allocation.ToRelayServerData("dtls");

            
            
            if (NetworkManager.Singleton == null)
            {
                
                return null;
            }

            if (NetworkManager.Singleton.GetComponent<UnityTransport>() == null)
            {
               
                return null;
            }

            
            NetworkManager.Singleton
                .GetComponent<UnityTransport>()
                .SetRelayServerData(relayServerData);

            
            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (System.Exception e) 
        {
            
            return null;
        }
    }
    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = joinAllocation.ToRelayServerData("dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
              relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
