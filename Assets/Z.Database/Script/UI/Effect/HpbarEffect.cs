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

            // ���� ���� 0���� �۾����� �ʵ��� �մϴ�.
            c.a = Mathf.Max(c.a, 0);

            // ����� ������ �̹����� �����մϴ�.
            img.color = c;
            if (img.color.a <= 0) gameObject.SetActive(false);
        }
    }
}
