using StarterAssets;
using Unity.Netcode;
using UnityEngine;

public class ActivateBomb : NetworkBehaviour
{
    [SerializeField] private float explosionTime = 10f;
    [SerializeField] private float timer;

    [SerializeField] private NetworkVariable<ulong> currentHolder = new NetworkVariable<ulong>();
    private Camera playerCamera;

    void Start()
    {
        if (IsServer)
        {
            timer = explosionTime;
        }

        playerCamera = Camera.main;  // ใช้ Camera หลักของผู้เล่น
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

        // ตรวจสอบการคลิกซ้ายของผู้เล่น
        if (IsOwner && Input.GetMouseButtonDown(0)) // คลิกซ้าย
        {
            AttemptPassBomb();
        }
    }

    private void AttemptPassBomb()
    {
        if (IsServer)
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ตรวจสอบว่า hit object เป็น player หรือไม่
                if (hit.collider.CompareTag("Player")) // เปลี่ยนเป็น Tag ของผู้เล่น
                {
                    NetworkObject hitPlayer = hit.collider.GetComponent<NetworkObject>();
                    if (hitPlayer != null && hitPlayer.IsOwner == false) // ตรวจสอบว่าไม่ใช่ผู้เล่นที่ถือระเบิด
                    {
                        ulong newHolderId = hitPlayer.OwnerClientId;
                        PassBombServerRpc(newHolderId);
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PassBombServerRpc(ulong newHolderId)
    {
        currentHolder.Value = newHolderId;
        NetworkManager.SpawnManager.GetPlayerNetworkObject(newHolderId).TryGetComponent(out StarterAssetsInputs player);

        transform.SetParent(player.transform);
        transform.localPosition = Vector3.up;
        timer = explosionTime;
    }

    void Explode()
    {
        Debug.Log($"Boom! Player {currentHolder.Value} ขิต!!!");
        NetworkManager.SpawnManager.GetPlayerNetworkObject(currentHolder.Value).Despawn();
        NetworkObject.Despawn();

        FindObjectOfType<GameManager>().CheckPlayers();
    }
}
