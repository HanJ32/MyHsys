Shader "Hsys/ZLens/Twirl"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  
        _CenterRadius ("Center_Radius", Vector) = (0.5,0.5, 0.5,0.5) 
    }

    CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "../../../BaseLib/CgincLib/Postprocessing/Lens.cginc"
        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };
        uniform sampler2D _MainTex;
        uniform float4 _MainTex_ST;
        uniform float4 _MainTex_TexelSize;
        float4x4 _RotationM;
        half4 _CenterRadius;
    ENDCG
    SubShader
    {
        Name "Lens_Twirl"
        ZTest Always Cull Off ZWrite Off
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            struct v2f_twirl
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f_twirl vert (appdata v)
            {
                v2f_twirl o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv - _CenterRadius.xy;
                return o;
            }

            fixed4 frag (v2f_twirl i) : SV_Target
            {
                return Hsys::Lens::Lens_Twirl(_MainTex, i.uv, _RotationM, _CenterRadius);
            }
            ENDCG
        }
    }
}
