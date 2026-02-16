using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class PaddleController : NetworkBehaviour, ICollidable
{
    public float speed = 5f;

    // NetworkVariable to sync Y position across clients
    public NetworkVariable<float> yPosition = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );

    protected virtual void Update()
    {
        if (IsOwner)
        {
            // Owner moves the paddle
            float move = GetMovementInput();
            transform.Translate(Vector2.up * move * speed * Time.deltaTime);

            // Clamp paddle inside vertical bounds
            Vector3 pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, -4.5f, 4.5f);
            transform.position = pos;

            // Update network variable so other clients see the position
            yPosition.Value = pos.y;
        }
        else
        {
            // Non-owner clients just read the network variable
            Vector3 pos = transform.position;
            pos.y = yPosition.Value;
            transform.position = pos;
        }
    }

    // Abstract method to define input in derived class
    protected abstract float GetMovementInput();

    // ICollidable implementation
    public virtual void OnHit(Collision2D collision)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.yellow;
            Invoke(nameof(ResetColor), 0.1f);
        }
    }

    void ResetColor()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.white;
    }
}
