using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    //image는 나중에

    public Action SelectAction;
    int maxClass = 2;

    int classint;

    [Header("변경되어야 할 오브젝트")]
    [SerializeField] TextMeshProUGUI ClassName;
    [SerializeField] TextMeshProUGUI CurrentClassText;
    [SerializeField] TextMeshProUGUI CurrentNicknameText;
    [SerializeField] TMP_InputField input_NickName;
    [SerializeField] Button SelectButton;
    [SerializeField] Image ClassImage;
    [Header("입력 안했을 시 뜨는 패널")]
    [SerializeField] GameObject Panel;
    [SerializeField] Button OKButton;

    List<Sprite> playerImages;

    string[] classNameinfo;
    public int CurrentClass { get; private set; }
    private void Awake()
    {
        classint = 0;
        CurrentClass = -1;
        playerImages=  new List<Sprite>();
    }
    private void Start()
    {
        classNameinfo = new string[] {"WARRIOR","GUNNER","MAGE" };
        SelectButton.onClick.AddListener(SelectClass);
        OKButton.onClick.AddListener(() => Panel.SetActive(false));

        playerImages.Add(Resources.Load<Sprite>("warrior"));
        playerImages.Add(Resources.Load<Sprite>("gunner"));
        playerImages.Add(Resources.Load<Sprite>("mage"));
        ClassImage.sprite = playerImages[classint];
    }

    public void NextClass(bool b)
    {
        if (b==true)
        {
            if (classint < maxClass)
            {
                classint++;
                ClassName.text = classNameinfo[classint];
            }
        }
        else
        {
            if (classint > 0)
            {
                classint--;
                ClassName.text = classNameinfo[classint];
            }
        }
        ClassImage.sprite = playerImages[classint];
    }

    public void SelectClass()
    {
        if (CurrentClass == -1)
        {
            SelectAction?.Invoke();
        }
        CurrentClass = classint;
        CurrentClassText.text = ClassName.text = classNameinfo[classint];
        CurrentNicknameText.text = input_NickName.text;

    }
    public bool ErrorBool()
    {
        if (CurrentClassText.text == string.Empty || CurrentNicknameText.text == string.Empty) return false;
        return true;
    }
    public string GetNickName()
    {
        return CurrentNicknameText.text;
    }
    public void OnErrorPanel()
    {
        Panel.SetActive(true);
    }

}
