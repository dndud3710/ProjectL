using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossSkinChanger : MonoBehaviour
{
    public enum SkinType
    {
        Prev, Next
    }

    [SerializeField] SkinnedMeshRenderer[] curMeshRenderers;
    [SerializeField] SkinnedMeshRenderer[] NextMeshRenderers;

    [SerializeField] GameObject nextRenderObject;

    Dictionary<int, List<Material>> nextMats = new();

    private void Awake()
    {
        for(int i = 0; i < NextMeshRenderers.Length; i++)
        {
            int listSize = NextMeshRenderers[i].materials.Length;
            nextMats[i] = new List<Material>(listSize);

            foreach(var material in NextMeshRenderers[i].materials)
            {
                nextMats[i].Add(material);
            }
        }

        nextRenderObject.SetActive(false);
    }

    /*
    [SerializeField]
    SkinnedMeshRenderer
        Arm_Armor, Armor_Belt, Element, Helmet_1, Helmet_2, Helmet_2_Horns, Helmet_3, Helmet_3_Horns, Helmet_4, Knife_Arm, Knife_Leg, Knife_Shoulder,
        Shoulder_Armor_1, Shoulder_Armor_2, Shoulder_Armor_3, Skull_Belt, Skull_Leg, Skull_Shoulder, Skull_Torso, Torso, Visor, ¬³ollar;
    */


    public void ChangeNextSkin()
    {
        for(int i = 0; i < curMeshRenderers.Length; i++)
        {
            curMeshRenderers[i].materials = nextMats[i].ToArray();
        }
    }
}
