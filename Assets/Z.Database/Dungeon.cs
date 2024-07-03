using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "dungeon", menuName = "Dungeon/CreateDungeon", order = 1)]
public class Dungeon : ScriptableObject
{
    public string DungeonName;
    public string DungeonSceneName;

    public Sprite DungeonImage;

    public bool isVisible;
    public bool isOpen;
    public int MaxPlayer;
    public RoomOptions getRoomoption()
    {
        RoomOptions room = new RoomOptions()
        {
            IsVisible = isVisible,
            IsOpen = isOpen,
            MaxPlayers = MaxPlayer,
        };
        return room;
    }
}
