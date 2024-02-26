Shader "Hsys/ZBlur/Bokeh"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Offset ("Offset", Vector) = (1.0,1.0,1.0,1.0)
        _Blur("Blur", Float) = 1.0
        _Count("Count", Range(1,5)) = 1
        _Redius("Redius", Float) = 1.0
        _PixelSize("PixelSize", Float) = 1.0
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
        half4 _Offset;
        half _Count;
        half _Blur;
        half _Redius;
        half _PixelSize;
    ENDCG
    SubShader
    {
        Name "BOKE"
        Tags { "RenderType"="Opaque" }
        LOD 3020
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct v2f_boke_blur
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            

            v2f_boke_blur vert (appdata v)
            {
                v2f_boke_blur o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_boke_blur i) : SV_Target
            {
                //return fixed4(1.0,1.0,1.0,1.0); 
                
                return Hsys::Blur::Blur_Boke(_MainTex, i.uv, fixed4(_Offset.x,_Offset.y,_Offset.z,_Offset.w), _Redius*_Blur, _PixelSize*_Blur,_Count);
            }
            ENDCG
        }
    }
}
