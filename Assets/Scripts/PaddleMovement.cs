using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    public float speed = 5f;

    // Keys for this paddle (2 players can have different keys for U/D movement))
    public KeyCode upKey = KeyCode.UpArrow;
    public KeyCode downKey = KeyCode.DownArrow;

    void Update()
    {
        float move = 0f;
        if (Input.GetKey(upKey)) move = 1f;
        else if (Input.GetKey(downKey)) move = -1f;

        transform.Translate(Vector2.up * move * speed * Time.deltaTime);

        // Keep paddle inside vertical bounds
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, -4.5f, 4.5f); // adjust based on wall positions
        transform.position = pos;
    }

}
