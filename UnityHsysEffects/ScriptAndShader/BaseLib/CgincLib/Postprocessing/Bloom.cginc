#ifndef __BLOOM_CGINC_
#define __BLOOM_CGINC_

namespace Hsys
{
    #include "./Blur.cginc"
    namespace Bloom
    {
        half4 GetNeedAreaOfBloom(sampler2D tex, float2 uv, half lu)
        {
            half4 mtex = tex2D(tex, uv);
            half l = clamp(0.2125 * mtex.r + 0.7154 * mtex.g + 0.0721 * mtex.b, 0.0, 1.0);
            
            //mtex = half4(mtex.rgb * (half(1) - mtex.a) * half(3), 1.0);
            return mtex * l * lu;
        }
        //HDR Bloom
        half4 Bloom_HDRColor(sampler2D tex,float4 tex_texelsize, float2 uv, half4 color, half luminance, half strength)
        {
            half strengthd = (sin(strength * strength) + half(1.0)) * half(0.5);
            half4 mtex = tex2D(tex, uv);
            half4 mtexo = mtex;
            //提取
            mtex = half4(mtex.rgb * (half(1) - mtex.a) * half(3),mtex.w);
            
            //模糊
            half4 blurbloom = Hsys::Blur::DownSample(tex, tex_texelsize, uv);
            blurbloom = Hsys::Blur::UpSample(tex, blurbloom, uv);
            
            mtex = (mtex * (half(2.51) * mtex + half(0.03))) / (mtex * (half(2.43) * mtex + half(0.53)) + half(0.14));
            mtex = half4(saturate(pow(mtex.rgb, half3(0.454545, 0.454545, 0.454545))), mtex.w);
            mtex *= luminance;
            
            //Hsys::Blur::DownSample(tex, tex_texelsize, uv)
            return mtex + strengthd * blurbloom * color * step(0, mtexo.a);
        }
        //HDR输入 带 HDR 颜色的
        half4 Bloom_HDRColor_Box(sampler2D tex, sampler2D blur,float2 uv, half2 offset, half4 HDRcolor, half strength)
        {
            half4 blurbloom = Hsys::Blur::Blur_Box_UseOfBloom(blur, uv, float2(offset.x, offset.y), true);
            half4 mtex = tex2D(tex, uv);
            //mtex = (mtex * (half(2.51) * mtex + half(0.03))) / (mtex * (half(2.43) * mtex + half(0.53)) + half(0.14));
            //mtex = half4(saturate(pow(mtex.rgb, half3(0.454545, 0.454545, 0.454545))), mtex.w);
            return mtex + blurbloom * HDRcolor*strength;
        }
        //half4 BloomHDRColor_Boke(sampler2D tex, float2 uv, half4 HDRcolor, half strength)

        //闪烁
        half4 Bloom_HDRFlashingColor(sampler2D tex, float4 tex_texelsize, float2 uv, half4 color, half luminance, half2 strength_time)
        {
            half strength = (sin(strength_time.y * strength_time.x) + half(1.0)) * half(0.5);
            half4 mtex = tex2D(tex, uv);
            half4 mtexo = mtex;

            
            //提取
            mtex = half4(mtex.rgb * (half(1) - mtex.a) * half(3), mtex.w);
            
            //模糊
            half4 blurbloom = Hsys::Blur::DownSample(tex, tex_texelsize, uv);
            blurbloom = Hsys::Blur::UpSample(tex, blurbloom, uv);
            mtex = (mtex * (half(2.51) * mtex + half(0.03))) / (mtex * (half(2.43) * mtex + half(0.53)) + half(0.14));
            mtex = half4(saturate(pow(mtex.rgb, half3(0.454545, 0.454545, 0.454545))), mtex.w);
            mtex *= luminance;
            
            //Hsys::Blur::DownSample(tex, tex_texelsize, uv)
            return mtex + strength * blurbloom * color * step(0, mtexo.a);
        }

    }
}
#endif