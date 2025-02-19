using Unity.Netcode;
using UnityEngine;

public class BombTimer : NetworkBehaviour
{
    public float timer = 10f; // เวลาในการส่งระเบิด
    private float countdown;

    void Start()
    {
        countdown = timer;
    }

    void Update()
    {
        if (IsServer)
        {
            if (BombManager.playerWithBomb.Value == NetworkManager.Singleton.LocalClientId)
            {
                countdown -= Time.deltaTime;
                if (countdown <= 0)
                {
                    Debug.Log("Player failed to pass the bomb. Removing...");

                    ulong clientIdToDisconnect = BombManager.playerWithBomb.Value;

                    NetworkManager.Singleton.DisconnectClient(clientIdToDisconnect);

                    ResetBomb();
                }
            }
        }
    }

    void ResetBomb()
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count > 1)
        {
            ulong randomPlayer = NetworkManager.Singleton.ConnectedClientsList[Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count)].ClientId;
            BombManager.playerWithBomb.Value = randomPlayer;
        }
    }

}
