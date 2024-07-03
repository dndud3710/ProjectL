using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePhotonManager : MonoBehaviourPunCallbacks
{
    public static InGamePhotonManager Instance
    {
        get; private set;
    }
    private void Awake()
    {
        Instance = this;
        Initialize();
    }
    private void Initialize()
    {

    }
    public override void OnJoinedRoom()
    {

    }
}
