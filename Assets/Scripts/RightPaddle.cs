using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightPaddle : PaddleController
{
    protected override float GetMovementInput()
    {
        float move = 0f;

        if (Input.GetKey(KeyCode.UpArrow)) move = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) move = -1f;

        return move;
    }
}
