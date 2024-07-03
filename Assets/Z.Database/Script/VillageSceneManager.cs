using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VillageSceneManager : GameSceneManager<VillageSceneManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        LoadManager.Instance.LoadUI();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J)) { GameManager.Instance.playerStatus.CurHP -= 10; }
    }
}
