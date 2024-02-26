#ifndef __BLUR_CGINC_
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
//#pragma exclude_renderers d3d11 gles
#define __BLUR_CGINC_

namespace Hsys
{
    namespace Blur
    {
		//#include "../HlslLib/Constant.hlsl"

        half4 Blur_Box(sampler2D tex, float2 uv, float2 texelSize)
        {
		
            float4 d = texelSize.xyxy * half4(-1.0, -1.0, 1.0, 1.0);
		
            half4 s = tex2D(tex, uv);
            s = tex2D(tex, uv + d.xy);
            s += tex2D(tex, uv + d.zy);
            s += tex2D(tex, uv + d.xw);
            s += tex2D(tex, uv + d.zw);
		
            return s / half(4.0);
        }

        half4 Blur_Box(sampler2D tex, float2 uv, float2 texelSize, bool id)
        {
            half4 d = texelSize.xyxy * half4(-1.0, -1.0, 1.0, 1.0);
            half4 s = tex2D(tex, uv);
            s += tex2D(tex, uv + d.yz);
            s += tex2D(tex, uv + d.xy);
            s += tex2D(tex, uv + d.zy);
            s += tex2D(tex, uv + d.zw);

            s += tex2D(tex, uv + half2(0.0, d.w));
            s += tex2D(tex, uv + half2(0.0, d.y));
            s += tex2D(tex, uv + half2(d.z, 0.0));
            s += tex2D(tex, uv + half2(d.x, 0.0));
  
            return s / half(9.0);
        }
        
        half Blur_Box_UseOfBloom(sampler2D tex, float2 uv, float2 offset, bool id)
        {

            half4 d = offset.xyxy * half4(-1.0, -1.0, 1.0, 1.0);
            half4 s = tex2D(tex, uv);
            s += tex2D(tex, uv + d.yz) / 0.4;
            s += tex2D(tex, uv + d.xy) / 0.4;
            s += tex2D(tex, uv + d.zy) / 0.2;
            s += tex2D(tex, uv + d.zw) / 0.2;

            s += tex2D(tex, uv + half2(0.0, d.w)) / 0.1;
            s += tex2D(tex, uv + half2(0.0, d.y)) / 0.1;
            s += tex2D(tex, uv + half2(d.z, 0.0)) / 0.125;
            s += tex2D(tex, uv + half2(d.x, 0.0)) / 0.1;
            
            return s;
        }
		//==================================================
		//Dual Blur

        half4 DownSample(sampler2D tex, float4 texlexelsize, float2 uv)
        {
            half4 offset = texlexelsize.xyxy * float4(-1, -1, 1, 1);
            half4 sum = tex2D(tex, uv) * 4;
            sum += tex2D(tex, uv + offset.xy);
            sum += tex2D(tex, uv + offset.xw);
            sum += tex2D(tex, uv + offset.zy);
            sum += tex2D(tex, uv + offset.zw);

            return sum * 0.125;
        }
        half4 UpSample(sampler2D tex, float4 texlexelsize, float2 uv)
        {
            half4 offset = texlexelsize.xyxy * float4(-1, -1, 1, 1);
            half4 sum = tex2D(tex, uv + float2(offset.x, 0));
            sum += tex2D(tex, uv + float2(offset.z, 0));
            sum += tex2D(tex, uv + float2(0, offset.y));
            sum += tex2D(tex, uv + float2(0, offset.w));
            sum += tex2D(tex, uv + offset.xy / 2.0) * 2.0;
            sum += tex2D(tex, uv + offset.xw / 2.0) * 2.0;
            sum += tex2D(tex, uv + offset.zy / 2.0) * 2.0;
            sum += tex2D(tex, uv + offset.zw / 2.0) * 2.0;
            return sum * 0.0833;
        }

		//===================================
        //散景模糊
        half4 Blur_Boke(sampler2D tex, float2 uv, half4 goldenrot, half radius, half pixelsize, half iteration)
        {
            half2x2 rot = half2x2(goldenrot);
            half4 accumulator = 0.0;
            half4 divisor = 0.0;

            half r = 1.0;
            half2 angle = half2(0.0, radius);

            for (int j = 0; j < iteration; j++)
            {
                r += 1.0 / r;
                angle = mul(rot, angle);
                half4 bokeh = tex2D(tex, float2(uv + pixelsize * (r - 1.0) * angle));
                accumulator += bokeh * bokeh;
                divisor += bokeh;
            }
            return accumulator / divisor;
        }
		//====================================
        //轴模糊
        half4 Blur_TiltShift(sampler2D tex, float2 uv, half radius, half offset,half2 area_spread, half2 params, int count)
        {
            half cosv = cos(2.39996323f);
            half sinv = sin(2.39996323f);
            half2x2 rot = half2x2(cosv, sinv, -sinv, cosv);
            half4 accumulator = 0.0;
            half4 divisor = 0.0;

            half r = 1.0;
			
            float centerY = uv.y * 2.0 - 1.0 + offset;
            half2 angle = half2(0.0, radius * saturate(pow(abs(centerY * area_spread.x), area_spread.y)));

            for (int index = 0; index < count; index += 1)
            {
                r += 1.0 / r;
                angle = mul(rot, angle);
                half4 bokeh = tex2D(tex, float2(uv + params * (r - 1.0) * angle));
                accumulator += bokeh * bokeh;
                divisor += bokeh;
            }
            return accumulator / divisor;
        }
        
