Shader "Hsys/ZBlur/Iris"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CenterX ("CenterX", Float) = 1.0
        _CenterY ("CenterY", Float) = 1.0
        _Blur("Blur", Float) = 1.0
        _Count("Count", Range(1,5)) = 1
        _Redius("Redius", Float) = 1.0
        _Params("Params", Vector) = (1.0,1.0,1.0,1.0)
        _AreaSize("AreaSize", Float) = 1.0
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
        int _Count;
        half _Blur;
        half _CenterX;
        half _CenterY;
        half _Redius;
        half _AreaSize;
        half4 _Params;

    ENDCG
    SubShader
    {
        Name "IRIS"
        Tags { "RenderType"="Opaque" }
        LOD 3060
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct v2f_iris_blur
            {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            };
            

            v2f_iris_blur vert (appdata v)
            {
                v2f_iris_blur o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_iris_blur i) : SV_Target
            {
               
                return Hsys::Blur::Blur_Iris(_MainTex, i.uv,_Redius,half2(_CenterX,_CenterY),_AreaSize*_Blur,_Params*_Blur,_Count);
            }
            ENDCG
        }
    }
}
