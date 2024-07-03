using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillRune : MonoBehaviour
{
    public StatusEffectData effectData;
    public Text text;
    private Button button;
    private Image image;

    IEnumerator Start()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        text.text = effectData.Name;
        image.sprite = effectData.EffectIcon;

        yield return new WaitUntil(()=>GameManager.Instance != null);
        button.onClick.AddListener(
            () =>
            {
                var manager = GameManager.Instance;
                manager.playerAttack.SetSkillEffectRPC(manager.selectedSkillIndex, effectData.EffectType);
            }
        );
    }
}
