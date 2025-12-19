using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

namespace Assets.Scripts.SpawnManager
{
    public class PlayerState: NetworkBehaviour
    {
        public NetworkVariable<int> Score = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                Score.Value = 0;
        }
    }
}
