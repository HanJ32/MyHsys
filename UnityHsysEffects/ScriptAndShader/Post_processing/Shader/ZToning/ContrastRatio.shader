Shader "Hsys/ZToning/ContrastRatio"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Contrast ("Contrast", Float) = 1.0
    }

    CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "../../../BaseLib/CgincLib/Postprocessing/Toning.cginc"
        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        half _Contrast;
    ENDCG
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            struct v2f_contrastratio
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            

            v2f_contrastratio vert (appdata v)
            {
                v2f_contrastratio o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_contrastratio i) : SV_Target
            {
                return Hsys::Toning::Toning_ContrastRatio(_MainTex,i.uv,_Contrast);
            }
            ENDCG
        }
    }
}
