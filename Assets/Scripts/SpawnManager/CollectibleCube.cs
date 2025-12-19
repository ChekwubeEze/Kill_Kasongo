using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.SpawnManager
{
    [RequireComponent(typeof(Collider))]
    public class CollectibleCube: NetworkBehaviour
    {
        public NetworkVariable<ulong> TargetClientId = new NetworkVariable<ulong>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

        private void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;

            var playerNetObj = other.GetComponentInParent<NetworkObject>();
            if (playerNetObj == null) return;

            ulong touchingClientId = playerNetObj.OwnerClientId;

            // Only the intended owner can collect
            if (touchingClientId != TargetClientId.Value) return;

            // Award score on server
            var ps = playerNetObj.GetComponent<PlayerState>();
            if (ps != null) ps.Score.Value += 1;

            // Inform GameManager before despawn
            GameSpawnManager.Instance?.OnCubeCollectedServer(touchingClientId, NetworkObject);

            NetworkObject.Despawn(true);
        }
    }
}
