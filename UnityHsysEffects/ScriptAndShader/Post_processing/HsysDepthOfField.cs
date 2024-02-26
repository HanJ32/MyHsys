using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HsysDepthOfField : MonoBehaviour
{
    [Tooltip("影响远近")] public float Far = 1000f;
    [Tooltip("影响半径")] public float Near = 0f;
    [Tooltip("模糊系数")] public float Blur = 1.0f;
    [Tooltip("材质")] public Material _Material;


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _Material.SetFloat("_Far", Far);
        _Material.SetFloat("_Near", Near);
        _Material.SetFloat("_Blur", Blur);
        Graphics.Blit(source, destination, _Material);
    }
}

