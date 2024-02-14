using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class HsysWaterBl : MonoBehaviour
{
    [Tooltip("分布特征距离")] public float Distance_Factor = 60.0f;
    [Tooltip("时间系数")] public float Time_Factor = -30.0f;
    [Tooltip("周期")] public float Total_Factor = 1.0f;

    [Tooltip("波浪宽度")] public float Wave_Width = 0.3f;
    [Tooltip("波纹扩散的速度")] public float Wave_Speed = 0.3f;

    [SerializeField] private float m_wave_start_time;
    [SerializeField] private Vector4 m_start_position = new Vector4(0.5f, 0.5f, 0, 0);

    [Tooltip("材质")] public Material _Material;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        float curWaveDistance = (Time.time - m_wave_start_time) * Wave_Speed;

        _Material.SetFloat("_DistanceFactor", Distance_Factor);
        _Material.SetFloat("_TimeFactor", Time_Factor);
        _Material.SetFloat("_TotalFactor", Total_Factor);
        _Material.SetFloat("_WaveWidth", Wave_Width);
        _Material.SetFloat("_CurWaveDis", curWaveDistance);
        _Material.SetVector("_StartPos", m_start_position);
        Graphics.Blit(source, destination, _Material);
    }

    

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_Material == null) return;
            Vector2 mousepos = Input.mousePosition;
            m_start_position = new Vector4(mousepos.x / Screen.width, mousepos.y / Screen.height, 0, 0);
            //Graphics.Blit(m_source, m_destination, _Material);
            m_wave_start_time = Time.time;
        }
    }
}
