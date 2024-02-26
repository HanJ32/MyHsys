Shader "Hsys/WaterBl"
{
    Properties
    {
        _MainTex ("Base Tex", 2D) = "white" {}
        _DistanceFactor ("Distance Factor", Float) = 1
        _TimeFactor ("Time Factor", Float) = 2
        _TotalFactor ("Total Factor", Float) = 3
        _WaveWidth ("Wave Width", Float) = 4
        _CurWaveDis ("Curwave Distance", Float) = 5
        _StartPos ("Star Position", Vector) = (1,1,1,1)
        _MainTexTexelSize ("MainTex Texel Size", Vector) = (1,1,1,1)
    }
    SubShader
    {
        Name "WaterBl"
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
            FOG {Mode off}


            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            //uniform float4 _MainTex_ST;
            uniform float _DistanceFactor;
            uniform float _TimeFactor;
            uniform float _TotalFactor;
            uniform float _WaveWidth;
            uniform float _CurWaveDis;
            uniform float4 _StartPos;
            uniform float4 _MainTexTexelSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

//            v2f vert (appdata v)
//            {
//                v2f o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
//                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                //o.uv = v.uv;
//                //UNITY_TRANSFER_FOG(o,o.vertex);
//                return o;
//            }

            fixed4 frag (v2f_img i) : SV_Target
            {
                #if UNITY_UV_STARTS_AT_TOP
                if(_MainTexTexelSize.y < 0)
                {
                    _StartPos.y = 1 - _StartPos.y;
                }
                #endif

                float2 dv = _StartPos.xy - i.uv;
                dv = dv * float2(_ScreenParams.x / _ScreenParams.y, 1);
                float dis = sqrt(dv.x*dv.x + dv.y * dv.y);
                float sinfactor = sin(dis * _DistanceFactor + _Time.y * _TimeFactor) * _TotalFactor * 0.01;
                float disfactor = clamp(_WaveWidth - abs(_CurWaveDis - dis), 0, 1) / _WaveWidth;

                float2 dv1 = normalize(dv);

                float2 m_offset = dv1 * sinfactor * disfactor;
                float2 uv = m_offset + i.uv;
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
    FallBack Off
}
