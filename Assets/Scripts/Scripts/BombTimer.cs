using Unity.Netcode;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class BombTimer : NetworkBehaviour
{
    public float timer = 10f;
    public float countdown;
    public bool isBomb = false;

    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject showBomb;


    private ulong ownerClientId;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ownerClientId = GetComponent<NetworkObject>().OwnerClientId;
    }

    void Start()
    {
        countdown = timer;
    }

    void Update()
    {

        if (BombManager.Instance == null) return;

        if (BombManager.isGameStart.Value)
        {
            if (IsClient)
            {
                if (BombManager.playerWithBomb.Value == NetworkManager.Singleton.LocalClientId)
                {
                    countdown -= Time.deltaTime;
                    UpdateUI();
                    ShowBomb(true); // ✅ แสดง GameObject showBomb

                    if (countdown <= 0)
                    {
                        BombManager.countPlayer--;
                        Debug.Log("Player exploded! Removing from game...");
                        
                        ulong clientIdToRemove = BombManager.playerWithBomb.Value;

                        RemovePlayerFromGameServerRpc(clientIdToRemove);
                    }
                }
                else
                {
                    HideUI();
                    ShowBomb(false); // ✅ ซ่อน GameObject showBomb
                }

            }
        }
    }

    private void UpdateUI()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = countdown.ToString("F1") + "s";
        }
    }

    private void HideUI()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemovePlayerFromGameServerRpc(ulong playerId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerId, out var client))
        {
            GameObject playerObject = client.PlayerObject.gameObject;
            if (playerObject != null)
            {
                playerObject.GetComponent<NetworkObject>().Despawn(true);
                Debug.Log($"Player {playerId} removed!");
                ResetBomb();
            }
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
            } while (randomPlayer == BombManager.playerWithBomb.Value);

            BombManager.playerWithBomb.Value = randomPlayer;
            SetBombVisualClientRpc(randomPlayer);
        }
        countdown = timer;

        // Call ClientRpc to reset the timer on all clients
        ResetBombOnClientsClientRpc();
    }

    // ✅ ฟังก์ชันส่งระเบิดไปยังผู้เล่นอื่น
    public void TryPassBomb(ulong targetPlayerId)
    {
        if (IsClient && BombManager.playerWithBomb.Value == NetworkManager.Singleton.LocalClientId)
        {
            PassBombToPlayerServerRpc(targetPlayerId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PassBombToPlayerServerRpc(ulong targetPlayerId)
    {
        if (BombManager.Instance == null) return;

        // ตรวจสอบว่าผู้เล่นปลายทางมีอยู่ในระบบ
        if (NetworkManager.Singleton.ConnectedClients.ContainsKey(targetPlayerId))
        {
            // เปลี่ยนผู้เล่นที่ถือระเบิด
            BombManager.playerWithBomb.Value = targetPlayerId;
            SetBombVisualClientRpc(targetPlayerId);
            Debug.Log($"Bomb passed to player {targetPlayerId}");

            // รีเซ็ตเวลาระเบิดบน Server
            countdown = timer;

            // Call ClientRpc to reset the timer on all clients
            ResetBombOnClientsClientRpc();
        }
    }

    // ClientRpc to reset the bomb timer on all clients
    [ClientRpc]
    void ResetBombOnClientsClientRpc()
    {
        countdown = timer;
        UpdateUI();
        ShowBomb(BombManager.playerWithBomb.Value == NetworkManager.Singleton.LocalClientId);
    }


    private void ShowBomb(bool show)
    {
        if (showBomb != null)
        {
            showBomb.SetActive(show);
        }
    }

    [ClientRpc]
    void SetBombVisualClientRpc(ulong playerWithBombId)
    {
        bool isOwnerHasBomb = ownerClientId == playerWithBombId;
        ShowBomb(isOwnerHasBomb);
    }


}
