using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLineControl : MonoBehaviour
{
    [SerializeField] Projector projector;

    [SerializeField] float maxWaitTime;
    [SerializeField] float targetRatio;
    [SerializeField] float moveDist;

    int lineIndex;

    FirstPatternAttack rootOwner;

    string particlePath = "Prefabs/Enemy/BossParticle/WheelDrive";
    private void Awake()
    {
        rootOwner = transform.root.GetComponent<FirstPatternAttack>();

        projector.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(lineControlAttackCoroutine());
    }

    public void SetLineIndex(int index)
    {
        lineIndex = index;
    }

    IEnumerator lineControlAttackCoroutine()
    {
        transform.Translate(Vector3.right * moveDist);
        Vector3 instPos = transform.position;

        projector.gameObject.SetActive(true);

        float curWaitTime = 0;
        float startRatio = 0.01f;
        float xOffset = 0;

        while(curWaitTime < maxWaitTime)
        {
            curWaitTime += Time.deltaTime;  
            float t = curWaitTime / maxWaitTime;

            projector.aspectRatio = Mathf.Lerp(startRatio, targetRatio, t);

            xOffset = Mathf.Lerp(0, -moveDist, t);
            projector.transform.localPosition = new Vector3(xOffset, 0, 0);

            yield return null;
        }

        if(PhotonNetwork.IsMasterClient == true)
        {
            Vector3 dir = (rootOwner.transform.position - instPos).normalized;
            GameObject wheel = PhotonNetwork.Instantiate(particlePath, instPos, Quaternion.LookRotation(dir));

            while(true)
            {
                float dist = Vector3.Distance(wheel.transform.position, instPos);

                if(dist >= moveDist * 2)
                {
                    rootOwner.SetLineObjectOnRPC(lineIndex, false);
                    break;
                }

                yield return null;
            }
        }

        
    }
}
