Shader "Hsys/ZBloom/HDR"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _BlurTex ("BlurTex", 2D) = "white" {}
        _Color ("Color", Color) = (1.0,1.0,1.0,1.0)
        [HDR] _HDRColor ("Color", Color) = (1.0,1.0,1.0,1.0)
        _Offset ("Offset", Vector) = (1.0,1.0,1.0,1.0)
        _Strength ("Strength", Float) = 1.0
        _Luminance ("Luminance", Float) = 1.0
    }

    CGINCLUDE
        #include "../../../BaseLib/CgincLib/Postprocessing/Bloom.cginc"
        #include "UnityCG.cginc"
        
        struct appdata
        {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
        };

        uniform sampler2D _MainTex;
        uniform float4 _MainTex_ST;
        uniform sampler2D _BlurTex;
        uniform float4 _BlurTex_ST;
        half4 _Color;
        half4 _HDRColor;
        half4 _Offset;
        half _Strength;
        half _Luminance;
    ENDCG
    
    SubShader
    {
        Name "HDR"
        Tags { "RenderType"="Opaque" }
        LOD 3000

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct v2f_getarea
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f_getarea vert (appdata v)
            {
                v2f_getarea o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_getarea i) : SV_Target
            {
                return Hsys::Bloom::GetNeedAreaOfBloom(_MainTex, i.uv,_Luminance);
            }
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct v2f_bloomhdr
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f_bloomhdr vert (appdata v)
            {
                v2f_bloomhdr o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_bloomhdr i) : SV_Target
            {
                return Hsys::Bloom::Bloom_HDRColor_Box(_MainTex,_BlurTex, i.uv,_Offset.xy*0.01, _HDRColor, _Strength);
            }
            ENDCG
        }
    }
}
