using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpbarEffect : MonoBehaviour
{
    public Func<float> InitAction;
    Image img;
    float t, waittime=0.25f;
    private void Awake()
    {
        img = GetComponent<Image>();
    }
    Color c;
    private void OnEnable()
    {
        t = 0;
         c = img.color;
        c.a = 1;
        img.color = c;
        if (InitAction != null )
            img.fillAmount = InitAction.Invoke();
    }
    
    private void Update()
    {
        t += Time.deltaTime;
        if (t > waittime)
        {
            c.a -= Time.deltaTime;

            // 알파 값이 0보다 작아지지 않도록 합니다.
            c.a = Mathf.Max(c.a, 0);

            // 변경된 색상을 이미지에 적용합니다.
            img.color = c;
            if (img.color.a <= 0) gameObject.SetActive(false);
        }
    }
}
