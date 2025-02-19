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
                    // ลบผู้เล่นที่ถือระเบิดออกจากการแข่งขัน
                    Debug.Log("Player failed to pass the bomb. Removing...");
                    NetworkManager.Singleton.DisconnectClient(BombManager.playerWithBomb.Value);
                    ResetBomb();
                }
            }
        }
    }

    void ResetBomb()
    {
        // สุ่มระเบิดให้ผู้เล่นคนอื่น
        ulong randomPlayer = NetworkManager.Singleton.ConnectedClientsList[Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count)].ClientId;
        BombManager.playerWithBomb.Value = randomPlayer;
        countdown = timer;
    }
}
