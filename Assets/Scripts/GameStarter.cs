using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public BombManager bombManager;

    private async void Start()
    {
        var host = new HostGameManager();
        await host.StartHostAsync();

        bombManager.SetHostGameManager(host);  // ส่งไปให้ BombManager
    }
}
