#ifndef __OUT_BORDER_CGINC_
#define __OUT_BORDER_CGINC_

namespace Hsys
{
	namespace Effect3D
	{
		namespace OutBorder
		{
            half4 OutBorderLine(sampler2D tex, float2 uv, float2 ScreenParams, float2 outlinewidth_outlinecolor)
            {
                half4 mtex = tex2D(tex, uv);
                
                // �����Ե����
                half4 outlineCol = half4(0, 0, 0, 0);
                float2 pixelSize = 1.0 / ScreenParams.xy;
                half2 offset[8] =
                {
                    half2(-1, -1), half2(0, -1), half2(1, -1), half2(-1, 0), half2(1, 0),
                    half2(-1, 1), half2(0, 1), half2(1, 1)
                };
                
                float2 muv;
                for (int j = 0; j < 8; j++)
                {
                    muv = uv + offset[j] * outlinewidth_outlinecolor.x * pixelSize;
                    outlineCol += tex2D(tex, uv);
                }
                
                // �����Ե���ز�͸����������Ϊ��߿���ɫ
                if (outlineCol.a > 0)
                {
                    mtex = outlinewidth_outlinecolor.y;
                }
                
                return mtex;
            }
        }
	}
}

#endif
