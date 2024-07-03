using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDungeonManager : MonoBehaviourPunCallbacks
{
    public static TestDungeonManager Instance;

    public Sprite[] iconSprites;

    public RectTransform teamHPBarParent;
    public GameObject HPBarPrefab;

    private Dictionary<int, GameObject> HPBarDictionary;

    private void Awake()
    {
        Instance = this;
        HPBarDictionary = new Dictionary<int, GameObject>();
    }

    private void Start()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            
        }
    }
}
