using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamHpUI : MonoBehaviour
{
    [SerializeField] Image ClassAvatar;
    [SerializeField] Hpbar hpbar;
    public void Init(float maxhp,float curhp,string nickname,Sprite avatarimage)
    {
        hpbar.Init(maxhp, curhp,nickname);
        ClassAvatar.sprite = avatarimage;
    }

    public void setHp(float hp)
    {
        hpbar.setHp(hp);
    }
    
    //Instantiate기때문에 초기화 없음
}
