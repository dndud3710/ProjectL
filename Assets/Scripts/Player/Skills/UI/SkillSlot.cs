using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI key;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image coolImage;
    [SerializeField] private TextMeshProUGUI CostText;
    public Button skillButton;
    int id;
    private SkillInstance skill;

    public void Init(SkillInstance skill, int id)
    {
        this.id = id;
        coolImage.fillAmount = 0;
        CostText.text = skill.skillInfo.manaCost.ToString();
        this.skill = skill;
        iconImage.sprite = skill.skillInfo.icon;
        key.text = skill.skillInfo.bindingKey.ToString();

        skill.OnCooldownStart += HandleCooldownStart;
        skillButton.onClick.AddListener(()=> { skill.Activate(); });
    }

    private void HandleCooldownStart(float duration)
    {
        StartCoroutine(CooldownEffect(duration));
    }

    IEnumerator CooldownEffect(float duration)
    {
        coolImage.fillAmount = 1;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            coolImage.fillAmount = 1 - time / duration;
            yield return null;
        }

        coolImage.fillAmount = 0;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerUIManager.Instance.stateinfoUI.SpawnToolTip(skill.skillInfo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerUIManager.Instance.stateinfoUI.despawnTooltip();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            GameObject g = Instantiate(Resources.Load<GameObject>("UI/Elemental_Select"), PlayerUIManager.Instance.transform);
            g.GetComponent<SelectElemental>().Init(id);
            g.transform.localPosition = Vector2.zero;
        }

    }
}
