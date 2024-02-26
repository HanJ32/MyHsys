Shader "Hsys/DepthOfField"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Far ("Far", Float) = 1.0
        _Near("Near", Float) = 1.0
        _Blur("Blur", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv[9] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _CameraDepthTexture;
            float _Near;
			float _Far;
			float _BlurSize;
            sampler2D _MainTex;
			float4 _MainTex_TexelSize;
            v2f vert (appdata v)
            {
                v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv[0] = v.uv;
				o.uv[1] = v.uv + _MainTex_TexelSize.xy * float2(-1, -1) * _BlurSize;
				o.uv[2] = v.uv + _MainTex_TexelSize.xy * float2(0, -1) * _BlurSize;
				o.uv[3] = v.uv + _MainTex_TexelSize.xy * float2(1, -1) * _BlurSize;
				o.uv[4] = v.uv + _MainTex_TexelSize.xy * float2(0, -1) * _BlurSize;
				o.uv[5] = v.uv + _MainTex_TexelSize.xy * float2(0, 1) * _BlurSize;
				o.uv[6] = v.uv + _MainTex_TexelSize.xy * float2(1, -1) * _BlurSize;
				o.uv[7] = v.uv + _MainTex_TexelSize.xy * float2(1, 0) * _BlurSize;
				o.uv[8] = v.uv + _MainTex_TexelSize.xy * float2(1, 1) * _BlurSize;
				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv[0]);
				float z = LinearEyeDepth(d);
				float blur = (1 - sign(z - _Near) * sign(_Far - z)) * 8 ;
				float count = blur + 1;
				float4 sum = tex2D(_MainTex, i.uv[0]) / count;

				sum *= 1 + 2 * blur / 8; 
				for(int j = 1; j < count; j++)
				{
					sum += tex2D(_MainTex, i.uv[j]) * 0.0875;
				}
				float4 color = sum;

				return color;
            }
            ENDCG
        }
    }
}
