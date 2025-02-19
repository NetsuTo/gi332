using Unity.Netcode;
using UnityEngine;

public class GameEndManager : NetworkBehaviour
{
    void Update()
    {
        if (IsServer)
        {
            // ตรวจสอบว่ามีผู้เล่นเหลือแค่คนเดียวหรือไม่
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 1)
            {
                Debug.Log("Game Over! Player Wins!");
            }
        }
    }
}
