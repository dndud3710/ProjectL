using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PhotonNetWorkPool : IPunPrefabPool
{
    //private Dictionary<string, List<GameObject>> poolDictionary;

    //public PhotonNetWorkPool()
    //{
    //    poolDictionary = new Dictionary<string, List<GameObject>>();
    //}

    //public void PreSpawn(string prefabId, int count, Vector3 position = default, Quaternion rotation = default)
    //{
    //    if (poolDictionary.ContainsKey(prefabId) == false)
    //    {
    //        poolDictionary.Add(prefabId, new List<GameObject>());
    //    }

    //    for (int i = 0; i < count; i++)
    //    {
    //        GameObject instance = PhotonNetwork.Instantiate(prefabId, position, rotation);
    //        instance.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.All, false);
    //        poolDictionary[prefabId].Add(instance);
    //    }
    //}

    //public GameObject Spawn(string prefabId, Vector3 position, Quaternion rotation)
    //{
    //    if (poolDictionary.ContainsKey(prefabId) == false)
    //    {
    //        poolDictionary.Add(prefabId, new List<GameObject>());
    //    }

    //    GameObject instance = poolDictionary[prefabId].Find(element => element.activeSelf == false);

    //    if (instance == null)
    //    {
    //        instance = PhotonNetwork.Instantiate(prefabId, position, rotation);
    //        poolDictionary[prefabId].Add(instance);
    //    }
    //    else
    //    {
    //        instance.transform.position = position;
    //        instance.transform.rotation = rotation;
    //        instance.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.All, true);
    //    }

    //    return instance;
    //}

    //public void Despawn(GameObject despawnObject)
    //{
    //    despawnObject.GetComponent<PhotonView>().RPC("SetActiveRPC", RpcTarget.All, false);
    //}
    Dictionary<string, Queue<GameObject>> Spool; // 대기중인 오브젝트
    Dictionary<string, List<GameObject>> Susingpool; //이미 사용하고 있는 오브젝트
    Dictionary<string, GameObject> prefabsCache; // 리소스 
    public PhotonNetWorkPool()
    {
        prefabsCache = new Dictionary<string, GameObject>();
        Spool = new Dictionary<string, Queue<GameObject>>();
        Susingpool = new Dictionary<string, List<GameObject>>();
    }

    public void AddResource(GameObject g)
    {
        prefabsCache.Add(g.name, g);
        ReadyResource(g.name, 1);
    }
    public void AddResource(GameObject g,int count)
    {
        prefabsCache.Add(g.name, g);
        ReadyResource(g.name, count);
    }
    private void ReadyResource(string prefabId, int count)
    {
        GameObject obj = null;
        if (prefabsCache.TryGetValue(prefabId, out obj))
        {
            if (!Spool.ContainsKey(prefabId))
            {
                Spool.Add(prefabId, new Queue<GameObject>());
            }
            obj.SetActive(false);
            for (int i = 0; i < count; i++)
            {
                GameObject instance = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
                instance.name = obj.name;
                Spool[prefabId].Enqueue(instance);
            }
        }
        else
        {
            Debug.LogError("AddResource로 prefab을 추가해주세요!");
            return;
        }

    }
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject obj = null;
        //오브젝트 풀링을 사용하는 instantiate
        if (prefabsCache.TryGetValue(prefabId, out obj))// 리소스가 이미 있을경우
        {
                if (!Susingpool.ContainsKey(prefabId)) //사용중인 풀에 해당이름의 새로운 초기화가 안되있을때 초기화
                {
                    Susingpool.Add(prefabId, new List<GameObject>());
                }
                if (Spool[prefabId].Count > 0) //사용중인 풀이 0보다 클때 Dequeue
                {
                    obj = Spool[prefabId].Dequeue();
                }
                else //없으면 하나 추가해서 Dequeue
                {
                    ReadyResource(prefabId, 1);
                    obj = Spool[prefabId].Dequeue();
                }

                //Dequeue한 obj를 사용중인 풀에 추가하고 return
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                Susingpool[prefabId].Add(obj);
                Debug.LogError("풀링사용");
                return obj;
            //else //풀링을 사용하지 않는 instantiate
            //{
            //    //켜져있으면 끄고 꺼져있으면 안건듦 (애초에 프리팹에서 Activeself가 false일경우 그대로 사용한다는 의미)
            //    bool wasActive = obj.activeSelf;
            //    if (wasActive) obj.SetActive(false);
            //    GameObject instance = GameObject.Instantiate(obj, position, rotation);
            //    if (wasActive) obj.SetActive(true);
            //    return instance;
            //}
        }
        else
        {
            Debug.Log("없음");
            GameObject res = null;
            bool cached = this.prefabsCache.TryGetValue(prefabId, out res);
            if (!cached)
            {
                res = Resources.Load<GameObject>(prefabId);
                if (res == null)
                {
                    Debug.LogError("DefaultPool failed to load \"" + prefabId + "\". Make sure it's in a \"Resources\" folder. Or use a custom IPunPrefabPool.");
                    return null;
                }
                else
                {
                    Debug.Log(prefabId);
                    this.prefabsCache.Add(prefabId, res);
                }
            }
            //if (Spool.ContainsKey(prefabId)) //프리팹이 false상태의 풀에 여유 풀이 존재할 경우!
            //{
            //    if (!Susingpool.ContainsKey(prefabId))
            //    {
            //        Susingpool.Add(prefabId, new List<GameObject>());
            //    }
            //    if (Spool[prefabId].Count > 0)
            //    {
            //        obj = Spool[prefabId].Dequeue();
            //    }
            //    else
            //    {
            //        ReadyResource(prefabId, 1);
            //        obj = Spool[prefabId].Dequeue();
            //    }


            //    obj.transform.position = position;
            //    obj.transform.rotation = rotation;
            //    Susingpool[prefabId].Add(obj);
            //    return obj;
            //}
            {

                bool wasActive = res.activeSelf;
                if (wasActive) res.SetActive(false);

                GameObject instance = GameObject.Instantiate(res, position, rotation);
                if (wasActive) res.SetActive(true);
                return instance;
            }
        }

    }

    public void Destroy(GameObject gameObject)
    {
        if (!Spool.ContainsKey(gameObject.name)) //풀에 이름이 없으면 destroy
        {
            GameObject.Destroy(gameObject);
            return;
        }
        //풀에 있을 경우
        GameObject obj = null;
        if (Susingpool.ContainsKey(gameObject.name)) //사용중인 풀에 이름이 있으면
        {
            obj = Susingpool[gameObject.name].Find(d => d == gameObject); //해당오브젝트를 찾고 Obj에 캐싱한뒤
            Susingpool[gameObject.name].Remove(obj); //사용중인 풀에서는 지운다음
        }
        obj.SetActive(false); //캐싱한 오브젝트를 끄고
        Spool[gameObject.name].Enqueue(obj); // 대기중인 풀에 넣는다
    }

}
