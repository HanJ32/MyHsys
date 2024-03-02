#ifndef __AO_CGINC_
#define __AO_CGINC_

namespace Hsys
{
    namespace AO
    {
        //====================================
        //From UnityCG.cginc
        float4 DecodeViewNormalStereo(float4 enc4)
        {
            float kScale = 1.7777;
            float3 nn = enc4.xyz * float3(2 * kScale, 2 * kScale, 0) + float3(-kScale, -kScale, 1);
            float g = 2.0 / dot(nn.xyz, nn.xyz);
            float3 n;
            n.xy = g * nn.xy;
            n.z = g - 1;
            return n;
        }
        //传统SSAO
        half4 SSAO(sampler2D tex, sampler2D depth_normal_tex, float2 uv, 
                    in float4 inviewVec, float linear01depth, out float viewNormal, 
                    float4x4 samplekernelarray,out float4 outviewVec, float4 screenpos, 
                    float3 ProjectionParams, float4 unity_StereoScaleOffset, in float4x4 unity_CameraInvProjection,
                    float samplekernelradius, float strength, int count)
        {
            half4 mtex = tex2D(tex, uv);
            half4 depth_normal_mtex = tex2D(depth_normal_tex, uv);
            
            half depth = dot(depth_normal_mtex.xy, half2(1.0, 1 / 255.0));

            half kscale = 1.7777;
            half3 nn = depth_normal_mtex.xyz * half3(2 * kscale, 2 * kscale, 0) + half3(-kscale, -kscale, 1);
            half g = 2.0 / dot(nn.xyz, nn.xyz);
            half3 normal;
            normal.xy = g * nn.xy;
            normal.z = g - 1;
            
            half4 o = screenpos * 0.5f;
            o.xy = float2(o.x, o.y * ProjectionParams.x) + o.w;
            o.zw = screenpos.zw;
#if defined(UNITY_SINGLE_PASS_STEREO)
            half4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
            o.xy = uv.xy * scaleOffset.xy + scaleOffset.zw * w;
#endif
            half4 ndcPos = (screenpos / screenpos.w) * 2 - 1;
            half3 clipVec = half3(ndcPos.x, ndcPos.y, 1.0) * ProjectionParams.z;
            outviewVec = mul(unity_CameraInvProjection, clipVec.xyzz).xyz;
            half3 viewPos = linear01depth * inviewVec;
            
            viewNormal = normalize(viewNormal * half3(1, 1, -1));
            half3 randvec = normalize(half3(1, 1, 1));
            half3 tangent = normalize(randvec - viewNormal * dot(randvec, viewNormal));
            half3 bitangent = cross(viewNormal, tangent);
            half3x3 TBN = float3x3(tangent, bitangent, viewNormal);
            half3 randomVec, randomPos;
            half3 rclipPos;
            half2 rscreenPos;
            half randomdepth;
            half3 randomnormal;
            half4 rcdn;
            half weight, ao;
            for (int index = 0; index < count; index += 1)
            {
                randomVec = mul(samplekernelarray[index].xyz, TBN);
                randomPos = viewPos + randomVec * samplekernelradius;

                rclipPos = mul((half3x3) unity_CameraInvProjection, randomPos);
                rscreenPos = (rclipPos.xy / rclipPos.z) * 0.5 + 0.5;
                rcdn = tex2D(depth_normal_tex, rscreenPos);
                weight = smoothstep(0, 0.2, length(randomVec.xy));

            }
            ao = max(0.0, 1 - (ao / count) * strength);
            
            return half4(ao, ao, ao, 1.0);

        }
        
        half4 SSAO_easy(sampler2D tex, sampler2D depth_tex, float4 depth_texelsize, sampler2D noise_tex, float noisesize, float uv, float4x4 projectionMatrix, float kernel[], float radius, float4x4 viewMatrix, int kernelsize)
        {
            
            half2 texelSize = depth_texelsize.xy;
            half3 depthpos = tex2D(depth_tex, uv).xyz;
            half3 normal = tex2D(depth_tex, uv).xyz;

            half3 randomVec = tex2D(noise_tex, uv * noisesize).xyz;
            half3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
            half3 bitangent = cross(normal, tangent);
            float3x3 TBN = half3x3(tangent, bitangent, normal);

            float ao = 0.0;
            for (int i = 0; i < kernelsize; ++i)
            {
                float3 samplePos = kernel[i] * radius;
                half4 offset = half4(mul(TBN, samplePos), 1.0);
                offset *= mul(projectionMatrix, viewMatrix);
                offset.xyz /= offset.w;
                offset.xyz = offset.xyz * 0.5 + 0.5;

                half sampleDepth = tex2D(depth_tex, offset.xy).r;
                half rangeCheck = smoothstep(0.0, 1.0, radius / abs(depthpos.z - sampleDepth));
                ao += (sampleDepth >= samplePos.z ? 1.0 : 0.0) * rangeCheck;
            }

            ao = 1.0 - (ao / kernelsize);
            return half4(half3(ao), 1.0);
        }
        //float4 SSAO(sampler2D tex, sampler2D depth_tex, float2 uv)
        //{
        //    float4 mtex = tex2D(tex, uv);
        //    float4 depth_mtex = tex2D(depth_tex, uv);
        //    float2 kDecodeDot = float2(1.0, 1 / 255.0);
        //}
        
