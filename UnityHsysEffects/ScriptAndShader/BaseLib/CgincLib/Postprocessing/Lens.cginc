#ifndef __LENS_CGINC_
#define __LENS_CGINC_

namespace Hsys
{
    namespace Lens
    {
        half4 Lens_Twirl(sampler2D tex, float2 uv, float4x4 ratation, half4 center_radius)
        {
            half2 new_uv = mul(ratation, float4(uv.x, uv.y, 0, 0)).xy;
            half tp = min(1, length(uv / center_radius.zw));
            uv = lerp(new_uv, uv, tp);
            uv += center_radius.xy;
            return tex2D(tex, uv);
        }
        
        half4 Lens_Twirl(sampler2D tex, float2 uv, float2 ratation_uv, half4 center_radius, bool useUnity)
        {
            half tp = min(1, length(uv / center_radius.zw));
            uv = lerp(ratation_uv, uv, tp);
            uv += center_radius.xy;
            return tex2D(tex, uv);
        }
        
        half4 Lens_Twist(sampler2D tex, float2 uv, half4 center_radius,half _angle)
        {
            half angle = max(0,1.0 - length(uv / center_radius.zw));
            angle = max(0, angle);
            angle = angle * angle * _angle;
            half coslength, sinlength;
            
            sincos(angle, sinlength, coslength);
            half2 res_uv;
            res_uv.x = coslength * uv[0] - sinlength * uv[1];
            res_uv.y = sinlength * uv[0] + coslength * uv[1];
            res_uv += center_radius.xy;
            
            return tex2D(tex, res_uv);
        }

    }
}

#endif