using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CreateCharacterMessage : NetworkMessage
{
    public string name;
    public Color playerColor;
}
