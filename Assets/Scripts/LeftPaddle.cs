using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftPaddle : PaddleController
{
    protected override float GetMovementInput()
    {
        float move = 0f;

        if (Input.GetKey(KeyCode.W)) move = 1f;
        else if (Input.GetKey(KeyCode.S)) move = -1f;

        return move;
    }
}
