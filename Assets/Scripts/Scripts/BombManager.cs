using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BombManager : NetworkBehaviour
{
    [SerializeField] private GameObject timeText;
    public static BombManager Instance { get; private set; } // Singleton เพื่อให้ BombTimer ใช้งานได้

    public static NetworkVariable<ulong> playerWithBomb = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (IsServer)
        {
            ulong randomPlayer = NetworkManager.Singleton.ConnectedClientsList[Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count)].ClientId;
            playerWithBomb.Value = randomPlayer;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        UpdateTimeTextVisibility();
    }

    private void Update()
    {
        UpdateTimeTextVisibility();
    }

    private void UpdateTimeTextVisibility()
    {
        if (timeText != null)
        {
            timeText.SetActive(playerWithBomb.Value == NetworkManager.Singleton.LocalClientId);
        }
    }
}
