using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI loadingtext;
    [SerializeField] TextMeshProUGUI percentagetext;

    float T;

    StringBuilder str;
    int count;

    public Action OnLoadedSceneLoads;
    //�ε� �Ϸ��� Scene �̸� �˱�
    //�ε� ���� room���� 
    private void Awake()
    {
        str = new StringBuilder();
        count = 1;
    }
    private void Start()
    {
        GameStartLoading(LoadManager.Instance.SceneName_);
    }
    string dot = ".";
    private void Update()
    {
            T = T + Time.deltaTime;
            if (T > 0.2f)
            {
                str.Append("Loading");
                for (int i = 0; i < count; i++)
                    str.Append(dot);
                loadingtext.text = str.ToString();
                str.Clear();
                if (count++ > 4)
                    count = 1;
                T = 0;
            }
    }
    private void OnDestroy()
    {
    }
    /// <summary>
    /// ����ó�� ���۷ε�
    /// </summary>
    public void GameStartLoading(string SceneName)
    {
        StartCoroutine(LoadStartScene(SceneName));
    }

    IEnumerator LoadStartScene(string SceneName)
    {

        //loadingBar.value = 0;
        //handle = Addressables.LoadSceneAsync(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        //handle.Completed += (obj) =>
        //{

        //};
        //await handle.Task;
        //Debug.Log("�Ϸ�");    

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        PhotonNetwork.LoadLevel(SceneName);
        //while (PhotonNetwork.LevelLoadingProgress < 1f)
        //{
        //    setValue(PhotonNetwork.LevelLoadingProgress);
        //}
        yield return null;
        //SceneManager.sceneLoaded += OnSceneLoaded;
        //AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName);
        //operation.allowSceneActivation = false; //���ε� �Ŀ� �ڵ����� ���� �Ѿ���°� false��
        
        //while (!operation.isDone)
        //{
        //    yield return null;
        //    if (loadingBar.value < 1f)
        //    {
        //        if (operation.progress >= 0.9f)
        //        {
        //            setValue(Mathf.MoveTowards(loadingBar.value, 1f, Time.deltaTime));
        //        }
        //        else
        //        {
        //            setValue(operation.progress);
        //        }
        //        string percent = (loadingBar.value * 100).ToString("F2");
        //        percentagetext.text = $"{percent}%...";
        //    }
        //    else
        //    {
        //        CompleteMessage("Completed!", "Press SpaceBar");
        //    }
        //    if (Input.GetKeyDown(KeyCode.Space) && loadingBar.value >= 1f)
        //    {
        //        operation.allowSceneActivation = true;
        //    }
        //}

    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode scenemode)
    {

    }
    private void CompleteMessage(string a,string b)
    {
        percentagetext.text = b;
        loadingtext.text = a;
    }
}
