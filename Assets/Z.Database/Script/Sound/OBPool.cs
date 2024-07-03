using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using UnityEngine.VFX;

/// <summary>
/// 이슈
/// 1. tooltip의 transform (가려지는거때문에)
/// 2. obpool의 uispawn만들게 된 계기
/// 
/// </summary>
public class OBPool  : MonoBehaviour
{
    public static OBPool Instance { get; private set; }

     Dictionary<string, ObjectPool<GameObject>> pool = new Dictionary<string, ObjectPool<GameObject>>();
    Dictionary<string,GameObject> UiPool = new Dictionary<string,GameObject>();


    List<WindowUI> WindowUIstack;
    //DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
    //if (pool != null && objects != null)
    //{
    //    foreach (GameObject prefab in objects)
    //    {
    //        pool.ResourceCache.Add(prefab.name, prefab);

    //    }
    //}

    ////어떤시점에 원래 PhotonNetwork.Instantiate("test1");

    //pool.Instantiate("Enemy", Vector3.zero, Quaternion.identity);
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            WindowUIstack = new List<WindowUI>();
        }
        else
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    public GameObject Get(GameObject g, Vector2 pos,Quaternion rot, Transform t= null, Action callback = null)
    {
        string str = g.name;
        if (!pool.ContainsKey(str))
        {
            CreatePool(g,str);
        }
        GameObject obj = pool[str].Get();
        if (t == null)
            obj.transform.SetParent(transform);
        else
        {
            obj.transform.SetParent(t);
        }
        obj.transform.position = pos;
        return obj;
       
        #region 안씀
        //GameObject obj = pool[str].Dequeue();
        //if (usingpool == null) usingpool = new Dictionary<string, List<GameObject>>();
        //if (!usingpool.ContainsKey(str))
        //{
        //    usingpool.Add(str, new List<GameObject>());
        //}
        //usingpool[str].Add(obj);
        //obj.gameObject.SetActive(true);
        //obj.transform.position = pos;
        //obj.transform.rotation = rot;
        //callback?.Invoke();
        //return obj;
        #endregion
    }
    public GameObject Get(GameObject g,Transform t = null, Action callback = null)
    {
        string str = g.name;
        if (!pool.ContainsKey(str))
        {
            CreatePool(g, str);
        }
        GameObject obj = pool[str].Get();

        if (t != null)
            obj.transform.SetParent(t);
        return obj;

    }
    public GameObject Get<T>(GameObject g, Transform t = null, Action callback = null)
    {
        string str = g.name;
        if (!pool.ContainsKey(str))
        {
            CreatePool(g, str);
        }
        GameObject obj = pool[str].Get();
        
        if (t != null)
            obj.transform.SetParent(t);
        return obj;

    }
    public GameObject Get<T>(GameObject g, out T cls, Transform t = null, Action callback = null)
    {
        string str = g.name;
        if (!pool.ContainsKey(str))
        {
            CreatePool(g, str);
        }
        GameObject obj = pool[str].Get();
        if (t!= null)
            obj.transform.SetParent(transform);
        else
        {
            obj.transform.SetParent(t);
        }
        obj.TryGetComponent<T>(out cls);
        return obj;

    }
    public GameObject Get<T>(GameObject g, Vector2 pos, Quaternion rot,out T cls, Transform t = null, Action callback = null)
    {
        string str = g.name;
        if (!pool.ContainsKey(str))
        {
            CreatePool(g, str);
        }
        GameObject obj = pool[str].Get();

        if (t != null)
            obj.transform.SetParent(t);
        else
            obj.transform.SetParent(t);
        obj.transform.position = pos;
        obj.TryGetComponent<T>(out cls);
        return obj;
    }
    /// <summary>
    /// 이곳저곳에서 UI를 켜야할때, transform 위치때문에 만듦
    /// </summary>
    /// <param name="g"></param>
    /// <param name="t"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public GameObject GetUI(GameObject g,Vector2 pos,Transform t = null, Action callback = null)
    {
        string str = g.name; 
        
        if (!UiPool.ContainsKey(str))
        {
            GameObject g_ = Instantiate(g);
            g_.name = str;
            UiPool.Add(str, g_);
        }
        GameObject obj = UiPool[str];

        if(obj.TryGetComponent<WindowUI>(out WindowUI ui_))
        {
            WindowUIstack.Add(ui_);
        }
        //if(obj.TryGetComponent<PopupWindow>)
        obj.SetActive(true);
        obj.transform.SetParent(t,false); 
        g.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        return obj;
    }
    public void UIdespawn(GameObject g)
    {
        string str = g.name;
        if (!UiPool.ContainsKey(str))
        {
            return;
        }
        if (g.TryGetComponent<WindowUI>(out WindowUI ui_))
        {
            WindowUIstack.Remove(WindowUIstack.Find((d) => d == ui_));
        }
        
        UiPool[str].SetActive(false);
    }
    public void UIdespawn(GameObject g,bool f)
    {
        string str = g.name;
        if (!UiPool.ContainsKey(str))
        {
            Debug.LogError($"{str}오브젝트는 UIpool에 없습니다!");
            return;
        }
        UiPool[str].SetActive(false);
    }
    private void CreatePool(GameObject prefab,string key)
    {
        ObjectPool<GameObject> pool_ = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = Instantiate(prefab);
                obj.gameObject.name = key;

                return obj;
            },
            actionOnGet: (GameObject obj) =>
            {
                obj.gameObject.SetActive(true);
            },
            actionOnRelease: (GameObject obj) =>
            {
                obj.gameObject.SetActive(false);
            },
            actionOnDestroy: (GameObject obj) =>
            {
                Destroy(obj);
            }
            );
            pool.Add(key, pool_);
    }

    public void Despawn(GameObject g)
    {
        if (g.activeSelf == true)
        {
            String str = g.gameObject.name;
            if (pool.ContainsKey(str))
            {
                pool[str].Release(g);
            }
        }
        #region 안씀
        //if (!pool.ContainsKey(str))
        //{

        //    pool.Add(str, new Queue<GameObject>());
        //}



        //if (usingpool.ContainsKey(str))
        //{
        //    obj = usingpool[str].Find(d => d == g);
        //}
        //if (obj == null)
        //{
        //    if (tr != null)
        //    {
        //        obj = GameObject.Instantiate(g, tr);

        //    }
        //    else
        //    {
        //        obj = GameObject.Instantiate(g);
        //    }
        //    obj.name = str;
        //}
        //else
        //{
        //    usingpool[str].Remove(obj);
        //}
        //obj.SetActive(false);
        //pool[str].Enqueue(obj);
        #endregion
    }
    private IEnumerator TimeDespawn(GameObject g,float time)
    {
        yield return new WaitForSeconds(time);
        if (g.activeSelf == true)
        {
            String str = g.gameObject.name;
            if (pool.ContainsKey(str))
            {
                pool[str].Release(g);
            }
        }
    }
    public void Despawn(GameObject g , float time)
    {
        StartCoroutine(TimeDespawn(g, time));
    }


        private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (WindowUIstack.Count > 0)
            {
                WindowUIstack[WindowUIstack.Count - 1].Disable();
                WindowUIstack.RemoveAt(WindowUIstack.Count - 1);
            }
        }
    }
}
