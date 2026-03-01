using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    // Networked scores — automatically sync to all clients
    public NetworkVariable<int> leftScore = new NetworkVariable<int>(0);
    public NetworkVariable<int> rightScore = new NetworkVariable<int>(0);

    public TextMeshProUGUI scoreText;
    public Transform ball;

    [Header("Paddles")]
    public NetworkObject leftPaddle;   // drag Left Paddle here
    public NetworkObject rightPaddle;  // drag Right Paddle here

    [Header("Audio")]
    public AudioSource scoreAudio;

    [Header("Win Screen")]
    public GameObject winScreen;
    public TextMeshProUGUI winText;
    public Button restartButton;

    public int winningScore = 5;
    public float ballSpeed = 5f;

    private Vector2 ballStartPos;
    private bool gameOver = false;

    void Start()
    {
        ballStartPos = ball.position;

        if (winScreen != null)
            winScreen.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonPressed);
    }

    public override void OnNetworkSpawn()
    {
        // Listen for score changes on ALL clients
        leftScore.OnValueChanged += OnScoreChanged;
        rightScore.OnValueChanged += OnScoreChanged;

        UpdateScoreUI();

        // Server listens for new players connecting
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

    // When a new client connects, give them ownership of the right paddle
    void OnClientConnected(ulong clientId)
    {
        // Skip the host — host already owns the left paddle
        if (clientId == NetworkManager.Singleton.LocalClientId) return;

        // Give the second player ownership of the right paddle
        if (rightPaddle != null)
        {
            rightPaddle.ChangeOwnership(clientId);
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
            ShowWinScreenClientRpc("Player Left");
        }
        else if (rightScore.Value >= winningScore)
        {
            ShowWinScreenClientRpc("Player Right");
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

        if (winScreen != null)
            winScreen.SetActive(true);

        if (winText != null)
            winText.text = winner + " Wins!";

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

        if (winScreen != null)
            winScreen.SetActive(false);
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