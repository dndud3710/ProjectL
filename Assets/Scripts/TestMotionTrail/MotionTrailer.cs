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

        // 새로운 TailContainer 게임오브젝트를 만들어서 Trail 오브젝트들을 관리
        TrailContainer = new GameObject("TrailContainer").transform;
        // TrailContainer.parent = gameObject.transform; // 
        for (int i = 0; i < TrailCount; i++)
        {
            // 원하는 TrailCount만큼 생성
            MeshTrailStruct pss = new MeshTrailStruct();
            pss.Container = Instantiate(MeshTrailPrefab, TrailContainer);
            pss.BodyMeshFilter = pss.Container.transform.GetChild(0).GetComponent<MeshFilter>();
            pss.ClothMeshFilter = pss.Container.transform.GetChild(1).GetComponent<MeshFilter>();
            pss.HeadMeshFilter = pss.Container.transform.GetChild(2).GetComponent<MeshFilter>();

            pss.bodyMesh = new Mesh();
            pss.clothMesh = new Mesh();
            pss.headMesh = new Mesh();

            // 각 mesh에 원하는 skinnedMeshRenderer Bake
            SMR_Body.BakeMesh(pss.bodyMesh);
            SMR_Head.BakeMesh(pss.headMesh);
            SMR_Cloth.BakeMesh(pss.clothMesh);

            // 각 MeshFilter에 알맞은 Mesh 할당
            pss.BodyMeshFilter.mesh = pss.bodyMesh;
            pss.HeadMeshFilter.mesh = pss.headMesh;
            pss.ClothMeshFilter.mesh = pss.clothMesh;

            MeshTrailStructs.Add(pss);

            bodyParts.Add(pss.Container);

            // Material 속성 설정
            float alphaVal = (1f - (float)i / TrailCount) * 0.5f;
            pss.BodyMeshFilter.GetComponent<MeshRenderer>().material.SetFloat("_Alpha", alphaVal);
            pss.ClothMeshFilter.GetComponent<MeshRenderer>().material.SetFloat("_Alpha", alphaVal);
            pss.HeadMeshFilter.GetComponent<MeshRenderer>().material.SetFloat("_Alpha", alphaVal);

            Color tmpColor = Color.Lerp(frontColor, backColor, (float)i / TrailCount); // TODO : 주석 처리된 메서드들은 추후에 shader를 사용할 경우 color를 직접 넣어주는 대신 SetColor를 사용할 것 
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
            print("실행중인 모션 트레일이 없습니다");
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
    /// Trail을 만드는 코루틴
    /// </summary>
    IEnumerator BakeMeshCoroutine()
    {   
        while(!isFinished)
        {
            // Mesh 자체를 Swap하는 것이 아니라 vertices, Triangles를 복사
            // 만약 triangles를 복사하지 않으면 메쉬가 제대로 복사되지 않음
            for (int i = MeshTrailStructs.Count - 2; i >= 0; i--)
            {
                MeshTrailStructs[i + 1].bodyMesh.vertices = MeshTrailStructs[i].bodyMesh.vertices;
                MeshTrailStructs[i + 1].clothMesh.vertices = MeshTrailStructs[i].clothMesh.vertices;
                MeshTrailStructs[i + 1].headMesh.vertices = MeshTrailStructs[i].headMesh.vertices;

                MeshTrailStructs[i + 1].bodyMesh.triangles = MeshTrailStructs[i].bodyMesh.triangles;
                MeshTrailStructs[i + 1].clothMesh.triangles = MeshTrailStructs[i].clothMesh.triangles;
                MeshTrailStructs[i + 1].headMesh.triangles = MeshTrailStructs[i].headMesh.triangles;
            }

            // 첫 번째 것은 새로 Bake해줘야함
            SMR_Body.BakeMesh(MeshTrailStructs[0].bodyMesh);
            SMR_Cloth.BakeMesh(MeshTrailStructs[0].clothMesh);
            SMR_Head.BakeMesh(MeshTrailStructs[0].headMesh);


            // Snake 게임처럼 이전의 position과 rotation을 기억
            posMemory.Insert(0, transform.position);
            rotMemory.Insert(0, transform.rotation);

            // Trail Count를 넘어서면 제거
            if (posMemory.Count > TrailCount)
                posMemory.RemoveAt(posMemory.Count - 1);
            if (rotMemory.Count > TrailCount)
                rotMemory.RemoveAt(rotMemory.Count - 1);
            // 기억해둔 Pos, Rot 할당
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
