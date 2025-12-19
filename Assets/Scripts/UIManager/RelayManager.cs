using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;

namespace Assets.Scripts.UIManager
{
    public class RelayManager : MonoBehaviour
    {
        public static RelayManager Instance;

        private const string RelayKey = "relayJoinCode";

        private void Awake() => Instance = this;

        public async void CreateRelayAndStartHost(string lobbyId)
        {
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(4);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            await LobbyService.Instance.UpdateLobbyAsync(lobbyId,
                new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                    { RelayKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                    }
                });

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(new RelayServerData(
                alloc.RelayServer.IpV4,
                (ushort)alloc.RelayServer.Port,
                alloc.AllocationIdBytes,
                alloc.Key,
                alloc.ConnectionData,
                alloc.ConnectionData,
                true));

            NetworkManager.Singleton.StartHost();
        }

        public async void JoinRelayAndStartClient(Lobby lobby)
        {
            string joinCode = lobby.Data[RelayKey].Value;
            JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(new RelayServerData(
                joinAlloc.RelayServer.IpV4,
                (ushort)joinAlloc.RelayServer.Port,
                joinAlloc.AllocationIdBytes,
                joinAlloc.Key,
                joinAlloc.ConnectionData,
                joinAlloc.HostConnectionData,
                true));

            NetworkManager.Singleton.StartClient();
        }
    }
}
