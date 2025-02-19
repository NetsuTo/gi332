using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public List<ulong> players = new List<ulong>();
    [SerializeField] private GameObject bombPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Invoke("SpawnBomb", 2f);
        }
    }

    void SpawnBomb()
    {
        if (players.Count > 1)
        {
            ulong randomPlayerId = players[Random.Range(0, players.Count)];
            GameObject bomb = Instantiate(bombPrefab);
            bomb.GetComponent<NetworkObject>().Spawn();
            bomb.GetComponent<ActivateBomb>().PassBombServerRpc(randomPlayerId);
        }
    }

    public void CheckPlayers()
    {
        players.RemoveAll(id => NetworkManager.SpawnManager.GetPlayerNetworkObject(id) == null);

        if (players.Count <= 1)
        {
            Debug.Log($"{players[0]} WIN!!!");
            Invoke("RestartGame", 3f);
        }
    }

    void RestartGame()
    {
        NetworkManager.Singleton.Shutdown();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}