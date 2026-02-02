using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TMP support
using UnityEngine.UI; // For Button

public class GameManager : MonoBehaviour
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
        if (gameOver) return; // ignore scoring if game ended

        if (side == "Left") leftScore++;
        else if (side == "Right") rightScore++;

        UpdateScoreUI();

        // Play score sound
        PlayScoreSound();

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
            ResetBall();
        }
    }

    void UpdateScoreUI()
    {
        scoreText.text = leftScore + " - " + rightScore;
    }

    void ResetBall()
    {
        ball.position = ballStartPos;
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;

        // Relaunch the ball after a short delay
        Invoke("LaunchBall", 1f);
    }

    void LaunchBall()
    {
        if (gameOver) return;

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

        // Random initial direction
        float x = Random.Range(0.5f, 1f) * (Random.value < 0.5f ? -1 : 1);
        float y = Random.Range(-0.5f, 0.5f);

        rb.velocity = new Vector2(x, y).normalized * 5f; // consistent speed
    }

    public void PlayScoreSound()
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
        gameOver = false;
        leftScore = 0;
        rightScore = 0;
        UpdateScoreUI();

        if (winScreen != null)
            winScreen.SetActive(false);

        ResetBall();
    }
}
