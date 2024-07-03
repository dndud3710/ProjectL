using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    [Header("�ִϸ��̼� ���� �ð�")]
    public float duration = 1f;
    public float moveY = 5f;
    [Header("������ ū �� ����")]
    public float sizeThreshold = 50;
    [Space(20)]

    [Header("ū ������")]
    public float maxDamage;
    public float maxFontSize = 100; // ū �ؽ�Ʈ ������
    public Color largeDmgColor;

    [Header("�Ϲ� ������")]
    public float minDamage;
    public float minFontSize = 50; // ���� �ؽ�Ʈ ������
    public Color smallDmgColor;

    private TextMeshProUGUI damageText;
    private RectTransform rect;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        damageText = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(float damageAmount)
    {
        damageText.text = damageAmount.ToString("0");

        // ������ ũ�⿡ ���� �ؽ�Ʈ ũ��� ���� ����
        float normailizedDamage = (damageAmount - minDamage) / (maxDamage - minDamage);
        normailizedDamage = Mathf.Clamp01(normailizedDamage);

        float fontSize = Mathf.Lerp(minFontSize, maxFontSize, normailizedDamage);
        Color textColor = Color.Lerp(smallDmgColor, largeDmgColor, normailizedDamage);

        damageText.fontSize = fontSize;
        damageText.color = textColor;

        // �ؽ�Ʈ �ִϸ��̼�
        damageText.DOFade(0, duration).OnComplete(() => Destroy(gameObject));
        rect.DOMoveY(rect.position.y + moveY, duration, false);
    }
}
