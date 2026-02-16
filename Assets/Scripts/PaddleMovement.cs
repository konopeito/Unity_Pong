using UnityEngine;

public class PaddleMovement : PaddleController
{
    [Header("Input Keys")]
    public KeyCode upKey = KeyCode.W;       // Player 1 default
    public KeyCode downKey = KeyCode.S;     // Player 1 default

    protected override float GetMovementInput()
    {
        if (!IsOwner) return 0f;

        float move = 0f;
        if (Input.GetKey(upKey)) move = 1f;
        else if (Input.GetKey(downKey)) move = -1f;

        return move;
    }
}
