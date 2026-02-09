using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollidable
{
    void OnHit(Collision2D collision);
}
