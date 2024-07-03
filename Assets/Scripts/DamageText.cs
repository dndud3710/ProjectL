using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageText : MonoBehaviour
{
    [Header("애니메이션 지속 시간")]
    public float duration = 1f;
    public float moveY = 5f;
    [Header("데미지 큰 값 기준")]
    public float sizeThreshold = 50;
    [Space(20)]

    [Header("큰 데미지")]
    public float maxDamage;
    public float maxFontSize = 100; // 큰 텍스트 사이즈
    public Color largeDmgColor;

    [Header("일반 데미지")]
    public float minDamage;
    public float minFontSize = 50; // 작은 텍스트 사이즈
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

        // 데미지 크기에 따라 텍스트 크기와 색상 설정
        float normailizedDamage = (damageAmount - minDamage) / (maxDamage - minDamage);
        normailizedDamage = Mathf.Clamp01(normailizedDamage);

        float fontSize = Mathf.Lerp(minFontSize, maxFontSize, normailizedDamage);
        Color textColor = Color.Lerp(smallDmgColor, largeDmgColor, normailizedDamage);

        damageText.fontSize = fontSize;
        damageText.color = textColor;

        // 텍스트 애니메이션
        damageText.DOFade(0, duration).OnComplete(() => Destroy(gameObject));
        rect.DOMoveY(rect.position.y + moveY, duration, false);
    }
}
