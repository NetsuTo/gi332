using TMPro;
using Unity.Netcode;
using UnityEngine;

public class BombTimer : NetworkBehaviour
{
    public float timer = 10f;
    public float countdown;
    [SerializeField] private TextMeshProUGUI countdownText;
    
    void Start()
    {
        countdown = timer;
    }

    void Update()
    {
        if (BombManager.Instance == null) return;

        if (BombManager.isGameStart.Value == true)
        {

            if (IsClient)
            {
                if (BombManager.playerWithBomb.Value == NetworkManager.Singleton.LocalClientId)
                {
                    countdown -= Time.deltaTime;
                    UpdateUI();

                    if (countdown <= 0)
                    {
                        Debug.Log("Player exploded! Removing from game...");
                        ulong clientIdToRemove = BombManager.playerWithBomb.Value;
                        RemovePlayerFromGameServerRpc(clientIdToRemove);
                    }
                }
            }
            /*
            else if (IsClient)
            {
                UpdateUI();
            }*/
        }
    }

    private void UpdateUI()
    {
        if (BombManager.playerWithBomb.Value == NetworkManager.Singleton.LocalClientId && countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = countdown.ToString("F1") + "s";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemovePlayerFromGameServerRpc(ulong playerId)
    {
        GameObject playerObject = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject.gameObject;

        if (playerObject != null)
        {
            playerObject.GetComponent<NetworkObject>().Despawn(true);
            Debug.Log($"Player {playerId} removed!");

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
            } while (randomPlayer == BombManager.playerWithBomb.Value);

            BombManager.playerWithBomb.Value = randomPlayer;
        }
        countdown = timer;
    }
}
