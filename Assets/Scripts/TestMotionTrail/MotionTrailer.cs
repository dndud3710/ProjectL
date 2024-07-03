using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshTrailStruct
{
    public GameObject Container;

    public MeshFilter BodyMeshFilter;
    public MeshFilter HeadMeshFilter;
    public MeshFilter ClothMeshFilter;

    public Mesh bodyMesh;
    public Mesh headMesh;
    public Mesh clothMesh;
}

public class MotionTrailer : MonoBehaviourPun
{
    #region Variables & Initializer
    [Header("[PreRequisite]")]
    [SerializeField] private SkinnedMeshRenderer SMR_Body;
    [SerializeField] private SkinnedMeshRenderer SMR_Cloth;
    [SerializeField] private SkinnedMeshRenderer SMR_Head;

    private Transform TrailContainer;
    [SerializeField] private GameObject MeshTrailPrefab;
    private List<MeshTrailStruct> MeshTrailStructs = new List<MeshTrailStruct>();

    private List<GameObject> bodyParts = new List<GameObject>();
    private List<Vector3> posMemory = new List<Vector3>();
    private List<Quaternion> rotMemory = new List<Quaternion>();

    [Header("[Trail Info]")]
    [SerializeField] private int TrailCount;
    [SerializeField] private float TrailGap;
    [SerializeField][ColorUsage(true, true)] private Color frontColor;
    [SerializeField][ColorUsage(true, true)] private Color backColor;
    [SerializeField][ColorUsage(true, true)] private Color frontColor_Inner;
    [SerializeField][ColorUsage(true, true)] private Color backColor_Inner;

    float additionalSpeed;
    bool isFinished;

    Coroutine bakeCoroutine;
    Coroutine gradualCompleteCoroutine;

    #endregion

    #region MotionTrail

    void Start()
    {

        // ���ο� TailContainer ���ӿ�����Ʈ�� ���� Trail ������Ʈ���� ����
        TrailContainer = new GameObject("TrailContainer").transform;
        // TrailContainer.parent = gameObject.transform; // 
        for (int i = 0; i < TrailCount; i++)
        {
            // ���ϴ� TrailCount��ŭ ����
            MeshTrailStruct pss = new MeshTrailStruct();
            pss.Container = Instantiate(MeshTrailPrefab, TrailContainer);
            pss.BodyMeshFilter = pss.Container.transform.GetChild(0).GetComponent<MeshFilter>();
            pss.ClothMeshFilter = pss.Container.transform.GetChild(1).GetComponent<MeshFilter>();
            pss.HeadMeshFilter = pss.Container.transform.GetChild(2).GetComponent<MeshFilter>();

            pss.bodyMesh = new Mesh();
            pss.clothMesh = new Mesh();
            pss.headMesh = new Mesh();

            // �� mesh�� ���ϴ� skinnedMeshRenderer Bake
            SMR_Body.BakeMesh(pss.bodyMesh);
            SMR_Head.BakeMesh(pss.headMesh);
            SMR_Cloth.BakeMesh(pss.clothMesh);

            // �� MeshFilter�� �˸��� Mesh �Ҵ�
            pss.BodyMeshFilter.mesh = pss.bodyMesh;
            pss.HeadMeshFilter.mesh = pss.headMesh;
            pss.ClothMeshFilter.mesh = pss.clothMesh;

            MeshTrailStructs.Add(pss);

            bodyParts.Add(pss.Container);

            // Material �Ӽ� ����
            float alphaVal = (1f - (float)i / TrailCount) * 0.5f;
            pss.BodyMeshFilter.GetComponent<MeshRenderer>().material.SetFloat("_Alpha", alphaVal);
            pss.ClothMeshFilter.GetComponent<MeshRenderer>().material.SetFloat("_Alpha", alphaVal);
            pss.HeadMeshFilter.GetComponent<MeshRenderer>().material.SetFloat("_Alpha", alphaVal);

            Color tmpColor = Color.Lerp(frontColor, backColor, (float)i / TrailCount); // TODO : �ּ� ó���� �޼������ ���Ŀ� shader�� ����� ��� color�� ���� �־��ִ� ��� SetColor�� ����� �� 
            //pss.BodyMeshFilter.GetComponent<MeshRenderer>().material.SetColor("_FresnelColor", tmpColor);
            pss.BodyMeshFilter.GetComponent<MeshRenderer>().material.color = tmpColor;
            //pss.ClothMeshFilter.GetComponent<MeshRenderer>().material.SetColor("_FresnelColor", tmpColor);
            pss.ClothMeshFilter.GetComponent<MeshRenderer>().material.color = tmpColor;
            //pss.HeadMeshFilter.GetComponent<MeshRenderer>().material.SetColor("_FresnelColor", tmpColor);
            pss.HeadMeshFilter.GetComponent<MeshRenderer>().material.color = tmpColor;

            Color tmpColor_Inner = Color.Lerp(frontColor_Inner, backColor_Inner, (float)i / TrailCount);
            //pss.BodyMeshFilter.GetComponent<MeshRenderer>().material.SetColor("_BaselColor", tmpColor_Inner);
            //pss.ClothMeshFilter.GetComponent<MeshRenderer>().material.SetColor("_BaselColor", tmpColor_Inner);
            //pss.HeadMeshFilter.GetComponent<MeshRenderer>().material.SetColor("_BaselColor", tmpColor_Inner);

            pss.BodyMeshFilter.GetComponent<MeshRenderer>().material.color = tmpColor_Inner;
            pss.ClothMeshFilter.GetComponent<MeshRenderer>().material.color = tmpColor_Inner;
            pss.HeadMeshFilter.GetComponent<MeshRenderer>().material.color = tmpColor_Inner;
        }

        // StartCoroutine(BakeMeshCoroutine());
        SetTrails(false);
    }

