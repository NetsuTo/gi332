using Unity.Netcode;
using UnityEngine;

public class ReadyButton : NetworkBehaviour
{
    [SerializeField] private GameObject myButton;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            myButton.SetActive(true);
        }
    }

    public void OnPlayerReady()
    {
        if (BombManager.Instance == null) return;

        if (IsClient)
        {
            UpdatePlayerNumReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerNumReadyServerRpc()
    {
        BombManager.numPlayerReady.Value++;
    }
}
