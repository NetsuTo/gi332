using Unity.Netcode;
using UnityEngine;

public class BombTransfer : NetworkBehaviour
{
    public Collider bombCollider;

    void Update()
    {
        if (IsLocalPlayer && bombCollider != null && Input.GetMouseButtonDown(0)) // คลิกซ้าย
        {
            if (bombCollider.bounds.Contains(transform.position))
            {
                // ส่งระเบิดไปให้ผู้เล่นที่มี Collider
                SendBombToPlayer(bombCollider.GetComponent<NetworkObject>().OwnerClientId);
            }
        }
    }

    void SendBombToPlayer(ulong newPlayer)
    {
        if (IsServer)
        {
            BombManager.playerWithBomb.Value = newPlayer;
            Debug.Log($"Bomb sent to player {newPlayer}");
        }
    }

}
