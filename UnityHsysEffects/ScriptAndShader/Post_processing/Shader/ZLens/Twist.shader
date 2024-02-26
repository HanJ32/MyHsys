Shader "Hsys/ZLens/Twist"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CenterRadius ("Center_Radius", Vector) = (0.5,0.5, 0.5,0.5) 
        _Angle ("Angle",Float) = 1.0
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
        half _Angle;
    ENDCG
    SubShader
    {
        Name "Lens_Twist"
        ZTest Always Cull Off ZWrite Off
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            struct v2f_twist
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f_twist vert (appdata v)
            {
                v2f_twist o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv - _CenterRadius.xy, _MainTex);
                return o;
            }

            fixed4 frag (v2f_twist i) : SV_Target
            {
                return Hsys::Lens::Lens_Twist(_MainTex, i.uv, _CenterRadius,_Angle);
            }
            ENDCG
        }
    }
}
