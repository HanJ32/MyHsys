#ifndef __PRE_SSS_HLSL_
#define __PRE_SSS_HLSL

namespace Hsys
{
	#include "Constant.hlsl"
	//float PHBeckmann(float ndoth, float m)
	//{
	//	float alpha = acos(ndoth);
	//	float ta = tan(alpha);
	//	float val = 1.0 / (m * m * pow(ndoth, 4.0)) * exp(-(ta * ta) / (m * m));
	//	return val;
	//}
	//
	//float KSTextureCompute(float2 tex)
	//{
    //// Scale the value to fit within [0,1] – invert upon lookup.
	//	return 0.5 * pow(PHBeckmann(tex.x, tex.y), 0.1);
	//}
	//
	//float fresnelReflectance(float3 H, float3 V, float F0)
	//{
	//	float base = 1.0 - dot(V, H);
	//	float exponential = pow(base, 5.0);
	//	return exponential + F0 * (1.0 - exponential);
	//}
	//
	//float KS_Skin_Specular(
    //float3 N, // Bumped surface normal
    //float3 L, // Points to light
    //float3 V, // Points to eye
    //float m, // Roughness
    //float rho_s, // Specular brightness
    //uniform texobj2D beckmannTex)
	//{
	//	float result = 0.0;
	//	float ndotl = dot(N, L);
	//	if (ndotl > 0.0)
	//	{
	//		float3 h = L + V; // Unnormalized half-way vector
	//		float3 H = normalize(h);
	//		float ndoth = dot(N, H);
	//		float PH = pow(2.0 * f1tex2D(beckmannTex, float2(ndoth, m)), 10.0);
	//		float F = fresnelReflectance(H, V, 0.028);
	//		float frSpec = max(PH * F / dot(h, h), 0);
	//		result = ndotl * rho_s * frSpec; // BRDF * dot(N,L) * rho_s
	//	}
	//	return result;
	//}
	//Adapted From https://therealmjp.github.io/posts/sss-intro/
	
	
	
	
	//=========================================
	//双向子面散射反射分布函数
	//Adapted From: http://www.graphics.stanford.edu/papers/bssrdf/bssrdf.pdf
    void BSRDF()
    {

    }
	
	//Adapted From : https://graphics.stanford.edu/papers/fast_bssrdf/fast_bssrdf.pdf
    void Fast_BSSRDF()
    {
		
    }
	//==========================================
	//Adapted From https://graphics.pixar.com/library/ApproxBSSRDF/paper.pdf
    //这个是基于BSRDF 和 Diffusion model[Jensen et al. 2001]/[d`Eon and Lrving 2011] 和 Diffuse single-scattering model [Habel et al. 2013]
	//的经验模型
    float BSRDF_EmpiricalModel(float _A, float _r)
    {
        float A = _A;
        float s = 1.9 - A + 3.5 * pow((A - 0.8), 2.0);
		//float s = 3.5 + 100 * pow((A - 0.33), 4.0);

        float r = _r;
        float R_r = A * s * ((pow(EXP, -s * r) + pow(EXP, -(s * r) / 3)) / 8 * PI * r);
        return R_r;
    }
	
    
	
	//Adapted From https://zero-radiance.github.io/post/sampling-diffusion/
	//预积分模糊反射采样
    void SampleBurleyDiffusionProfile(float u, float rcpS, out float r, out float rcpPdf)
    {
        u = 1 - u;
		
        float g = 1 + (4 * u) * (2 * u + sqrt(1 + (4 * u) * u));
        float n = exp2(log2(g) * (-1.0 / 3.0));
        float p = (g * n) * n;
        float c = 1 + p + n;
        float x = (3 / LOG2_E) * log2(c * rcp(4 * u));
		
        float rcpExp = ((c * c) * c) * rcp((4 * u) * ((c * c) + (4 * u) * (4 * u)));
        r = x * rcpS;
        rcpPdf = (8 * PI * rcpS) * rcpExp;
    }
	
	//屏幕空间次表面散射
	//Adapted From : https://www.iryoku.com/sssss/downloads/Screen-Space-Perceptual-Rendering-of-Human-Skin.pdf
    float SSSS()
    {
		
    }
	
	//预集成次表面散射
    float SSSS_Pre()
    {
		
    }
    namespace Private_Hsys
    {
		
		//CDF_r BSRDF_EmpiricalModel 辅助
#ifndef USE_HALF_DEAL
        float CDF_r(float r, float d)
        {
            return 1 - (1 / 4) * pow(EXP, -(r / d)) - (3 / 4) * pow(EXP, -(r / 3 * d));
        }
#endif		
#ifdef USE_HALF_DEAL 
		half CDF_r(half r, half d)
        {
            return 1 - (1 / 4) * pow(EXP, -(r / d)) - (3 / 4) * pow(EXP, -(r / 3 * d));
        }
#endif
		//最小动态处理
#ifdef USE_MIN_DEAL
		min10float CDF_R (min10float r, min10float d)
        {
            return 1 - (1 / 4) * pow(EXP, -(r / d)) - (3 / 4) * pow(EXP, -(r / 3 * d));
        }
#endif 		
    };
};
#endif