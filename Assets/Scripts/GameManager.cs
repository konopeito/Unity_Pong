using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TMP support
using UnityEngine.UI; // For Button
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public int leftScore = 0;
    public int rightScore = 0;

    public TextMeshProUGUI scoreText; // assign TMP Text in Inspector
    public Transform ball;             // reference to ball to reset

    [Header("Audio")]
    public AudioSource scoreAudio;     // assign score clip here

    [Header("Win Screen")]
    public GameObject winScreen;       // assign WinScreen panel
    public TextMeshProUGUI winText;    // TMP Text inside WinScreen
    public Button restartButton;       // assign restart button

    public int winningScore = 5;

    private Vector2 ballStartPos;
    private bool gameOver = false;

    void Start()
    {
        ballStartPos = ball.position;
        UpdateScoreUI();

        // Hide win screen at start
        if (winScreen != null)
            winScreen.SetActive(false);

        // Setup restart button
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    public void ScorePoint(string side)
    {
        if (!IsServer || gameOver) return; // Only server updates scores

        if (side == "Left") leftScore++;
        else if (side == "Right") rightScore++;

        UpdateScoreUI();

        // Play score sound on all clients
        PlayScoreSoundClientRpc();

        // Check for winner
        if (leftScore >= winningScore)
        {
            ShowWinScreen("Player Left");
        }
        else if (rightScore >= winningScore)
        {
            ShowWinScreen("Player Right");
        }
        else
        {
            ResetBallServerRpc();
        }
    }

    void UpdateScoreUI()
    {
        scoreText.text = leftScore + " - " + rightScore;
    }

    // ServerRpc to reset ball on server, then sync to all clients
    [ServerRpc(RequireOwnership = false)]
    public void ResetBallServerRpc()
    {
        if (ball == null) return;

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        ball.position = ballStartPos;

        // Relaunch ball after short delay
        Invoke(nameof(LaunchBallServerRpc), 1f);
    }

    [ServerRpc(RequireOwnership = false)]
    void LaunchBallServerRpc()
    {
        if (ball == null) return;

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // Random initial direction
        float x = Random.Range(0.5f, 1f) * (Random.value < 0.5f ? -1 : 1);
        float y = Random.Range(-0.5f, 0.5f);

        rb.velocity = new Vector2(x, y).normalized * 5f;
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

    void ShowWinScreen(string winner)
    {
        gameOver = true;

        if (winScreen != null)
            winScreen.SetActive(true);

        if (winText != null)
            winText.text = winner + " Wins!";

        // Stop ball movement
        if (ball != null)
        {
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }
    }

    void RestartGame()
    {
        if (!IsServer) return; // Only server resets scores

        gameOver = false;
        leftScore = 0;
        rightScore = 0;
        UpdateScoreUI();

        if (winScreen != null)
            winScreen.SetActive(false);

        ResetBallServerRpc();
    }
}
