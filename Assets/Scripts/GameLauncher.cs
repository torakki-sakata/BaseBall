using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkObject localGameManager;
    [SerializeField] private NetworkPrefabRef scoreManagerPrefab;
    private NetworkObject localScoreManager;
    [Header("Prefabs & UI")]
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private NetworkPrefabRef playerAvatarPrefab;
    [SerializeField] private NetworkPrefabRef ballPrefab;

    [Header("UI Elements")]
    [SerializeField] private GameObject loadingUI;
    [SerializeField] private Text messageText;
    [SerializeField] private Text scoreText;

    [Header("Score Feedback Texts")]
    [SerializeField] private Text hitText, outText, twoBaseHitText, threeBaseHitText, homeRunText, faulText, straikText, doubleOutText;

    private float feedbackDisplayTime = 1.5f;
    private Dictionary<Text, float> feedbackTimers = new();
    private NetworkRunner runner;
    private bool joinedRoom = false;
    private int playerCount = 0;
    private int localScore = 0;
    private bool isFirstAttack = true; // ① 攻守交代を追跡
    private int currentInning = 1;     // ③ イニング表示用
    private bool isTop = true;         // ③ 表か裏か
    private int score1 = 0;
    private int score2 = 0;

    public static GameLauncher Instance { get; private set; }
    public NetworkPrefabRef PlayerAvatarPrefab => playerAvatarPrefab;
    public NetworkPrefabRef BallPrefab => ballPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 必要に応じて
    }

    private void Update() => UpdateFeedbackTimers();

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        loadingUI?.SetActive(true);
        messageText.text = "";
        scoreText.text = "Score: 0";

        runner = Instantiate(networkRunnerPrefab);
        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        var sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        await runner.JoinSessionLobby(SessionLobby.Shared);

        InitializeFeedbackTimers();
        HideAllFeedbackTexts();
    }

    public async void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if (joinedRoom || runner == null) return;
        joinedRoom = true;

        string sessionName = sessionList.FirstOrDefault(s => s.PlayerCount < 2)?.Name ?? "Room_" + UnityEngine.Random.Range(1000, 9999);
        var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = 2,
            SceneManager = sceneManager
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        playerCount++;

        if (player == runner.LocalPlayer)
        {
            var players = runner.ActivePlayers.ToList();
            players.Sort((a, b) => a.RawEncoded.CompareTo(b.RawEncoded));
            isFirstAttack = runner.LocalPlayer == players[0];

            localScoreManager = runner.Spawn(scoreManagerPrefab, Vector3.zero, Quaternion.identity, player);
            localScoreManager.GetComponent<Score>().OnPitchLimitReached += HandlePitchLimitReached;

            messageText.text = isFirstAttack ? "先攻です！" : "後攻です！";
            StartCoroutine(HideMessageAfterDelay(2f));

            SpawnBallOrBatter();
        }

        if (runner.ActivePlayers.Count() >= 2)
        {
            StartCoroutine(HideLoadingUIAfterDelay(2f));
        }
    }

    private void SpawnBallOrBatter()
    {
        Vector3 spawnPos = isFirstAttack
            ? new Vector3(32.93f, 0f, -0.5f)   // Batter側
            : new Vector3(20.95001f, 0.1999969f, -0.07f); // Ball側

        var prefab = isFirstAttack ? playerAvatarPrefab : ballPrefab;
        runner.Spawn(prefab, spawnPos, Quaternion.identity, runner.LocalPlayer);

        StartCoroutine(DelayedLinkBallAndBatter());
    }

    private void HandlePitchLimitReached()
    {
        // Ball/Batter削除
        var ball = FindObjectOfType<Ball>();
        var batter = FindObjectOfType<Batter>();
        if (ball) Destroy(ball.gameObject);
        if (batter) Destroy(batter.gameObject);

        // スコア保存と表示更新（⑤）
        if (isFirstAttack)
        {
            score1 = localScore;
        }
        else
        {
            score2 = localScore;
        }
        localScore = 0;
        scoreText.text = "Score: 0";

        // 交代処理（①②）
        isFirstAttack = !isFirstAttack;

        // イニング処理（③④）
        if (isTop)
        {
            isTop = false;
        }
        else
        {
            isTop = true;
            currentInning++; // 裏が終わったらイニングを1つ進める
        }

        // Inning表示更新（③④）
        var inningText = GameObject.Find("InningText")?.GetComponent<Text>();
        var halfText = GameObject.Find("InningHalfText")?.GetComponent<Text>();
        if (inningText) inningText.text = currentInning.ToString();
        if (halfText) halfText.text = isTop ? "表" : "裏";

        // 交代テキスト表示（①）
        StartCoroutine(ShowChangeText("攻守交代！"));

        // 再生成（②）
        SpawnBallOrBatter();
    }

    public void AddScore(int points)
    {
        localScore += points;
        scoreText.text = $"Score: {localScore}";
        ShowFeedback(points);
    }

    private IEnumerator DelayedLinkBallAndBatter()
    {
        float timer = 0f, timeout = 5f;
        Ball ball = null;
        Batter batter = null;

        while ((ball == null || batter == null) && timer < timeout)
        {
            ball = FindObjectOfType<Ball>();
            batter = FindObjectOfType<Batter>();
            yield return new WaitForSeconds(0.2f);
            timer += 0.2f;
        }

        if (ball != null && batter != null) batter.SetBall(ball);
    }

    private void ShowFeedback(int points)
    {
        switch (points)
        {
            case 500: ShowText(homeRunText, "ホームラン！"); break;
            case 300: ShowText(threeBaseHitText, "スリーベースヒット！"); break;
            case 200: ShowText(twoBaseHitText, "ツーベースヒット！"); break;
            case 100: ShowText(hitText, "ヒット！"); break;
            case 10: ShowText(faulText, "ファウル"); break;
            case 0: ShowText(straikText, "ストライク"); break;
            case -50: ShowText(outText, "アウト"); break;
            case -100: ShowText(doubleOutText, "ダブルアウト"); break;
        }
    }

    private void ShowText(Text textElement, string message)
    {
        if (textElement == null) return;
        textElement.text = message;
        textElement.enabled = true;
        feedbackTimers[textElement] = 0f;
    }

    private void InitializeFeedbackTimers()
    {
        feedbackTimers = new()
        {
            { hitText, 0f }, { outText, 0f }, { twoBaseHitText, 0f },
            { threeBaseHitText, 0f }, { homeRunText, 0f },
            { faulText, 0f }, { straikText, 0f }, { doubleOutText, 0f }
        };
    }

    private void UpdateFeedbackTimers()
    {
        foreach (var kvp in feedbackTimers.ToList())
        {
            var text = kvp.Key;
            if (text.enabled)
            {
                feedbackTimers[text] += Time.deltaTime;
                if (feedbackTimers[text] > feedbackDisplayTime)
                {
                    text.enabled = false;
                    feedbackTimers[text] = 0f;
                }
            }
        }
    }

    private IEnumerator ShowChangeText(string message)
    {
        var changeText = GameObject.Find("ChangeText")?.GetComponent<Text>();
        if (changeText)
        {
            changeText.text = message;
            changeText.enabled = true;
            yield return new WaitForSeconds(2f);
            changeText.enabled = false;
        }
    }

    private void HideAllFeedbackTexts()
    {
        foreach (var text in feedbackTimers.Keys) text.enabled = false;
    }

    private IEnumerator HideMessageAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        messageText.text = "";
    }

    private IEnumerator HideLoadingUIAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        loadingUI?.SetActive(false);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) => playerCount = Mathf.Max(0, playerCount - 1);
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        GameInput gameInput = new GameInput
        {
            isSwinging = Input.GetMouseButtonDown(0),
            changeStance = Input.GetKeyDown(KeyCode.C),
            rotateLeft = Input.GetKeyDown(KeyCode.A),
            moveUp = Input.GetKey(KeyCode.UpArrow),
            moveDown = Input.GetKey(KeyCode.DownArrow),
            throwBall = Input.GetKeyDown(KeyCode.Alpha1),
            resetBall = Input.GetKeyDown(KeyCode.O),
            throwSlow = Input.GetKeyDown(KeyCode.Alpha2),
            throwSlider = Input.GetKeyDown(KeyCode.Alpha3),
            throwShoot = Input.GetKeyDown(KeyCode.Alpha4),
            throwInvisible = Input.GetKeyDown(KeyCode.V),
            throwStop = Input.GetKeyDown(KeyCode.S),
        };

        input.Set(gameInput);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
