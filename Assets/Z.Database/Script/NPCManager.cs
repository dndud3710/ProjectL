using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager
{
    private static NPCManager instance;
    public static NPCManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NPCManager();
            }
            return instance;
        }
    }
    Dictionary<int, TalkDialogue> talkData;


    public void Init()
    {
        LoadResources();
    }
    private void LoadResources()
    {
        talkData = ConvertJson<Dictionary<int, TalkDialogue>>.ReadJsonFile(EFolderName.Item, "Dialogue");
    }

    public TalkDialogue getNpcMessage(int id)
    {
        return talkData[id];
    }

    public void StartTalkNpc(int id,npcType_ type,GameNPC npc)
    {
        LoadManager.Instance.changeUI(LoadManager.UILoadType.simulationui);
        if(type == npcType_.talk) SimulationUI.Instance.setNpcUI(talkData[id], npc);
        if(type == npcType_.Dungeon) SimulationUI.Instance.setDungeonNpcUI(talkData[id], npc);
        if(type == npcType_.shop) SimulationUI.Instance.setShopNpcUI(talkData[id], npc);
        if(type == npcType_.Portal) SimulationUI.Instance.setPortalNpcUI(talkData[id], npc);
    }

}
