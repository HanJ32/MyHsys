Shader "Hsys/ZBlur/Box"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TexelSizeX ("TexelSizeX", Float) = 1.0
        _TexelSizeY ("TexelSizeY", Float) = 1.0
        _Blur("Blur", Float) = 1.0
        _Count("Count", Range(1,5)) = 1
        _Redius("Redius", Float) = 1.0
    }

     CGINCLUDE
        #include "../../../BaseLib/CgincLib/Postprocessing/Blur.cginc"
        

        #include "UnityCG.cginc"
        
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        uniform sampler2D _MainTex;
        uniform float4 _MainTex_ST;
        half _TexelSizeX;
        half _TexelSizeY;
        half _Count;
        half _Blur;
        half _Redius;
    ENDCG

    SubShader
    {
        Name "BOX"
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always
        LOD 3000
        Pass
        {
            CGPROGRAM  
            #pragma vertex vert
            #pragma fragment frag
            struct v2f_box_blur
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f_box_blur vert (appdata v)
            {
                v2f_box_blur o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_box_blur i) : SV_Target
            {
                return  Hsys::Blur::Blur_Box(_MainTex, i.uv, float2(_TexelSizeX, _TexelSizeY)*_Blur);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM  
            #pragma vertex vert
            #pragma fragment frag
            struct v2f_box_blur
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f_box_blur vert (appdata v)
            {
                v2f_box_blur o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_box_blur i) : SV_Target
            {
                return Hsys::Blur::Blur_Box(_MainTex, i.uv, float2(_TexelSizeX, _TexelSizeY)*_Blur,1.0);
            }
            ENDCG
        }
        
    }
}