        //光影追踪上使用的
        half4 SFSSAO(sampler2D tex, float4 tex_texelsize, sampler2D depth_tex, sampler2D depth_normal_tex, sampler2D dither_tex, float2 uv, 
                    float4x4 projectionmatrixinverse, float4x4 unity_worldcamera, float2 radius_orthographic, 
                    float3 screenParams_sampledistributioncurve, float2 drawdistance_drawDistancefadesize, float3 zthickness_bias_intensity, 
                    int3 preservedetails_downsamp_halfsampling)
        {	
            float depth = tex2Dlod(depth_tex, float4(uv.x, uv.y, 0.0, 0.0)).x;
		
            float4 viewPosition = mul(projectionmatrixinverse, float4(uv.x * 2.0 - 1.0, uv.y * 2.0 - 1.0, 2.0 * depth - 1.0, 1.0));
            viewPosition /= viewPosition.w;
            
            float3 viewSpacePosition = viewPosition.xyz;
            float3 normal = DecodeViewNormalStereo(tex2D(depth_normal_tex, uv));
				
            float2 noiseCoord = uv * float2(tex_texelsize.zw);
            noiseCoord /= (5.0 - preservedetails_downsamp_halfsampling.x * 2) + (float) preservedetails_downsamp_halfsampling.y * (5.0 - preservedetails_downsamp_halfsampling.x * 2);
            float dither = 0.5;
            dither = tex2Dlod(dither_tex, float4(noiseCoord.xy, 0.0, 0.0)).a;
				
            float3 origin = viewSpacePosition.xyz;
				
            int numRaysPassed = 0;
				
            float ao = 0.0f;
				
            int numSamples = (8 + preservedetails_downsamp_halfsampling.x * 4) - preservedetails_downsamp_halfsampling.z * (4 + preservedetails_downsamp_halfsampling.x * 2);
				
            float weights = 0.0;

            float radius2 = radius_orthographic.x;
            float spread = radius2 / lerp((-origin.z + 0.0001), 10.0, radius_orthographic.y);
				
            const float sweeps = 31.0;
				
            float3 bleed = float3(0.0, 0.0, 0.0);
				
            float4 emphasisDir = mul(unity_worldcamera, float4(0.0, -1.0, 0.0, 0.0));
            emphasisDir.xyz = normalize(emphasisDir.xyz);
            emphasisDir.x *= -1.0f;
            emphasisDir.y *= -1.0f;
				
            for (int i = 0; i < numSamples; i++)
            {
                float2 kernel;
                float fi = (float) i / (float) numSamples;
                kernel.x = sin(fi * 3.14159265 * sweeps * 2.0 + dither * 6.0);
                kernel.y = cos(fi * 3.14159265 * sweeps * 2.0 + dither * 6.0);
                float2 kernelNormalized = kernel;
                kernel.y *= screenParams_sampledistributioncurve.x / screenParams_sampledistributioncurve.y;
                kernel *= pow(dither, screenParams_sampledistributioncurve.z);
					
                float2 finalKernel = kernel * spread + kernelNormalized * 0.001;
                
                float2 viewspaceuv = clamp(uv + finalKernel, float2(0.0, 0.0), float2(1.0, 1.0));
                float depth = tex2Dlod(depth_tex, float4(viewspaceuv.x, viewspaceuv.y, 0.0, 0.0)).x;
		
                float4 viewPosition = mul(projectionmatrixinverse, float4(viewspaceuv.x * 2.0 - 1.0, viewspaceuv.y * 2.0 - 1.0, 2.0 * depth - 1.0, 1.0));
                viewPosition /= viewPosition.w;
                float3 samplePosition = viewPosition.xyz;
					
                float3 sampleVector = normalize(samplePosition - origin);
					
                float angle = dot(sampleVector, normal.xyz);
					
                float weight = 1.0;
					
                float dist = length(samplePosition.z - origin.z);
                dist = max(0.0, dist - 0.8);
                weight = pow(saturate(1.0 - dist / (length(dither) * zthickness_bias_intensity.x * spread * 25.5)), 1.0);
					
                weights += weight;
					
                float emphasisWeight = (dot(sampleVector, emphasisDir.xyz)) * 4.0 + 1.0;
					
                if (angle > zthickness_bias_intensity.y)
                {
                    ao += saturate(angle * (1.0 + zthickness_bias_intensity.y) - zthickness_bias_intensity.y) * weight * (2.0 - 1.0 * dither);
                }
            }
				
            if (weights < 0.01)
            {
                weights = 1.0;
            }
				
            ao /= weights;
            ao = 1.0f - ao * 1.2;
            ao = saturate(ao * 1.0);
            ao = pow(ao, zthickness_bias_intensity.z);
				
            ao = lerp(1.0, ao, saturate((drawdistance_drawDistancefadesize.x / drawdistance_drawDistancefadesize.y) + (origin.z / drawdistance_drawDistancefadesize.y)));
				
            float4 res = float4(normalize(float3(1.0, 1.0, 1.0)), ao);
				
            return res;
        }
        
