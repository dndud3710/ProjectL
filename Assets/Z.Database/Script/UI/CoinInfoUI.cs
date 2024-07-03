using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;

    //Inventory ��ũ��Ʈ���� getGold�Լ��� ����Ǹ� Action�� ����ǵ��� ����
    private void Start()
    {
        Inventory.Instance.getGoldEventHandler += setgoldText;
    }
    //setgoldText�Լ��� �����ϸ� goldText�� Inventory�� Gold���� �־���
    private void setgoldText()
    {
        goldText.text = Inventory.Instance.Gold.ToString();
    }
}
