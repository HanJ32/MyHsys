#ifndef __SHADOW_CGINC_
#define __SHADOW_CGINC_
namespace Hsys
{
#include "../HlslLib/Constant.hlsl"
#ifndef USE_HALF_DEAL

    float PCF_Shadow(sampler2D shadowmap, float3 map, float2 uv, int level)
    {
        float s = 0.0;
        float sp = 0.0;
        int halflevel = max(0, (level - 1) / 2);
        for (int index = -halflevel; index <= halflevel; ++index)
        {
            for (int index2 = -halflevel; index2 < halflevel; ++index)
            {
                sp = Hsys::Private_Hsys::DecodeFloatRGBA(tex2D(shadowmap, uv + float2(index, index2) * map.x));
                s += sp < map.y ? map.z : 1;
            }
        }
        return s / pow(level, 2.0);
    }
    //==============================================
    //ѧϰ : https://github.com/wlgys8/SRPLearn/wiki/PCFSampleOptimize
    //==============================================
    
    float PCF_Shadow(sampler2D shadowmap, float4 coords)
    {
        Hsys::Private_Hsys::uniformDiskSamples(coords.rg);

    }
    
    float PCSS(sampler2D maptex, float3 map, float2 uv)
    {
        
    }
#endif


#ifdef USE_HALF_DEAL

#endif
}
#endif