    [PunRPC]
    public void StartMotionTrail()
    {
        ResetBakedMesh();
        bakeCoroutine = StartCoroutine(BakeMeshCoroutine());

        SetTrails(true);
    }
    [PunRPC]
    public void StartMotionTrailRPC()
    {
        photonView.RPC("StartMotionTrail", RpcTarget.All);
    }

    [PunRPC]
    public void FinishMotionTrail()
    {
        if(bakeCoroutine == null)
        {
            print("�������� ��� Ʈ������ �����ϴ�");
            return;
        }

        StopCoroutine(bakeCoroutine);
        gradualCompleteCoroutine = StartCoroutine(GradualCompleteMesh());
        //TrailContainer.gameObject.SetActive(false);
    }
    [PunRPC]
    public void FinishMotionTrailRPC()
    {
        photonView.RPC("FinishMotionTrail", RpcTarget.All);
    }

    /// <summary>
    /// Trail�� ����� �ڷ�ƾ
    /// </summary>
    IEnumerator BakeMeshCoroutine()
    {   
        while(!isFinished)
        {
            // Mesh ��ü�� Swap�ϴ� ���� �ƴ϶� vertices, Triangles�� ����
            // ���� triangles�� �������� ������ �޽��� ����� ������� ����
            for (int i = MeshTrailStructs.Count - 2; i >= 0; i--)
            {
                MeshTrailStructs[i + 1].bodyMesh.vertices = MeshTrailStructs[i].bodyMesh.vertices;
                MeshTrailStructs[i + 1].clothMesh.vertices = MeshTrailStructs[i].clothMesh.vertices;
                MeshTrailStructs[i + 1].headMesh.vertices = MeshTrailStructs[i].headMesh.vertices;

                MeshTrailStructs[i + 1].bodyMesh.triangles = MeshTrailStructs[i].bodyMesh.triangles;
                MeshTrailStructs[i + 1].clothMesh.triangles = MeshTrailStructs[i].clothMesh.triangles;
                MeshTrailStructs[i + 1].headMesh.triangles = MeshTrailStructs[i].headMesh.triangles;
            }

            // ù ��° ���� ���� Bake�������
            SMR_Body.BakeMesh(MeshTrailStructs[0].bodyMesh);
            SMR_Cloth.BakeMesh(MeshTrailStructs[0].clothMesh);
            SMR_Head.BakeMesh(MeshTrailStructs[0].headMesh);


            // Snake ����ó�� ������ position�� rotation�� ���
            posMemory.Insert(0, transform.position);
            rotMemory.Insert(0, transform.rotation);

            // Trail Count�� �Ѿ�� ����
            if (posMemory.Count > TrailCount)
                posMemory.RemoveAt(posMemory.Count - 1);
            if (rotMemory.Count > TrailCount)
                rotMemory.RemoveAt(rotMemory.Count - 1);
            // ����ص� Pos, Rot �Ҵ�
            for (int i = 0; i < bodyParts.Count; i++)
            {
                bodyParts[i].transform.position = posMemory[Mathf.Min(i, posMemory.Count - 1)];
                bodyParts[i].transform.rotation = Quaternion.Euler(rotMemory[Mathf.Min(i, rotMemory.Count - 1)].eulerAngles + new Vector3(-90f, 0, 0));
            }

            yield return new WaitForSeconds(TrailGap / 2);
        }
        
    }

    void ResetBakedMesh()
    {
        posMemory.Clear();
        rotMemory.Clear();
    }

    void SetTrails(bool setOn)
    {
        foreach (Transform trans in TrailContainer)
        {
            trans.gameObject.SetActive(setOn);
        }
    }

    IEnumerator GradualCompleteMesh()
    {
        int count = TrailContainer.childCount;

        while(count > 0)
        {
            TrailContainer.GetChild(count - 1).gameObject.SetActive(false);

            count--;

            yield return new WaitForSeconds(TrailGap);
        }
    }
}
    #endregion
