using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    //Inventory 스크립트에서 getGold함수가 실행되면 Action이 실행되도록 설정
    private void Start()
    {
        Inventory.Instance.getGoldEventHandler += setgoldText;
    }
    //setgoldText함수를 실행하면 goldText에 Inventory의 Gold값을 넣어줌
    private void setgoldText()
    {
        goldText.text = Inventory.Instance.Gold.ToString();
    }
}
