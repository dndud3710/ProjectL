using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FirstPatternAttack : MonoBehaviourPun
{
    [SerializeField] List<PatternLineControl> lines;
    [SerializeField] float setOnWaitTime;

    public UnityEvent OnFinishPattern;
    private void OnEnable()
    {
        if(PhotonNetwork.IsMasterClient == true)
        {
            StartCoroutine(SetRandomObjectCoroutine());
        }

    }


    [PunRPC]
    public void SetLineObjectOn(int index, bool isOn)
    {
        if(isOn == true)
        {
            lines[index].gameObject.SetActive(true);
            lines[index].SetLineIndex(index);
        }
        else
        {
            lines[index].gameObject.SetActive(false);

            if(CheckLinePatternEnd() == true)
            {
                OnFinishPattern?.Invoke();
                Destroy(gameObject);
            }
        }
        
    }

    public void SetLineObjectOnRPC(int index, bool isOn)
    {
        photonView.RPC("SetLineObjectOn", RpcTarget.All, index, isOn);
    }


    IEnumerator SetRandomObjectCoroutine()
    {
        System.Random random = new System.Random();

        List<int> nums = new List<int>();  
        int[] arr = new int[lines.Count];
        
        int count = lines.Count;
        
        for (int i = 0; i < count; ++i)
        {
            arr[i] = i;
        }

        while(nums.Count < count)
        {
            int index = random.Next(0, arr.Length);

            if (nums.Contains(index)) continue;
            nums.Add(index);

            SetLineObjectOnRPC(index, true);

            yield return new WaitForSeconds(setOnWaitTime);
        }
    }

    public bool CheckLinePatternEnd()
    {
        foreach(var line in lines)
        {
            if(line.gameObject.activeSelf == true)
            {
                return false;
            }
        }

        return true;
    }
}