        //From URP SSAO
        //=======================================================================================================

        
        float4 SSAO_URP(sampler2D tex, float4 tex_texelsize, sampler2D random_tex, sampler2D depth_tex, float2 uv, 
                        float4 zbufferparams, float radius, float3 falloff_area_base, float totalstrength)
        {
            float2 res = tex_texelsize.xy;
// 
            const int samples = 16;
            float3 sample_sphere[samples] =
            {
                float3(0.5381, 0.1856, -0.4319), float3(0.1379, 0.2486, 0.4430),
                    float3(0.3371, 0.5679, -0.0057), float3(-0.6999, -0.0451, -0.0019),
                    float3(0.0689, -0.1598, -0.8547), float3(0.0560, 0.0069, -0.1843),
                    float3(-0.0146, 0.1402, 0.0762), float3(0.0100, -0.1924, -0.0344),
                    float3(-0.3577, -0.5301, -0.4358), float3(-0.3169, 0.1063, 0.0158),
                    float3(0.0103, -0.5869, 0.0046), float3(-0.0897, -0.4940, 0.3287),
                    float3(0.7119, -0.0154, -0.0918), float3(-0.0533, 0.0596, -0.5411),
                    float3(0.0352, -0.0631, 0.5460), float3(-0.4776, 0.2847, -0.0271)
                    };
  
            float3 random = normalize(tex2D(random_tex, uv * 4.123).rgb);
                
            float depth = (1.0 / (zbufferparams.z * tex2D(depth_tex, uv) + zbufferparams.w)).r;

            float3 position = float3(uv, depth);
            
            const float2 offset1 = float2(0.0, 0.001);
            const float2 offset2 = float2(0.001, 0.0);
  
            float depth1 = (1.0 / (zbufferparams.z * tex2D(depth_tex, uv + offset1) + zbufferparams.w)).r;
            
            float depth2 = (1.0 / (zbufferparams.z * tex2D(depth_tex, uv + offset2) + zbufferparams.w)).r;
            
  
            float3 p1 = float3(offset1, depth1 - depth);
            float3 p2 = float3(offset2, depth2 - depth);
  
            float3 normal = cross(p1, p2);
            normal.z = -normal.z;

            float3 mnormal = normalize(normal);

            float radius_depth = radius / depth;
            float occlusion = 0.0;
            for (int i = 0; i < samples; i++)
            {
                
                float3 ray = radius_depth * reflect(sample_sphere[i], random);
                float3 hemi_ray = position + sign(dot(ray, normal)) * ray;
                    
                float occ_depth = (1.0 / (zbufferparams.z * tex2D(depth_tex, saturate(hemi_ray.xy)) + zbufferparams.w)).r;
                float difference = depth - occ_depth;
                    
                occlusion += step(falloff_area_base.x, difference) * (1.0 - smoothstep(falloff_area_base.x, falloff_area_base.y, difference));
            }
                
            float ao = 1.0 - totalstrength * occlusion * (1.0 / samples);
            float4 color;
            color.rgb = saturate(ao + falloff_area_base.z);

            return color;
        }
        //======================================================================================================
        
        //学习 https://research.nvidia.com/sites/default/files/pubs/2010-06_Ambient-Occlusion-Volumes/McGuire10AOV.pdf
        half4 HBAO(sampler2D tex, sampler2D depth_tex,float2 uv)
        {
            
        }
        
        float4 HBAO(sampler2D tex, float2 uv)
        {
            
        }
        
        float4 HBAO_easy(sampler2D tex, sampler2D depth_tex, float2 uv,float screensize, float4x4 projectionMatrix, float3 redius_bias_strength)
        {
            float ao = 0.0;
            float depth = tex2D(depth_tex, uv);
            float3 origin = float3(uv, depth);
            
            float2 offset, samplecoord;
            float sampledepth;
            for (int index = -1; index <= 1; index+=1)
            {
                for (int index2 = -1; index2 <= 1; index2 +=1)
                {
                    offset = float2(index, index2) / screensize;
                    samplecoord = uv + offset;
                    sampledepth = tex2D(depth_tex, uv).r;
                    ao += smoothstep(depth - redius_bias_strength.y, depth + redius_bias_strength.y, depth - redius_bias_strength.z);
                }
            }
            ao /= 9.0;
            ao = 1.0 - ao;
            ao *= redius_bias_strength.z;
            return float4(ao, ao, ao, 1.0);

        }
        
        half4 HDAO(sampler2D tex, float2 uv)
        {
            
        }
        
        float4 HDAO(sampler2D tex, float2 uv)
        {
            
        }
        
        half4 VAAO(sampler2D tex, float2 uv)
        {
            
        }

        

    }
}

#endif