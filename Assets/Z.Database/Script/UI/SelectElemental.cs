using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SelectElemental : MonoBehaviour
{
    public Func<string> SelectAction;

    [SerializeField] Image[] PartImages;
    RectTransform rectTransform;
    [SerializeField] TextMeshProUGUI elementText;
    [SerializeField] Transform rotMouse;
    [SerializeField] Transform rotPoint;
    EffectType effectype;
    int skilllistid;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = rotMouse.GetComponent<RectTransform>();

    }
    private Vector3 lastMousePosition;
    public float rotationSensitivity = 0.2f; // 회전 감도 설정
    public void Init(int skillistId)
    {
        this.skilllistid = skillistId;
    }
    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 rectPosition = rectTransform.position;
        Vector2 direction = mousePosition - rectPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        rectTransform.rotation = Quaternion.Euler(0, 0, angle);

        if( Input.GetAxis("Mouse X") !=0f|| Input.GetAxis("Mouse Y") != 0f)
        {
            Color c = new Color();
            c.a = 0.7f;
            foreach(Image i in PartImages)
            {
                if (Vector3.Distance(i.transform.position, rotPoint.position) < 170f)
                {
                    c = i.color;
                    c.a = 0.7f;
                    i.color = c; 
                    switch(i.name)
                    {
                        case "Fire":
                            elementText.text = "속성 : 불";
                            effectype = EffectType.Fire;
                            break;
                        case "Ice":
                            elementText.text = "속성 : 얼음";
                            effectype = EffectType.Ice;
                            break;
                        case "Poison":
                            elementText.text = "속성 : 독";
                            effectype = EffectType.Poison;
                            break;
                        case "Lightning":
                            elementText.text = "속성 : 번개";
                            effectype = EffectType.Lightning;
                            break;
                    }
                }
                else
                {
                    c = i.color;
                    c.a = 0.3f;
                    i.color = c;
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            effectype = EffectType.None;
            GameManager.Instance.playerAttack.SetSkillEffectRPC( skilllistid, effectype);
            Destroy(this.gameObject);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(skilllistid);
            Debug.Log(effectype);
            GameManager.Instance.playerAttack.SetSkillEffectRPC(skilllistid, effectype);
            Destroy(this.gameObject);
        }
    }
}
