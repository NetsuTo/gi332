using TMPro;
using Unity.Netcode;
using UnityEngine;

public class BombTimer : NetworkBehaviour
{
    public float timer = 10f; // เวลานับถอยหลังเริ่มต้น
    public float countdown;
    [SerializeField] private TextMeshProUGUI countdownText; // UI Text แสดงเวลา

    void Start()
    {
        countdown = timer;
    }

    void Update()
    {
        if (BombManager.Instance == null) return;

        if (IsServer && BombManager.playerWithBomb.Value == NetworkManager.Singleton.LocalClientId)
        {
            countdown -= Time.deltaTime;
            UpdateUI();

            if (countdown <= 0)
            {
                Debug.Log("Player exploded! Removing from game...");
                ulong clientIdToRemove = BombManager.playerWithBomb.Value;
                RemovePlayerFromGameServerRpc(clientIdToRemove); // เรียกใช้ ServerRpc
            }
        }
        else if (IsClient)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (BombManager.playerWithBomb.Value == NetworkManager.Singleton.LocalClientId && countdownText != null)
        {
            countdownText.text = countdown.ToString("F1") + "s"; // อัปเดตค่าเวลา
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemovePlayerFromGameServerRpc(ulong playerId)
    {
        GameObject playerObject = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject.gameObject;

        if (playerObject != null)
        {
            // ซ่อนผู้เล่นที่ระเบิดจากหน้าจอทุกคน
            playerObject.GetComponent<NetworkObject>().Despawn(true);
            Debug.Log($"Player {playerId} removed!");

            // ส่งระเบิดให้ผู้เล่นใหม่
            ResetBomb();
        }
    }

    void ResetBomb()
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count > 1)
        {
            ulong randomPlayer;
            do
            {
                randomPlayer = NetworkManager.Singleton.ConnectedClientsList[Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count)].ClientId;
            } while (randomPlayer == BombManager.playerWithBomb.Value); // หลีกเลี่ยงให้ระเบิดกลับไปที่ผู้เล่นเดิม

            BombManager.playerWithBomb.Value = randomPlayer;
        }
        countdown = timer; // รีเซ็ตเวลา
    }
}
