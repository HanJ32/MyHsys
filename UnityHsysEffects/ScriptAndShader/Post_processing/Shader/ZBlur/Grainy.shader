Shader "Hsys/ZBlur/Grainy"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Blur("Blur", Float) = 1.0
        _Count("Count", Range(1,5)) = 1
        _Redius("Redius", Float) = 1.0
        _Effect("Effect", Float) = 1.0
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
        half _Count;
        half _Blur;
        half _Redius;
        half _Effect;
    ENDCG
    SubShader
    {
        Name "GRAINY"
        Tags { "RenderType"="Opaque" }
        LOD 3080
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct v2f_grainy_blur
            {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            };
            

            v2f_grainy_blur vert (appdata v)
            {
                v2f_grainy_blur o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_grainy_blur i) : SV_Target
            {
                
                return Hsys::Blur::Blur_Grainy(_MainTex,i.uv,_Blur*_Effect,_Count);
            }
            ENDCG
        }
    }
}
