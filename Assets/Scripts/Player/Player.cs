using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerData _playerData;

    public PlayerData PlayerData
    {
        get { return _playerData; }
        set { _playerData = value; }
    }
}
