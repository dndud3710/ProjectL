using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameDialogue : MonoBehaviour
{
    Coroutine initcoroutine;

    TalkDialogue talkmessage;
    Button button;
    [SerializeField] Button ExitButton;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] TextMeshProUGUI NpcName;
    bool nextText = false; //���� �ؽ�Ʈ�� �Ѿ��

    //1. ���������� �ƴ���
    public void setDialogue(TalkDialogue talk)
    {
        talkmessage = talk;
        NpcName.text = talkmessage.NPCName;
        StartDialogue();
    }
    private void OnEnable()
    {
        messageText.text = string.Empty;
        NpcName.text = string.Empty;
    }
    private void Awake()
    {
        //get Component
        button = GetComponent<Button>();

       
    }
    private void Start()
    {

        button.onClick.AddListener(() => nextText = true); //���� �ؽ�Ʈ�� �Ѿ��
        ExitButton.onClick.AddListener(() => LoadManager.Instance.changeUI(LoadManager.UILoadType.playerui)); //������
        ExitButton.onClick.AddListener(() => SimulationUI.Instance.ExitHandler?.Invoke()); //������
        ExitButton.onClick.AddListener(() => SimulationUI.Instance.ExitHandler = null); //������
        ExitButton.onClick.AddListener(() => GameManager.Instance.playerStatus.NpcTalking=false); //������
       
    }
    
    public void StartDialogue()
    {
        if (talkmessage == null)
        {
            Debug.LogError("talkMessage�� �ƹ��͵� �����ϴ�.! TalkMessage�� �������ּ���");
            return;
        }
        initcoroutine = StartCoroutine(JustTalkCoroutine());
    }

    private IEnumerator JustTalkCoroutine()
    {
        List<string> message = talkmessage.Dialog;
        foreach(string msg in message) //�޼��� ����Ʈ ����
        {
            messageText.text = string.Empty;
            foreach (char c in msg.ToCharArray()) //�޼��� �ѱ��ھ� ���
            {
                messageText.text += c;
                yield return new WaitForSeconds(0.1f);
                if(nextText)
                {
                    messageText.text = msg;
                    break;
                }
            }
            yield return new WaitUntil(()=> nextText); //�����̽��� ���������� ���
            nextText = false;
            
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            nextText = true;
        }
        
    }

    private void OnDisable()
    {
        talkmessage = null;
        if (initcoroutine != null)
        {
            StopCoroutine(initcoroutine);
            initcoroutine = null;
        }
    }
}


[Serializable]
public class TalkDialogue
{
    public int ID; //��ȭ ID
    public string NPCName; //NPC �̸�
    public List<string> Dialog; //��ȭ ����
}