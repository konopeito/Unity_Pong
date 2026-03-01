using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BallMovement : NetworkBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    [Header("Audio")]
    public AudioSource bounceAudio;

    private GameManager gm;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = FindObjectOfType<GameManager>();

        if (!IsServer)
        {
            // Client: keep collider active but don't run physics
            rb.isKinematic = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        ICollidable collidable = collision.gameObject.GetComponent<ICollidable>();
        if (collidable != null)
        {
            collidable.OnHit(collision);
        }

        if (collision.gameObject.CompareTag("Paddle"))
        {
            Vector2 velocity = rb.velocity;
            velocity.y += Random.Range(-0.3f, 0.3f);
            rb.velocity = velocity.normalized * speed;

            PlayBounceSoundClientRpc();
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = rb.velocity.normalized * speed;
            PlayBounceSoundClientRpc();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.CompareTag("Goal"))
        {
            if (gm != null)
            {
                if (collision.gameObject.name == "LeftGoal")
                    gm.ScorePoint("Right");
                else if (collision.gameObject.name == "RightGoal")
                    gm.ScorePoint("Left");
            }
        }
    }

    [ClientRpc]
    void PlayBounceSoundClientRpc()
    {
        if (bounceAudio != null)
        {
            bounceAudio.pitch = Random.Range(0.95f, 1.05f);
            bounceAudio.Play();
        }
    }
}