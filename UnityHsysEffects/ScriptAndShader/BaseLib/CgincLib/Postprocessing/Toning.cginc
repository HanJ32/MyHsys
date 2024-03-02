#ifndef __TONING_CGINC_
#define __TONING_CGINC_

namespace Hsys
{
    namespace Toning
    {
        
        half3 rgb2hsv(half3 color)
        {
            half4 k = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
            half4 p = lerp(half4(color.bg, k.wz), half4(color.gb, k.xy), step(color.b, color.g));
            half4 q = lerp(half4(p.xyw, color.r), half4(color.r, p.yzx), step(p.x, color.r));
            half d = q.x - min(q.w, q.y);
            half e = 1.0e-10;
            half2 res = half2(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e));
            return half3(res, q.x);
        }

        half3 hsv2rgb(half3 color)
        {
            half4 k = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
            half3 p = abs(frac(color.xxx + k.xyz) * 6.0 - k.www);
            return color.z * lerp(k.xxx, clamp(p - k.xxx, 0.0, 1.0), color.y);
        }
        

        half4 Toning_Briginess(sampler2D tex, float2 uv, half strength)
        {
            half4 mtex = tex2D(tex, uv);
            return half4(mtex.rgb * strength,mtex.a);
        }
        
        half4 Toning_ContrastRatio(sampler2D tex, float2 uv, half contrast)
        {
            half4 mtex = tex2D(tex, uv);
            return lerp(0.5h, mtex, contrast);
        }
        
        half4 Toning_Vibrance(sampler2D tex, float2 uv, half range)
        {
            half4 mtex = tex2D(tex, uv);
            return lerp(0.2125 * mtex.r + 0.7154 * mtex.g + 0.0721 * mtex.b, mtex, range);
        }
        
        
        half4 Toning_Level(sampler2D tex, float2 uv, half minInput, half maxInput, half minOutput, half maxOutput ,half gamma)
        {
            half4 mtex = tex2D(tex, uv);
            half3 minout = pow(min(max(mtex.rgb - minInput, half3(0.0, 0.0, 0.0)) / (maxInput - minInput), half3(1.0, 1.0, 1.0)), 1.0 / gamma);
            return half4(lerp(minOutput, maxOutput, minout),1.0);
        }
        //USSSUVA 是 UnityStereoScreenSpaceUVAdjust 处理后的数据
        half4 Toning_Level(sampler2D tex, float2 USSSUVA, half minInput, half maxInput, half minOutput, half maxOutput,half gamma, bool is_VR_AR_MR)
        {
            half4 mtex = tex2D(tex, USSSUVA);
            half3 minout = pow(min(max(mtex.rgb - minInput, half3(0.0, 0.0, 0.0)) / (maxInput - minInput), half3(1.0, 1.0, 1.0)), 1.0 / gamma);
            return half4(lerp(minOutput, maxOutput, minout), 1.0);
        }
        
        half4 ColorGrading(sampler2D tex, float2 uv, half range, half3 hue_saturation_value)
        {
            half4 mtex = tex2D(tex, uv);
            half gray = 0.2125 * mtex.r + 0.7154 * mtex.g + 0.0721 * mtex.b;
            half lumiance = half3(gray, gray, gray);
            mtex.rgb = lerp(mtex.rgb, lumiance, range);
            
            half3 hsv = rgb2hsv(mtex.rgb);
            hsv.x += hue_saturation_value.x;
            hsv.y *= hue_saturation_value.y;
            hsv.z *= hue_saturation_value.z;
            return half4(hsv2rgb(hsv), mtex.a);
        }
        
        half4 ColorEqualizer(sampler2D tex, float2 uv,half4 red, half4 green, half4 blue)
        {
            half4 mtex = tex2D(tex, uv);
            return half4(dot(mtex.rgb, red.rgb) + red.w, dot(mtex.rgb, green.rgb) + green.w, dot(mtex.rgb, blue.rgb) + blue.w, half(1.0));
        }
        
        half4 ColorEqualizer(sampler2D tex, float2 uv, float4 tex_st, half4 red, half4 green, half4 blue)
        {
            uv = uv.xy * tex_st.xy + tex_st.zw;
            half4 mtex = tex2D(tex, uv);
            return half4(dot(mtex.rgb, red.rgb) + red.w, dot(mtex.rgb, green.rgb) + green.w, dot(mtex.rgb, blue.rgb) + blue.w, half(1.0));
        }
    }
}


#endif