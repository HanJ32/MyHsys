#ifndef __REAL_LIGHT_HLSL_
#define __REAL_LIGHT_HLSL_

namespace Hsys
{
    namespace RealLight
    {
        struct ray
        {
            float3 pos;
            float min;
            float3 dir;
            float max;
        };
    }
    
        
    bool SlabsBoxTest(float3 p0, float3 p1, float3 rayorigin, float3 invraydir,
                        RealLight::real raytmin, RealLight::real raytmax)
    {
        float3 tlower = (p0 - rayorigin) * invraydir;
        float3 tupper = (p1 - rayorigin) * invraydir;
        
        float4 tmins = float4(min(tlower, tupper),raytmin);
        float4 tmaxs = float4(min(tlower, tupper), raytmax);
        
        RealLight::real tboxmin = max_component(tmins);
        RealLight::real tboxmax = min_component(tmaxs);
        return tboxmax >= tboxmin;
    }
    
    void GenerateHaltonSequence(int N, int b, float sequence[])
    {
        int n = 0, d = 1, x;
        for (int index = 0; index < N; ++index)
        {
            x = d - n;
            if(x == 1)
            {
                n = 1;
                d *= b;
            }
            else
            {
                y = d / b;
                while(x <= y)
                {
                    y /= b;
                }
                n = (b + 1) * y - x;
            }
            sequence[i] = (float) n / (float) d;
        }

    }
}

#endif