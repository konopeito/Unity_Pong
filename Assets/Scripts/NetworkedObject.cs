using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetworkedObject : MonoBehaviour
{
    public abstract void Initialize();
    public abstract string GetNetworkId();
}
