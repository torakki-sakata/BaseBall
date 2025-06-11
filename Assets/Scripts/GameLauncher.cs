using System.Collections;
using System.Collections.Generic;
using System.Linq; // ← 重要
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private GameObject loadingUI;
    [SerializeField] private Text messageText;
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private NetworkPrefabRef scoreNetworkPrefab;

    public NetworkRunner runner { get; private set; }
    private bool joinedRoom = false;
    private bool isFirstAttack = true;
    public static GameLauncher Instance { get; private set; }
    public ShootBall4 shootBall;

    private int pitchCount = 0;
    private int maxPitchCount = 10;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        loadingUI?.SetActive(true);
        messageText.text = "";

        runner = Instantiate(networkRunnerPrefab);
        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        var sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        await runner.JoinSessionLobby(SessionLobby.Shared);
    }

    public async void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if (joinedRoom || runner == null) return;
        joinedRoom = true;

        string sessionName = sessionList.FirstOrDefault(s => s.PlayerCount < 2)?.Name ?? "Room_" + Random.Range(1000, 9999);

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = 2,
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            var players = runner.ActivePlayers.ToList();
            players.Sort((a, b) => a.RawEncoded.CompareTo(b.RawEncoded));
            isFirstAttack = runner.LocalPlayer == players[0];

            messageText.text = isFirstAttack ? "先攻です！" : "後攻です！";

            if (!isFirstAttack)
                StartCoroutine(HideLoadingUIAfterDelay(1f));
        }

        if (ScoreNetwork.Instance == null && runner.IsServer)
        {
            Debug.Log("【GameLauncher】ScoreNetworkのSpawnを試行");
            runner.Spawn(scoreNetworkPrefab, Vector3.zero, Quaternion.identity);
        }

        if (runner.ActivePlayers.Count() >= 2)
        {
            if (isFirstAttack)
            {
                loadingUI?.SetActive(false);
            }

            FindObjectOfType<ShootBall4>()?.StartThrowing();
        }
    }

    private IEnumerator HideLoadingUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        loadingUI?.SetActive(false);
    }

    public void ShowResult()
    {
        if (ScoreNetwork.Instance == null)
        {
            Debug.LogWarning("ScoreNetwork.Instance is null! 結果表示ができません。");
            return;
        }

        int p1 = ScoreNetwork.Instance.Player1Score;
        int p2 = ScoreNetwork.Instance.Player2Score;

        string resultText = p1 > p2 ? "先攻の勝ち！" :
                            p2 > p1 ? "後攻の勝ち！" :
                            "引き分け";

        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = $"{resultText}\n結果：{p1} - {p2}";
        }

        loadingUI?.SetActive(true);
    }

    public void AddLocalScoreFeedback(int score, string resultTag)
    {
        Debug.Log($"表示処理：{resultTag} = {score}");

        string feedback = resultTag switch
        {
            "Hit" => "ヒット！",
            "2B" => "2ベース！",
            "3B" => "3ベース！",
            "HR" => "ホームラン！",
            "Straik" => "ストライク！",
            "Faul" => "ファウル！",
            "Out" => "アウト！",
            "DoubleOut" => "ダブルアウト！",
            _ => ""
        };

        if (!string.IsNullOrEmpty(feedback))
        {
            messageText.gameObject.SetActive(true);
            messageText.text = feedback + $" (+{score}点)";
            StartCoroutine(HideMessageAfterDelay(2f));
        }
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
    }

    // ★ 追加：AddPitchCountAndCheckEnd()
    public void AddPitchCountAndCheckEnd()
    {
        pitchCount++;
        Debug.Log($"現在の投球数: {pitchCount}/{maxPitchCount}");

        if (pitchCount >= maxPitchCount)
        {
            ShowResult();
        }
    }

    // INetworkRunnerCallbacksのその他のコールバック
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
