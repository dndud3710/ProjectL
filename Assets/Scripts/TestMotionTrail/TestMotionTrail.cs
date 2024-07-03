using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMotionTrail : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer[] skinnedRenderers;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    Mesh bakedMesh;
    GameObject ghostContainer;

    float delayTime = 0.5f;
    float nowT = 0.0f;

    private void Start()
    {
        nowT = 0.0f;

        //new GameObject("")

        foreach(var render in skinnedRenderers)
        {
            bakedMesh = new Mesh();
            ghostContainer = new GameObject("VFX_UnitGhost");
            meshFilter = ghostContainer.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = bakedMesh;

            meshRenderer = ghostContainer.AddComponent<MeshRenderer>();
        }

        

        unitGhost();
    }

    public void Update()
    {
        nowT += Time.deltaTime;

        if (delayTime < nowT)
        {
            nowT = 0.0f;

            unitGhost();
        }
    }

    public void unitGhost()
    {
        /*
        skinnedRenderer.BakeMesh(bakedMesh);
        ghostContainer.transform.rotation = skinnedRenderer.gameObject.transform.rotation;
        ghostContainer.transform.position = skinnedRenderer.gameObject.transform.position;
        */
    }
}
