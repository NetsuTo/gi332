using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class BombPassProximity : NetworkBehaviour
{
    public BombTimer bombTimer;

    [SerializeField] private HashSet<NetworkObject> nearbyPlayers = new HashSet<NetworkObject>();

    void Start()
    {
        if (bombTimer == null)
            bombTimer = GetComponent<BombTimer>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (bombTimer != null && BombManager.playerWithBomb.Value == NetworkManager.Singleton.LocalClientId)
            {
                NetworkObject targetPlayer = GetClosestPlayer();
                if (targetPlayer != null)
                {
                    ulong targetId = targetPlayer.OwnerClientId;
                    bombTimer.TryPassBomb(targetId);
                    Debug.Log($"Bomb passed to player {targetId} via proximity");
                }
            }
        }
    }

    private NetworkObject GetClosestPlayer()
    {
        float closestDistance = float.MaxValue;
        NetworkObject closest = null;

        foreach (var player in nearbyPlayers)
        {
            if (player != null && player.OwnerClientId != NetworkManager.Singleton.LocalClientId)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = player;
                }
            }
        }

        return closest;
    }

    private void OnTriggerEnter(Collider other)
    {
        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj != null && netObj != this.GetComponent<NetworkObject>())
        {
            nearbyPlayers.Add(netObj);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            nearbyPlayers.Remove(netObj);
        }
    }
}
