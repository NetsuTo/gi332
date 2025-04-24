using StarterAssets;
using Unity.Netcode;
using UnityEngine;

public class ActivateBomb : NetworkBehaviour
{
    [SerializeField] private float explosionTime = 10f;
    [SerializeField] private float cooldownAfterExplode = 3f;
    [SerializeField] private GameObject explosionTextUI;

    [SerializeField] private NetworkVariable<ulong> currentHolder = new NetworkVariable<ulong>();
    private float timer;
    private bool exploded = false;

    private GameObject nearbyPlayer;
    private Camera playerCamera;

    void Start()
    {
        if (IsServer)
        {
            timer = explosionTime;
        }

        playerCamera = Camera.main;
    }

    void Update()
    {
        if (IsServer)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                Explode();
            }
        }

        if (IsOwner && Input.GetMouseButtonDown(0))
        {
            AttemptPassBomb();
        }
    }

    private void AttemptPassBomb()
    {
        if (IsOwner && nearbyPlayer != null)
        {
            ulong targetId = nearbyPlayer.GetComponent<NetworkObject>().OwnerClientId;
            PassBombServerRpc(targetId);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsOwner && other.CompareTag("Player"))
        {
            ulong targetId = other.GetComponent<NetworkObject>().OwnerClientId;
            if (targetId != currentHolder.Value)
            {
                nearbyPlayer = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsOwner && other.gameObject == nearbyPlayer)
        {
            nearbyPlayer = null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PassBombServerRpc(ulong newHolderId)
    {
        currentHolder.Value = newHolderId;

        NetworkObject target = NetworkManager.SpawnManager.GetPlayerNetworkObject(newHolderId);
        if (target != null && target.TryGetComponent(out StarterAssetsInputs player))
        {
            transform.SetParent(player.transform);
            transform.localPosition = Vector3.up;
            timer = explosionTime;
            exploded = false;
        }
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        Debug.Log($"Boom! Player {currentHolder.Value} ขิต!!!");

        if (IsServer)
        {
            NetworkObject playerObj = NetworkManager.SpawnManager.GetPlayerNetworkObject(currentHolder.Value);
            if (playerObj != null)
                playerObj.Despawn();

            NetworkObject.Despawn();
            ShowExplosionTextClientRpc();

            //Invoke(nameof(NotifyManager), cooldownAfterExplode);
        }
    }

    /*void NotifyManager()
    {
        FindObjectOfType<GameManager>().CheckPlayers();
    }*/

    [ClientRpc]
    void ShowExplosionTextClientRpc()
    {
        if (explosionTextUI != null)
        {
            GameObject ui = Instantiate(explosionTextUI);
            Destroy(ui, 2f);
        }
    }
}
