Shader "Hsys/ZBlur/Directional"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Blur("Blur", Float) = 1.0
        _Params("Params", Vector) = (1.0,1.0,1.0,1.0)
        _Count("Count", Range(1,5)) = 1

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
        half _Blur;
        int _Count;
        half4 _Params;
    ENDCG

    SubShader
    {
        Name "DIRECTIONAL"
        Tags { "RenderType"="Opaque" }
        LOD 3120
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct v2f_directional_blur
            {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            };
            

            v2f_directional_blur vert (appdata v)
            {
                v2f_directional_blur o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f_directional_blur i) : SV_Target
            {
                fixed4 col = Hsys::Blur::Blur_Directional(_MainTex,i.uv, _Params*_Blur, _Count);
                return col;
            }
            ENDCG
        }
    }
}
