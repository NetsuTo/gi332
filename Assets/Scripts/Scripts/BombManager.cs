using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BombManager : NetworkBehaviour
{
    [SerializeField] private GameObject winPanel;  // Panel สำหรับ Win
    [SerializeField] private GameObject losePanel; // Panel สำหรับ Lose
    [SerializeField] private GameObject timeText;
    [SerializeField] private TextMeshProUGUI codeText;
    public static BombManager Instance { get; private set; }

    public static NetworkVariable<ulong> playerWithBomb = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public static NetworkVariable<int> numPlayerReady = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public static NetworkVariable<bool> isGameStart = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private HostGameManager hostGameManager;

    public static int countPlayer = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        numPlayerReady.OnValueChanged += OnNumPlayerReadyChanged;
        StartCoroutine(WaitForJoinCode());
    }

    void Update()
    {
        if (IsClient)
        {
            if (countPlayer == 1)
            {
                // ผู้เล่นคนสุดท้ายชนะ (แสดงหน้า Win)
                if (isGameStart.Value)
                {
                    ShowWinUI();
                }
            }
        }
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
            if (newValue == NetworkManager.Singleton.ConnectedClientsList.Count)
            {
                isGameStart.Value = true;
                AllPlayerReady();
            }
        }
    }

    private void AllPlayerReady()
    {
        if (IsServer && countPlayer > 1)
        {
            ulong randomPlayer = NetworkManager.Singleton.ConnectedClientsList[Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count)].ClientId;
            playerWithBomb.Value = randomPlayer;
            countPlayer = NetworkManager.Singleton.ConnectedClientsList.Count;
        }
    }

    // ฟังก์ชันแสดงหน้า Win
    private void ShowWinUI()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true); // แสดง UI Win
        }
    }

    // ฟังก์ชันแสดงหน้า Lose
    private void ShowLoseUI()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true); // แสดง UI Lose
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกเมื่อผู้เล่นตาย
    public void PlayerDied()
    {
        if (IsServer)
        {
            ShowLoseUI();  // แสดงหน้า Lose สำหรับผู้เล่นที่ตาย
        }
    }

    public void SetHostGameManager(HostGameManager manager)
    {
        hostGameManager = manager;
    }
}