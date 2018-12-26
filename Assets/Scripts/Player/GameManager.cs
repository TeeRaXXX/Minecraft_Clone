using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static int Seed;
    public static string WorldName;

    public static bool IsServer = false;
    public static bool IsHost = false;

    public static void SetGame(bool isServer, bool isHost)
    {
        IsServer = isServer;
        IsHost = isHost;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
