using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float speed = 5f;          // constant ball speed
    private Rigidbody2D rb;

    [Header("Audio")]
    public AudioSource bounceAudio;    // assign in Inspector

    private GameManager gm;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = FindObjectOfType<GameManager>(); // reference GameManager for scoring
        LaunchBall();
    }

    void LaunchBall()
    {
        // Random initial direction
        float x = Random.Range(0.5f, 1f) * (Random.value < 0.5f ? -1 : 1);
        float y = Random.Range(-0.5f, 0.5f);

        rb.velocity = new Vector2(x, y).normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Paddle bounce randomness
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Vector2 velocity = rb.velocity;
            velocity.y += Random.Range(-0.3f, 0.3f);   // slight randomness
            rb.velocity = velocity.normalized * speed;

            PlayBounceSound();
        }

        // Wall bounces (top/bottom)
        if (collision.gameObject.CompareTag("Wall"))
        {
            PlayBounceSound();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
            // Notify GameManager for score and play score sound
            if (gm != null)
            {
                if (collision.gameObject.name == "LeftGoal")
                    gm.ScorePoint("Right");
                else if (collision.gameObject.name == "RightGoal")
                    gm.ScorePoint("Left");

                gm.PlayScoreSound();
            }
        }
    }

    private void PlayBounceSound()
    {
        if (bounceAudio != null)
        {
            bounceAudio.pitch = Random.Range(0.9f, 1.1f); // small variation
            bounceAudio.Play();
        }
    }
}
