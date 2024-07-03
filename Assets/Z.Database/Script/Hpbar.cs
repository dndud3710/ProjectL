using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hpbar : MonoBehaviour
{
    Slider hpbar;
    [SerializeField] TextMeshProUGUI nickname;
    [SerializeField] HpbarEffect hpeffect;
    float prefHP;
    public HpbarEffect hpbareffect => hpeffect;
    public void Init(float maxhp,float curhp ,string nickname)
    {
        hpbar = GetComponent<Slider>();
        hpbar.maxValue = maxhp;
        this.nickname.text = nickname;
        hpbar.value = curhp;
        hpeffect.InitAction += () => { return hpbar.value / hpbar.maxValue;  };
    }


    public void setHp(float hp)
    {
        hpeffect.gameObject.SetActive(true);
        hpbar.value = hp;
        prefHP = hp;
    }

}