        half Blur_TiltShift(sampler2D tex,float4 tex_texelsize, float2 uv, half2 blur_area,int count)
        {
            static const half3 DiscKernel[28] =
            {
                half3(0.62463, 0.54337, 0.82790),
		        half3(-0.13414, -0.94488, 0.95435),
		        half3(0.38772, -0.43475, 0.58253),
		        half3(0.12126, -0.19282, 0.22778),
		        half3(-0.20388, 0.11133, 0.23230),
		        half3(0.83114, -0.29218, 0.88100),
		        half3(0.10759, -0.57839, 0.58831),
		        half3(0.28285, 0.79036, 0.83945),
		        half3(-0.36622, 0.39516, 0.53876),
		        half3(0.75591, 0.21916, 0.78704),
		        half3(-0.52610, 0.02386, 0.52664),
		        half3(-0.88216, -0.24471, 0.91547),
		        half3(-0.48888, -0.29330, 0.57011),
		        half3(0.44014, -0.08558, 0.44838),
		        half3(0.21179, 0.51373, 0.55567),
		        half3(0.05483, 0.95701, 0.95858),
		        half3(-0.59001, -0.70509, 0.91938),
		        half3(-0.80065, 0.24631, 0.83768),
		        half3(-0.19424, -0.18402, 0.26757),
		        half3(-0.43667, 0.76751, 0.88304),
		        half3(0.21666, 0.11602, 0.24577),
		        half3(0.15696, -0.85600, 0.87027),
		        half3(-0.75821, 0.58363, 0.95682),
		        half3(0.99284, -0.02904, 0.99327),
		        half3(-0.22234, -0.57907, 0.62029),
		        half3(0.55052, -0.66984, 0.86704),
		        half3(0.46431, 0.28115, 0.54280),
		        half3(-0.07214, 0.60554, 0.60982),
            };
            half4 sum = tex2D(tex, uv);
            half2 tapCoord = (uv * 2.0 - 1.0);

            half w = clamp(dot(tapCoord, tapCoord) * blur_area.y, 0, blur_area.x);

            half4 ps = tex_texelsize.xyxy * w;

            for (int l = 0; l < count; l += 1)
            {
                float4 s = tex2D(tex, uv + DiscKernel[l].xy * ps.xy);
                sum += s;
            }
            return float4(sum.rgb / (29.0), w);
        }
        
        //=======================================
        //光圈模糊
        half4 Blur_Iris(sampler2D tex, float2 uv, half radius, half2 pcenter, half area, half2 pixelsize, int count)
        {
            half cosv = cos(2.39996323f);
            half sinv = sin(2.39996323f);
            half2x2 rot = half2x2(cosv, sinv, -sinv, cosv);
            half4 accumulator = 0.0;
            half4 divisor = 0.0;
            
            half r = 1.0;
            half2 center = uv * 2.0 - 1.0 + pcenter;
            
            half2 angle = half2(0.0, radius * saturate(dot(center, center) * area));

            for (int index = 0; index < count; index += 1)
            {
                r += 1.0 / r;
                angle = mul(rot, angle);
                half4 bokeh = tex2D(tex, float2(uv + pixelsize * (r - 1.0) * angle));
                accumulator += bokeh * bokeh;
                divisor += bokeh;
            }
            return accumulator / divisor;
        }

        //========================================
        //粒度模糊
        half4 Blur_Grainy(sampler2D tex, float2 uv, half blur, int count)
        {
            half2 randomoffset = float2(0.0, 0.0);
            half4 finalcolor = half4(0.0, 0.0, 0.0, 0.0);
            float random = sin(dot(uv, half2(1233.224, 1743.335)));
            
            for (int index = 0; index < count; index += 1)
            {
                random = frac(43758.5453 * random + 0.61432);
                randomoffset.x = (random - 0.5) * 2.0;
                random = frac(43758.5453 * random + 0.61432);
                randomoffset.y = (random - 0.5) * 2.0;

                finalcolor += tex2D(tex, half2(uv + randomoffset * blur));
            }
            return finalcolor / count;
        }
        
        //=======================================
        //径向模糊
        half4 Blur_Radial(sampler2D tex, float2 uv, half4 params)
        {
            float2 blurv = (params.zw - uv.xy) * params.x;
            half4 acumulatecolor = half4(0, 0, 0, 0);
            
            [unroll(30)]
            for (int index = 0; index < params.y; index+=1)
            {
                acumulatecolor += tex2D(tex, uv);
                uv.xy += blurv;
            }
            
            return acumulatecolor / params.y;
        }
        
        half4 Blur_Radial(sampler2D tex, float2 uv, half4 params, half redius,half2 center)
        {
            half2 blurParams = distance(uv, center);
            half2 blurv = (params.zw - uv.xy) * params.x;
            half4 acumulatecolor = half4(0, 0, 0, 0);
            half ats = saturate(blurParams / redius) * 0.1;
            [unroll(30)]
            for (int index = 0; index < params.y; index += 1)
            {
                acumulatecolor += tex2D(tex, uv);
                uv.xy += blurv * ats * index;

            }
            
            return acumulatecolor / params.y;
        }
        //=======================================
        //方向模糊
        half4 Blur_Directional(sampler2D tex, float2 uv, half4 params, int count)
        {
            half4 color = half4(0.0, 0.0, 0.0, 0.0);
            for (int index = -count; index < count; index += 1)
            {
                color += tex2D(tex, uv - params.yz * index);
            }
            half4 result = color / (half(count) * 2.0);
            return result;
        }
    }
}

#endif