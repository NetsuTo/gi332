using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class BombManager : NetworkBehaviour
{
    public static NetworkVariable<ulong> playerWithBomb = new NetworkVariable<ulong>(NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Start()
    {
        if (IsServer)
        {
            // สุ่มผู้เล่นที่ถือระเบิด
            ulong randomPlayer = NetworkManager.Singleton.ConnectedClientsList[Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count)].ClientId;
            playerWithBomb.Value = randomPlayer;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsClient && playerWithBomb.Value == NetworkManager.Singleton.LocalClientId)
        {
            // ถ้าผู้เล่นนี้ถือระเบิด ให้แสดงให้ทราบ
            Debug.Log("You have the bomb!");
        }
    }
}
