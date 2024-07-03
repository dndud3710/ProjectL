using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum npcType_
{
    talk,
    shop,
    Dungeon,
    Portal
}
public class GameNPC : MonoBehaviour
{
    [SerializeField] string NPCName;
    [SerializeField] int talkid_;
    [SerializeField] npcType_ npctype;
    
    public string NPCName_ => NPCName;
    public int talkid { get { return talkid_; } }
    public npcType_ NpcType { get { return npctype; } }

    bool Talking;

    private void Awake()
    {
    }
    private void Update()
    {

    }
    private void OnEnable()
    {
        Talking = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if ( other.tag == "Player" &&other.transform.parent.TryGetComponent<PhotonView>(out PhotonView t)&&t.IsMine)
        {
            PlayerUIManager.Instance.InteractionOn();
            Talking = true;
            
        }
    }



    public virtual void MakeUI(GameObject g, Transform tr, out IMakeUI makeui, Action Callback = null) { makeui = null; }


    private void OnTriggerStay(Collider other)
    {
        if (Talking&& GameManager.Instance.playerStatus.NpcTalking == false)
        {
            PlayerUIManager.Instance.InteractionOn();
            if (Input.GetKeyDown(KeyCode.F))
            {
                 NPCManager.Instance.StartTalkNpc(talkid, NpcType, this);
                 GameManager.Instance.playerStatus.NpcTalking = true;
                 PlayerUIManager.Instance.InteractionOff();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.transform.parent.TryGetComponent<PhotonView>(out PhotonView t) && t.IsMine)
        {
            PlayerUIManager.Instance.InteractionOff();
            Talking = false;
        }
    }

}
