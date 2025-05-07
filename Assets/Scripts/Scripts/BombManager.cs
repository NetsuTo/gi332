using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BombManager : NetworkBehaviour
{
    [SerializeField] private GameObject timeText;
    [SerializeField] private TextMeshProUGUI codeText;
    public static BombManager Instance { get; private set; }

    public static NetworkVariable<ulong> playerWithBomb = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public static NetworkVariable<int> numPlayerReady = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public static NetworkVariable<bool> isGameStart = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private HostGameManager hostGameManager;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        numPlayerReady.OnValueChanged += OnNumPlayerReadyChanged;
        StartCoroutine(WaitForJoinCode());
    }

    private IEnumerator WaitForJoinCode()
    {
        while (string.IsNullOrEmpty(HostGameManager.StaticJoinCode))
        {
            yield return null;
        }

        codeText.text = HostGameManager.StaticJoinCode;
    }
    private void OnNumPlayerReadyChanged(int oldValue, int newValue)
    {
        if (IsServer)
        {
            Debug.Log(newValue);
            if (newValue == NetworkManager.Singleton.ConnectedClientsList.Count)
            {
                isGameStart.Value = true;
                Debug.Log(isGameStart.Value);
                AllPlayerReady();
            }
        }
    }
    private void AllPlayerReady()
    {
        if (IsServer)
        {
            ulong randomPlayer = NetworkManager.Singleton.ConnectedClientsList[Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count)].ClientId;
            playerWithBomb.Value = randomPlayer;
            Debug.Log("player count : " + NetworkManager.Singleton.ConnectedClientsList.Count);
            Debug.Log("player id : " + playerWithBomb.Value);
            Cursor.visible = false;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // UpdateTimeTextVisibility();
    }
    public void SetHostGameManager(HostGameManager manager)
    {
        hostGameManager = manager;
    }
    /*
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
        */
}