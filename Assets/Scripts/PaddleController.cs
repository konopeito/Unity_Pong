using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PaddleController : NetworkedObject, ICollidable
{
    public float speed = 5f;

    protected virtual void Update()
    {
        float move = GetMovementInput();

        transform.Translate(Vector2.up * move * speed * Time.deltaTime);

        // Clamp inside screen bounds
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, -4.5f, 4.5f);
        transform.position = pos;
    }

    // ABSTRACT method (required by assignment)
    protected abstract float GetMovementInput();

    // ICollidable implementation
    public virtual void OnHit(Collision2D collision)
    {
        // Optional visual feedback
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
        if (sr != null)
            sr.color = Color.white;
    }

    // NetworkedObject requirements
    public override void Initialize() { }

    public override string GetNetworkId()
    {
        return gameObject.name;
    }
}
