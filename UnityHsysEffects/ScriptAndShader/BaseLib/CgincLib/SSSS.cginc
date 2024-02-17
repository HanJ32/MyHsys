


//float4 BlurPS(PassV2P input, uniform float2 step) : SV_TARGET
//{
//    // Gaussian weights for the six samples around the current pixel:
//    //   -3 -2 -1 +1 +2 +3
//	float w[6] = { 0.006, 0.061, 0.242, 0.242, 0.061, 0.006 };
//	float o[6] = { -1.0, -0.6667, -0.3333, 0.3333, 0.6667, 1.0 };
//
//    // Fetch color and linear depth for current pixel:
//	float4 colorM = colorTex.Sample(PointSampler, input.texcoord);
//	float depthM = depthTex.Sample(PointSampler, input.texcoord);
//
//    // Accumulate center sample, multiplying it with its gaussian weight:
//	float4 colorBlurred = colorM;
//	colorBlurred.rgb *= 0.382;
//
//    // Calculate the step that we will use to fetch the surrounding pixels,
//    // where "step" is:
//    //     step = sssStrength * gaussianWidth * pixelSize * dir
//    // The closer the pixel, the stronger the effect needs to be, hence
//    // the factor 1.0 / depthM.
//	float2 finalStep = colorM.a * step / depthM;
//
//    // Accumulate the other samples:
//    [unroll]
//	for (int i = 0; i < 6; i++)
//	{
//        // Fetch color and depth for current sample:
//		float2 offset = input.texcoord + o[i] * finalStep;
//		float3 color = colorTex.SampleLevel(LinearSampler, offset, 0).rgb;
//		float depth = depthTex.SampleLevel(PointSampler, offset, 0);
//
//        // If the difference in depth is huge, we lerp color back to "colorM":
//		float s = min(0.0125 * correction * abs(depthM - depth), 1.0);
//		color = lerp(color, colorM.rgb, s);
//
//        // Accumulate:
//		colorBlurred.rgb += w[i] * color;
//	}
//
//    // The result will be alpha blended with current buffer by using specific
//    // RGB weights. For more details, I refer you to the GPU Pro chapter :)
//	return colorBlurred;
//}

//=========================================================================================================

//float distance(float3 posW, float3 normalW, int i)
//{
//  // Shrink the position to avoid artifacts on the silhouette:
//	posW = posW - 0.005 * normalW;
//
//  // Transform to light space:
//	float4 posL = mul(float4(posW, 1.0), lights[i].viewproj);
//
//  // Fetch depth from the shadow map:
//  // (Depth from shadow maps is expected to be linear)
//	float d1 = shwmaps[i].Sample(
//
//	sampler, posL.
//	xy / posL.w);
//	float d2 = posL.z;
//
//  // Calculate the difference:
//	return abs(d1 - d2);
//}
//
//// This function can be precomputed for efficiency
//float3 T(float s)
//{
//	return float3(0.233, 0.455, 0.649) * exp(-s * s / 0.0064) +
//         float3(0.1, 0.336, 0.344) * exp(-s * s / 0.0484) +
//         float3(0.118, 0.198, 0.0) * exp(-s * s / 0.187) +
//         float3(0.113, 0.007, 0.007) * exp(-s * s / 0.567) +
//         float3(0.358, 0.004, 0.0) * exp(-s * s / 1.99) +
//         float3(0.078, 0.0, 0.0) * exp(-s * s / 7.41);
//}
//
//// The following is done for each light 'i'.
//
//// Calculate the distance traveled by the light inside of the object:
//// ('strength' modulates the strength of the effect; 'normalW' is the vertex
////  normal)
//float s = distance(posW, normalW, i) / strength;
//
//// Estimate the irradiance on the back:
//// ('lightW' is the regular light vector)
//float irradiance = max(0.3 + dot(-normalW, lightW), 0.0);
//
//// Calculate transmitted light:
//// ('attenuation' and 'spot' are the usual spot light factors, as in regular
////  reflected light):
//float3 transmittance = T(s) * lights[i].color *
//                       attenuation * spot * albedo.rgb * irradiance;

// Add the contribution of this light:
	//color += transmittance + reflectance;

#ifndef __SSSS_CGINC_
#define __SSSS_CGINC_

namespace Hsys
{
    inline float Sample_Inverse_Depth(float2 _uv, sampler2D _camerap_depth_tex, float unity_CameraProjection_m11)
	{
        
        float z = LinearEyeDepth(tex2D(_camerap_depth_tex, _uv).r);

        float size = 0.5 / (z + 0.5);
        return size * unity_CameraProjection_m11 * 0.6;
    }
    
    inline float SAMPLE_INVERSE_DEPTH_LINEAR(float2 _uv, sampler2D _camerap_depth_tex)
    {
        return saturate(1.0 - Linear01Depth(tex2D(_camerap_depth_tex, _uv).r));
    }


}

#endif