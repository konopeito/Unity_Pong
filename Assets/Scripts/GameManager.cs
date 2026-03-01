using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public NetworkVariable<int> leftScore = new NetworkVariable<int>(0);
    public NetworkVariable<int> rightScore = new NetworkVariable<int>(0);

    public TextMeshProUGUI scoreText;
    public Transform ball;

    [Header("Paddles")]
    public NetworkObject leftPaddle;
    public NetworkObject rightPaddle;

    [Header("Renderers (for hiding before game starts)")]
    public SpriteRenderer ballRenderer;
    public SpriteRenderer leftPaddleRenderer;
    public SpriteRenderer rightPaddleRenderer;

    [Header("Audio")]
    public AudioSource scoreAudio;

    [Header("Win Screen")]
    public GameObject winScreen;
    public TextMeshProUGUI winText;
    public Button restartButton;

    [Header("UI Panels")]
    public GameObject waitingPanel;
    public GameObject gameUI;

    public int winningScore = 5;
    public float ballSpeed = 5f;

    private Vector2 ballStartPos;
    private bool gameOver = false;
    private bool gameStarted = false;

    void Start()
    {
        ballStartPos = ball.position;

        if (winScreen != null) winScreen.SetActive(false);
        if (gameUI != null) gameUI.SetActive(false);

        SetGameObjectsVisible(false);
        FreezeBall();

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonPressed);
    }

    public override void OnNetworkSpawn()
    {
        leftScore.OnValueChanged += OnScoreChanged;
        rightScore.OnValueChanged += OnScoreChanged;

        UpdateScoreUI();

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    public override void OnNetworkDespawn()
    {
        leftScore.OnValueChanged -= OnScoreChanged;
        rightScore.OnValueChanged -= OnScoreChanged;

        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    void OnClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId) return;

        if (rightPaddle != null)
        {
            rightPaddle.ChangeOwnership(clientId);
        }

        if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2 && !gameStarted)
        {
            gameStarted = true;
            StartGameClientRpc();
        }
    }

    [ClientRpc]
    void StartGameClientRpc()
    {
        gameStarted = true;

        if (waitingPanel != null) waitingPanel.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);

        SetGameObjectsVisible(true);

        // Unfreeze ball on server only
        if (IsServer && ball != null)
        {
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = true;
                rb.isKinematic = false;
            }
        }

        if (IsServer)
        {
            LaunchBall();
        }
    }

    void OnScoreChanged(int oldValue, int newValue)
    {
        UpdateScoreUI();
    }

    public void ScorePoint(string side)
    {
        if (!IsServer || gameOver) return;

        if (side == "Left") leftScore.Value++;
        else if (side == "Right") rightScore.Value++;

        PlayScoreSoundClientRpc();

        if (leftScore.Value >= winningScore)
        {
            ShowWinScreenClientRpc("Player 1");
        }
        else if (rightScore.Value >= winningScore)
        {
            ShowWinScreenClientRpc("Player 2");
        }
        else
        {
            ResetBall();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = leftScore.Value + " - " + rightScore.Value;
    }

    void ResetBall()
    {
        if (!IsServer || ball == null) return;

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        ball.position = ballStartPos;

        Invoke(nameof(LaunchBall), 1f);
    }

    void LaunchBall()
    {
        if (!IsServer || ball == null) return;

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float x = Random.Range(0.5f, 1f) * (Random.value < 0.5f ? -1 : 1);
        float y = Random.Range(-0.5f, 0.5f);

        rb.velocity = new Vector2(x, y).normalized * ballSpeed;
    }

    void SetGameObjectsVisible(bool visible)
    {
        Color show = Color.white;
        Color hide = Color.clear;

        if (ballRenderer != null) ballRenderer.color = visible ? show : hide;
        if (leftPaddleRenderer != null) leftPaddleRenderer.color = visible ? show : hide;
        if (rightPaddleRenderer != null) rightPaddleRenderer.color = visible ? show : hide;
    }

    void FreezeBall()
    {
        if (ball != null)
        {
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
            }
        }
    }

    [ClientRpc]
    public void PlayScoreSoundClientRpc()
    {
        if (scoreAudio != null)
        {
            scoreAudio.pitch = 1f;
            scoreAudio.Play();
        }
    }

    [ClientRpc]
    void ShowWinScreenClientRpc(string winner)
    {
        gameOver = true;

        if (winScreen != null) winScreen.SetActive(true);
        if (winText != null) winText.text = winner + " Wins!";

        if (ball != null)
        {
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }
    }

    [ClientRpc]
    void HideWinScreenClientRpc()
    {
        gameOver = false;

        if (winScreen != null) winScreen.SetActive(false);
    }

    void OnRestartButtonPressed()
    {
        RestartGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void RestartGameServerRpc()
    {
        gameOver = false;
        leftScore.Value = 0;
        rightScore.Value = 0;

        HideWinScreenClientRpc();
        ResetBall();
    }
}