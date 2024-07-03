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
    Dictionary<string, Queue<GameObject>> Spool; // ������� ������Ʈ
    Dictionary<string, List<GameObject>> Susingpool; //�̹� ����ϰ� �ִ� ������Ʈ
    Dictionary<string, GameObject> prefabsCache; // ���ҽ� 
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
            Debug.LogError("AddResource�� prefab�� �߰����ּ���!");
            return;
        }

    }
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject obj = null;
        //������Ʈ Ǯ���� ����ϴ� instantiate
        if (prefabsCache.TryGetValue(prefabId, out obj))// ���ҽ��� �̹� �������
        {
                if (!Susingpool.ContainsKey(prefabId)) //������� Ǯ�� �ش��̸��� ���ο� �ʱ�ȭ�� �ȵ������� �ʱ�ȭ
                {
                    Susingpool.Add(prefabId, new List<GameObject>());
                }
                if (Spool[prefabId].Count > 0) //������� Ǯ�� 0���� Ŭ�� Dequeue
                {
                    obj = Spool[prefabId].Dequeue();
                }
                else //������ �ϳ� �߰��ؼ� Dequeue
                {
                    ReadyResource(prefabId, 1);
                    obj = Spool[prefabId].Dequeue();
                }

                //Dequeue�� obj�� ������� Ǯ�� �߰��ϰ� return
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                Susingpool[prefabId].Add(obj);
                Debug.LogError("Ǯ�����");
                return obj;
            //else //Ǯ���� ������� �ʴ� instantiate
            //{
            //    //���������� ���� ���������� �Ȱǵ� (���ʿ� �����տ��� Activeself�� false�ϰ�� �״�� ����Ѵٴ� �ǹ�)
            //    bool wasActive = obj.activeSelf;
            //    if (wasActive) obj.SetActive(false);
            //    GameObject instance = GameObject.Instantiate(obj, position, rotation);
            //    if (wasActive) obj.SetActive(true);
            //    return instance;
            //}
        }
        else
        {
            Debug.Log("����");
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
            //if (Spool.ContainsKey(prefabId)) //�������� false������ Ǯ�� ���� Ǯ�� ������ ���!
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
        if (!Spool.ContainsKey(gameObject.name)) //Ǯ�� �̸��� ������ destroy
        {
            GameObject.Destroy(gameObject);
            return;
        }
        //Ǯ�� ���� ���
        GameObject obj = null;
        if (Susingpool.ContainsKey(gameObject.name)) //������� Ǯ�� �̸��� ������
        {
            obj = Susingpool[gameObject.name].Find(d => d == gameObject); //�ش������Ʈ�� ã�� Obj�� ĳ���ѵ�
            Susingpool[gameObject.name].Remove(obj); //������� Ǯ������ �������
        }
        obj.SetActive(false); //ĳ���� ������Ʈ�� ����
        Spool[gameObject.name].Enqueue(obj); // ������� Ǯ�� �ִ´�
    }

}
