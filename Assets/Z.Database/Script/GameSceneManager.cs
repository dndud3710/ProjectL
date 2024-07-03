using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager<T> : MonoBehaviour where T : class
{
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
      
          
    }
    protected virtual void Start()
    {  
        if (PlayerPrefs.GetInt("FirstLogin") == 0)
            LoadManager.Instance.GmaeStartLoadUI();
        if (PlayerUIManager.Instance != null)
            PlayerUIManager.Instance.gameObject.SetActive(true);

    }
    protected virtual void OnDestroy()
    {
        if (PlayerUIManager.Instance != null)
            PlayerUIManager.Instance.gameObject.SetActive(false);
    }


}
