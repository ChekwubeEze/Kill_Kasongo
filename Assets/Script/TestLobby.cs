using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestLobby : MonoBehaviour
{
    private Lobby currentLobby;
    private float heartbeatTimer;
    private const float HEARTBEAT_INTERVAL = 15f;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
    }

    // -----------------------------------
    // CREATE LOBBY
    // -----------------------------------
    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "TestLobby";
            int maxPlayers = 4;

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = new Player(
                    id: AuthenticationService.Instance.PlayerId,
                    data: new Dictionary<string, PlayerDataObject>
                    {
                        { "playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Player_" + Random.Range(100, 999)) }
                    }
                )
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            Debug.Log($"Created Lobby: {currentLobby.Id} | Code: {currentLobby.LobbyCode}");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    // -----------------------------------
    // LIST AVAILABLE LOBBIES
    // -----------------------------------
    public async void ListLobbies()
    {
        try
        {
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();

            Debug.Log("Found Lobbies: " + response.Results.Count);

            foreach (Lobby lobby in response.Results)
            {
                Debug.Log($"Lobby: {lobby.Name} | Players: {lobby.Players.Count}/{lobby.MaxPlayers} | Code: {lobby.LobbyCode}");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    // -----------------------------------
    // JOIN LOBBY USING LOBBY CODE
    // -----------------------------------
    public async void JoinLobby(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinOptions = new JoinLobbyByCodeOptions
            {
                Player = new Player(
                    id: AuthenticationService.Instance.PlayerId,
                    data: new Dictionary<string, PlayerDataObject>
                    {
                        { "playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Player_" + Random.Range(100, 999)) }
                    }
                )
            };

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinOptions);

            Debug.Log("Joined Lobby! Lobby ID: " + currentLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    // -----------------------------------
    // HOST HEARTBEAT (MUST RUN EVERY 15 SEC)
    // -----------------------------------
    private async void HandleLobbyHeartbeat()
    {
        if (currentLobby == null || currentLobby.HostId != AuthenticationService.Instance.PlayerId)
            return;

        heartbeatTimer -= Time.deltaTime;

        if (heartbeatTimer <= 0f)
        {
            heartbeatTimer = HEARTBEAT_INTERVAL;

            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
                Debug.Log("Heartbeat sent.");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }
    }

    // -----------------------------------
    // UPDATE PLAYER DATA
    // -----------------------------------
    public async void UpdatePlayerName(string newName)
    {
        if (currentLobby == null)
        {
            Debug.LogError("No lobby joined.");
            return;
        }

        try
        {
            Dictionary<string, PlayerDataObject> updateData = new Dictionary<string, PlayerDataObject>
            {
                { "playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, newName) }
            };

            UpdatePlayerOptions options = new UpdatePlayerOptions
            {
                Data = updateData
            };

            await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId, options);

            Debug.Log("Player name updated to: " + newName);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
