using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectChanger : MonoBehaviour
{
    public ParticleSystem[] effectChangeParticles;

    public Material[] materials;
    public MeshRenderer[] meshRenderers;

    public void SetParticleColor(StatusEffectData effectData)
    {
        var newColor = effectData.EffectColor;
        
        foreach(var effect in effectChangeParticles)
        {
            var main = effect.main;
            var alpha = main.startColor.color.a;
            main.startColor = new Color(newColor.r, newColor.g, newColor.b, alpha);
        }

        foreach (var renderer in meshRenderers)
        {
            renderer.material = materials[(int)effectData.EffectType];
        }
    }
}
