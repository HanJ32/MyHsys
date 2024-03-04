Shader "Hsys/ZToning/ColorEqualizer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RED ("RED_Color", Vector) = (1.0,1.0,1.0,1.0)
        _Green ("Green_Color", Vector) = (1.0,1.0,1.0,1.0)
        _Blue ("Blue_Color", Vector) = (1.0,1.0,1.0,1.0)
        _RGBC ("_RGBC", Vector) = (1.0,1.0,1.0,1.0)
        _Strength ("Strength", Float) = 1.0
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

        half3 _RED;
        half3 _Green;
        half3 _Blue;
        half3 _RGBC;
        half _Strength;
    ENDCG
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = Hsys::Toning::ColorEqualizer(_MainTex, i.uv,_MainTex_ST, half4(_RED,_RGBC.x), half4(_Green,_RGBC.y), half4(_Blue,_RGBC.z));
                return half4(col.rgb * _Strength, col.a);
            }
            ENDCG
        }
    }
}
