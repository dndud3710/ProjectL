using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNPC : GameNPC
{
    [SerializeField] private Dungeon dungeon;
    public Dungeon getDungeon()
    {
        return dungeon;
    }
    public override void MakeUI(GameObject g, Transform tr, out IMakeUI makeui, Action Callback = null)
    {
        GameObject g_= OBPool.Instance.Get(g, tr.position, Quaternion.identity, tr);
        DungeonMatchingUI mu = g_.GetComponent<DungeonMatchingUI>();
        Callback?.Invoke();
        makeui = mu;
    }
}
