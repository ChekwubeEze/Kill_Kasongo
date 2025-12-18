
using Assets.Scripts.UIManager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.SpawnManager
{
    public class GameSpawnManager: NetworkBehaviour
    {
        public static GameSpawnManager Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private NetworkObject cubePrefab;

        [Header("Spawn Points")]
        [SerializeField] private Transform collectibleSpawnPointsParent;
        [SerializeField] private Transform playerSpawnPointsParent;

        [Header("Rules")]
        [SerializeField] private int cubesPerPlayer = 3;
        [SerializeField] private int scoreToWin = 1;
        [SerializeField] private float cubeSpawnYOffset = 0.5f;

        private Transform[] collectibleSpawns;
        private Transform[] playerSpawns;

        [SerializeField] private string gameSceneName = "Floor Layout";
        [SerializeField] private string winSceneName = "WinScene";
        [SerializeField] private float winSceneDuration = 3f;

        // Track cubes per client
        private readonly Dictionary<ulong, List<NetworkObject>> cubesByClient = new();

        private void Awake() => Instance = this;

        public override void OnNetworkSpawn()
        {
            collectibleSpawns = GetChildrenTransforms(collectibleSpawnPointsParent);
            playerSpawns = GetChildrenTransforms(playerSpawnPointsParent);

            if (!IsServer) return;

            NetworkManager.OnClientConnectedCallback += OnClientConnectedServer;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnectedServer;

            // In case host already has connected clients
            foreach (var kvp in NetworkManager.ConnectedClients)
                SpawnNCubesForClientServer(kvp.Key);
        }

        private void OnDestroy()
        {
            if (NetworkManager == null) return;
            NetworkManager.OnClientConnectedCallback -= OnClientConnectedServer;
            NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectedServer;
        }

        private void OnClientConnectedServer(ulong clientId)
        {
            SpawnNCubesForClientServer(clientId);
        }

        private void OnClientDisconnectedServer(ulong clientId)
        {
            DespawnAllCubesForClientServer(clientId);
            cubesByClient.Remove(clientId);
        }

        private void SpawnNCubesForClientServer(ulong clientId)
        {
            if (!IsServer) return;
            if (cubePrefab == null || collectibleSpawns.Length == 0) return;

            // Clear existing cubes for this client (if any)
            DespawnAllCubesForClientServer(clientId);

            var list = new List<NetworkObject>();
            cubesByClient[clientId] = list;

            // Choose unique spawn points
            var available = new List<int>(collectibleSpawns.Length);
            for (int i = 0; i < collectibleSpawns.Length; i++) available.Add(i);

            int spawnCount = Mathf.Min(cubesPerPlayer, available.Count);

            for (int n = 0; n < spawnCount; n++)
            {
                int pick = Random.Range(0, available.Count);
                int idx = available[pick];
                available.RemoveAt(pick);

                Transform sp = collectibleSpawns[idx];
                Vector3 pos = sp.position + Vector3.up * cubeSpawnYOffset;

                var cube = Instantiate(cubePrefab, pos, sp.rotation);
                var netObj = cube.GetComponent<NetworkObject>();

                // Owned by that player
                netObj.SpawnWithOwnership(clientId);

                // Only that player can collect
                var collectible = cube.GetComponent<CollectibleCube>();
                collectible.TargetClientId.Value = clientId;

                list.Add(netObj);
            }
        }

        private void DespawnAllCubesForClientServer(ulong clientId)
        {
            if (!IsServer) return;

            if (!cubesByClient.TryGetValue(clientId, out var list) || list == null) return;

            foreach (var c in list)
                if (c != null && c.IsSpawned) c.Despawn(true);

            list.Clear();
        }

        // Called by CollectibleCube on server
        public void OnCubeCollectedServer(ulong clientId, NetworkObject collectedCube)
        {
            if (!IsServer) return;

            // Remove collected cube from tracking list
            if (cubesByClient.TryGetValue(clientId, out var list) && list != null)
            {
                list.Remove(collectedCube);
            }

            // Check score for win
            if (!NetworkManager.ConnectedClients.TryGetValue(clientId, out var client)) return;

            var ps = client.PlayerObject.GetComponent<PlayerState>();
            int score = ps != null ? ps.Score.Value : 0;

            //if (score >= cubesPerPlayer)
            //{
            //    AnnounceWinnerClientRpc(clientId);
            //    RestartRoundServer();
            //}

            if (score >= scoreToWin)
            {
                AnnounceWinnerClientRpc(clientId);

                // Server switches everyone to WinScene
                NetworkManager.Singleton.SceneManager.LoadScene(winSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);

                // After scene loads + delay, go back to game and restart
                StartCoroutine(ReturnToGameAfterWin());
            }
        }

        private void RestartRoundServer()
        {
            if(NetworkManager.Singleton.SceneManager.Equals(winSceneName))
            {
                NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            if (!IsServer) return;

            // Despawn all cubes for everyone
            foreach (var kv in cubesByClient)
            {
                foreach (var c in kv.Value)
                    if (c != null && c.IsSpawned) c.Despawn(true);
            }
            cubesByClient.Clear();

            // Reset scores and respawn players
            int i = 0;
            foreach (var kvp in NetworkManager.ConnectedClients)
            {
                var playerObj = kvp.Value.PlayerObject;

                var ps = playerObj.GetComponent<PlayerState>();
                if (ps != null) ps.Score.Value = 0;

                if (playerSpawns.Length > 0)
                {
                    var sp = playerSpawns[i % playerSpawns.Length];
                    playerObj.transform.SetPositionAndRotation(sp.position, sp.rotation);
                }
                i++;
            }

            // Spawn 3 cubes for each player again
            foreach (var kvp in NetworkManager.ConnectedClients)
                SpawnNCubesForClientServer(kvp.Key);
        }

        [ClientRpc]
        private void AnnounceWinnerClientRpc(ulong winnerClientId)
        {
            Debug.Log($"Player {winnerClientId} collected all cubes and wins! Restarting round...");
            WinData.WinnerClientId = winnerClientId;
        }

        private Transform[] GetChildrenTransforms(Transform parent)
        {
            if (parent == null) return new Transform[0];

            var list = new List<Transform>();
            for (int i = 0; i < parent.childCount; i++)
            {
                var t = parent.GetChild(i);
                if (t.gameObject.activeInHierarchy) list.Add(t); // avoids disabled points
            }
            return list.ToArray();
        }

        private IEnumerator ReturnToGameAfterWin()
        {
            // Small wait so clients fully load WinScene
            yield return new WaitForSeconds(winSceneDuration);

            NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);

            // Wait a frame; then restart round (spawns cubes etc.)
            yield return null;

            RestartRoundServer();
        }

    }
}
