using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    [Header("Audio")]
    public AudioSource bounceAudio;

    private GameManager gm;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = FindObjectOfType<GameManager>();
        LaunchBall();
    }

    public void LaunchBall()
    {
        float x = Random.Range(0.5f, 1f) * (Random.value < 0.5f ? -1 : 1);
        float y = Random.Range(-0.5f, 0.5f);

        rb.velocity = new Vector2(x, y).normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Call OnHit() if object implements ICollidable
        ICollidable collidable = collision.gameObject.GetComponent<ICollidable>();
        if (collidable != null)
        {
            collidable.OnHit(collision);
        }

        // Paddle bounce randomness
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Vector2 velocity = rb.velocity;
            velocity.y += Random.Range(-0.3f, 0.3f);
            rb.velocity = velocity.normalized * speed;

            PlayBounceSound();
        }

        // Wall bounce
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = rb.velocity.normalized * speed;
            PlayBounceSound();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Goal"))
        {
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
            bounceAudio.pitch = Random.Range(0.95f, 1.05f);
            bounceAudio.Play();
        }
    }
}
