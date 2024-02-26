#ifndef __DEFORM_CGINC_
#define __DEFORM_CGINC_

namespace Hsys
{
	namespace Private_Value
	{
	#ifndef USE_HALF_DEAL
		float2x2 Rotation(float a)
		{
			float s = sin(a);
			float c = cos(a);
			return float2x2(c,-s,s,c);
		}

		float2 Twirl(float2 uv, float2 center, float rg, float angle)
		{
			float d = distance(uv, center);
			uv -= center;
			d = smoothstep(0, rg, rg - d) * angle;
			uv = uv * Rotation(d);
			uv += center;
			return uv;
		}

		float2 Inflate(float2 uv, float2 center, float rg, float strength)
		{
			float dist = distance(uv, center);
			float2 dir = normalize(uv - center);
			float scale = 1.0 - strength + strength * smoothstep(0.0, 1.0, dist/rg);
			float newdist = dist * scale;
			return center + newdist * dir;
		}

		float2 Pinch(float2 uv, float2 targetpoint, float2 vt, float rg)
		{
			float2 center = targetpoint + vt;
			float dist = distance(uv, targetpoint);
			float2 pt = targetpoint + smoothstep(0.0,1.0,dist/rg) * rg;
			return uv - center + pt;
		}
	#endif
	}
}

#endif