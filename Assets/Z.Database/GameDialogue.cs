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
    bool nextText = false; //다음 텍스트로 넘어가기

    //1. 선택지인지 아닌지
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

        button.onClick.AddListener(() => nextText = true); //다음 텍스트로 넘어가기
        ExitButton.onClick.AddListener(() => LoadManager.Instance.changeUI(LoadManager.UILoadType.playerui)); //나가기
        ExitButton.onClick.AddListener(() => SimulationUI.Instance.ExitHandler?.Invoke()); //나가기
        ExitButton.onClick.AddListener(() => SimulationUI.Instance.ExitHandler = null); //나가기
        ExitButton.onClick.AddListener(() => GameManager.Instance.playerStatus.NpcTalking=false); //나가기
       
    }
    
    public void StartDialogue()
    {
        if (talkmessage == null)
        {
            Debug.LogError("talkMessage에 아무것도 없습니다.! TalkMessage를 설정해주세요");
            return;
        }
        initcoroutine = StartCoroutine(JustTalkCoroutine());
    }

    private IEnumerator JustTalkCoroutine()
    {
        List<string> message = talkmessage.Dialog;
        foreach(string msg in message) //메세지 리스트 시작
        {
            messageText.text = string.Empty;
            foreach (char c in msg.ToCharArray()) //메세지 한글자씩 출력
            {
                messageText.text += c;
                yield return new WaitForSeconds(0.1f);
                if(nextText)
                {
                    messageText.text = msg;
                    break;
                }
            }
            yield return new WaitUntil(()=> nextText); //스페이스바 누를때까지 대기
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
    public int ID; //대화 ID
    public string NPCName; //NPC 이름
    public List<string> Dialog; //대화 내용
}